using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class BaseQusetGiver : MonoBehaviour, IQuestGiver
{
    [Header("References")]
    public PlayerManager playerManager;
    public AudioManager audioManager;
    public QuestManager questManager;

    protected QuestManager.DisableUICallBack currentQuestCallback;
    protected int currentQuestIconId;

    [Header("Quests")]
    [NonReorderable]public List<quest2> Quests;
    //public quest2 Quest;
    protected quest2 currentQuest;

    protected int questIndexTracker = 0;

    //callback to the dialogue trigger, so can start next dialogue
    protected Action<QuestReturnState> questFinishedCallback;

    protected bool playerInRange;
    protected bool playerInUI;
    protected bool readyToStartQuest;
    protected bool playerOnQuest;

    protected bool startNow = false;

    [HideInInspector] public bool NoMoreQuests = false;

    protected bool acceptButtonHit = false;
    private int framesActiveAcc = 0;

    protected bool DeclineButtonHit = false;
    private int framesActiveDec = 0;

    protected virtual bool InputAccept()
    {
        if (ProgressManager.InCinemtatic) { return false; }

        if (playerInUI && acceptButtonHit) { return true; }

        return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1");
    }

    //returns true if the player presses decline button or key
    protected virtual bool InputDecline()
    {
        if (playerInUI && DeclineButtonHit) { return true; }

        return Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape);
    }

    protected virtual void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        currentQuest = Quests[questIndexTracker];

        QuestManager.AcceptButtonClickedEvent += AcceptButtonClick;
        QuestManager.DeclineButtonClickedEvent += DeclineButtonClick;
    }

    protected virtual void OnDestroy()
    {
        QuestManager.AcceptButtonClickedEvent -= AcceptButtonClick;
        QuestManager.DeclineButtonClickedEvent -= DeclineButtonClick;
    }

    private void StartQuest()
    {


    }

    protected virtual void Update()
    {
        ManageNearbyPlayer();
        UpdateQuestIcons();

        ManageButtonFlags();

        if(Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha4)) {
            Cheat_RecieveCurrentQuestMaterials();
        }
    }

    protected virtual void ManageNearbyPlayer()
    {
        if (startNow || (InputAccept()
            && playerInRange && !playerInUI && readyToStartQuest)) {

            bool playerCanCompleteQuest = CheckCanCompleteQuest();

            //player gets shown quest to accept it
            if (!playerOnQuest) {
                currentQuestCallback = questManager.SetQuestUI(currentQuest, QuestManager.QuestUIStatus.UnAccepted);
                playerInUI = true;
                startNow = false;
            }
            else if (playerCanCompleteQuest) {
                currentQuestCallback = questManager.SetQuestUI(currentQuest, QuestManager.QuestUIStatus.CanComplete);
                playerInUI = true;
            }
            //player gets shown quest to finish it
            else {
                currentQuestCallback = questManager.SetQuestUI(currentQuest, QuestManager.QuestUIStatus.InProgress);
                playerInUI = true;
            }
        }
        else if (playerInUI) {
            bool playerCancompleteQuest = CheckCanCompleteQuest();
            ManageActiveUI(playerCancompleteQuest);
        }

    }

    protected virtual void UpdateQuestIcons()
    {
        if (playerOnQuest) {
            questManager.UpdateQuestIcon(currentQuestIconId, IconInfo());
        }

    }

    //called each frame when the player is looking at the quest UI
    protected virtual void ManageActiveUI(bool canComplete)
    {
        //player out of range disable menu
        if (!playerInRange) {
            currentQuestCallback.Invoke();
            playerInUI = false;
        }

        //player completes quest
        if (InputAccept() && canComplete) {
            CompleteQuest();
        }

        //accept quest
        else if (InputAccept() && !playerOnQuest) {
            AcceptQuest();
        }

        //decline quest
        if (InputDecline()) {
            currentQuestCallback.Invoke();
            playerInUI = false;
            playerManager.inQuestScreen = false;

            //player declined the quest so will show dialogue again
            if (!playerOnQuest) {
                questFinishedCallback.Invoke(QuestReturnState.Declined);
            }
        }
    }
    protected virtual bool CheckCanCompleteQuest()
    {
        int goalsCompleted = 0;

        foreach (Goal questGoal in currentQuest.goals) {
            if (playerManager.InventoryContains(questGoal.itemId, questGoal.count, out int _)) {
                goalsCompleted++;
            }
        }

        return goalsCompleted >= currentQuest.goals.Count;
    }

    protected virtual void AcceptQuest()
    {
        StartQuest();

        audioManager.Play("QuestAccept");

        currentQuestCallback.Invoke();

        playerInUI = false;
        playerOnQuest = true;
        playerManager.inQuestScreen = false;

        currentQuestIconId = questManager.SetQuestIcon(IconInfo());

        questFinishedCallback.Invoke(QuestReturnState.Accepted);
    }

    protected virtual void CompleteQuest()
    {
        //remove items for quest
        foreach (var goal in currentQuest.goals) {
            playerManager.inventoryManager.RemoveItems(goal.itemId, goal.count);    
        }

        audioManager.Play("QuestComplete");
        currentQuestCallback.Invoke();
        playerInUI = false;
        playerOnQuest = false;
        playerManager.inQuestScreen = false;

        playerManager.PenguinCash += currentQuest.CoinReward;
        questManager.RemoveQuestIcon(currentQuestIconId);

        GiveRewards();

        readyToStartQuest = false;

        SetNextQuest();

        if(questManager.CheckLevelProgression(currentQuest.ProgressMainLevel, currentQuest.ProgressSideLevel, CinematicFinsihed)) {
            //if there is a cinematic, start dialogue after it's finished
        }
        else {
            //otherwise start dialogue right away
            questFinishedCallback.Invoke(QuestReturnState.Completed);
        }
    }

    protected virtual void GiveRewards()
    {


    }

    private void CinematicFinsihed()
    {
        questFinishedCallback.Invoke(QuestReturnState.Completed);
    }

    protected virtual void SetNextQuest()
    {
        if(questIndexTracker < Quests.Count - 1) {
            questIndexTracker++;
            currentQuest = Quests[questIndexTracker];
        }
        else {
            NoMoreQuests = true;
        }

    }

    protected virtual QuestIconInfo IconInfo()
    {
        List<int> goalItemIds = new List<int>();
        List<int> goalItemProgress = new List<int>();
        List<int> goalItemMax = new List<int>();

        foreach (var goal in currentQuest.goals) {
            goalItemIds.Add(goal.itemId);
            playerManager.InventoryContains(goal.itemId, goal.count, out int currentAmount);
            goalItemProgress.Add(currentAmount);
            goalItemMax.Add(goal.count);
        }

        QuestIconInfo iconInfo = new QuestIconInfo(
            currentQuest.QuestTitle,
            goalItemIds,
            goalItemProgress,
            goalItemMax
            );
        iconInfo.QuestIconID = currentQuestIconId;

        return iconInfo;
    }

    public virtual void EnableQuest(Action<QuestReturnState> callback)
    {
        questFinishedCallback = callback;
        readyToStartQuest = true;
        startNow = true;
    }

    public virtual void DisableQuest()
    {
        readyToStartQuest = false;
    }

    private void ManageButtonFlags()
    {
        if (framesActiveAcc > 2) { acceptButtonHit = false; }
        if (framesActiveDec > 2) { DeclineButtonHit = false; }
        framesActiveAcc++; framesActiveDec++;
    }

    private void AcceptButtonClick()
    {
        acceptButtonHit = true;
        framesActiveAcc = 0;
    }
    private void DeclineButtonClick()
    {
        DeclineButtonHit = true;
        framesActiveDec = 0;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out _)) {
            playerInRange = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out _)) {
            playerInRange = false;
        }
    }

    protected virtual void Cheat_RecieveCurrentQuestMaterials()
    {
        if (playerOnQuest) {
            foreach (Goal questGoal in currentQuest.goals) {
                playerManager.inventoryManager.AddItems(questGoal.itemId, questGoal.count);

            }
        }
    }

}

