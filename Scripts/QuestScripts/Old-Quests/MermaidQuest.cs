using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MermaidQuest : MonoBehaviour, IQuestGiver
{
    [Header("References")]
    public PlayerManager playerManager;
    public AudioManager audioManager;
    public QuestManager questManager;

    public GameObject QuestMark;
    public GameObject QuestMarkAccepted;

    private QuestManager.DisableUICallBack currentQuestCallback;
    private int currentQuestIconId;

    [Header("Quests")]
    public quest2 Quest;
    public quest2 SecondQuest;

    private quest2 currentQuest;

    //callback to the dialogue trigger, so can start next dialogue
    private Action<QuestReturnState> questFinishedCallback;

    private bool playerInRange;
    private bool playerInUI;
    private bool readyToStartQuest;
    private bool playerOnQuest;


    private bool InputAccept()
    {
        return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1") ;
    }

    //returns true if the player presses decline button or key
    private bool InputDecline()
    {
        return Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape);
    }

    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        currentQuest = Quest;
    }

    private void StartQuest()
    {

    }

    void Update()
    {
        //questManager.UpdateQuestIcon(currentQuestIconId, IconInfo());

        if (InputAccept()
            && playerInRange && !playerInUI && readyToStartQuest) {

            bool playerCanCompleteQuest = CheckCanCompleteQuest();

            //player gets shown quest to accept it
            if (!playerOnQuest) {
                currentQuestCallback = questManager.SetQuestUI(currentQuest, QuestManager.QuestUIStatus.UnAccepted);
                playerInUI = true;
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

        if (playerOnQuest) {
            questManager.UpdateQuestIcon(currentQuestIconId, IconInfo());
        }

        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha4)) {
            Cheat_RecieveCurrentQuestMaterials();
        }

    }

    //called each frame when the player is looking at the quest UI
    private void ManageActiveUI(bool canComplete)
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

    private bool CheckCanCompleteQuest()
    {
        int goalsCompleted = 0;

        Dictionary<FishType, int> playersCurrentFish = playerManager.FishCaught();

        foreach (Goal questGoal in currentQuest.goals) {
            switch (questGoal.goaltpye) {
                case GoalType.GatherItems:
                    if (playerManager.InventoryContains(questGoal.itemId, questGoal.count, out int _)) {
                        goalsCompleted++;
                    }
                    break;
            }
        }

        return goalsCompleted >= currentQuest.goals.Count;
    }

    private void AcceptQuest()
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

    private void CompleteQuest()
    {
        audioManager.Play("QuestComplete");
        currentQuestCallback.Invoke();
        playerInUI = false;
        playerOnQuest = false;
        playerManager.inQuestScreen = false;

        if(currentQuest == SecondQuest)
        {
            FindObjectOfType<WardrobeInventory>().AddAccessoryToInventory(CustomizationGift.ShellBra);
        }

        playerManager.PenguinCash += currentQuest.CoinReward;
        questManager.RemoveQuestIcon(currentQuestIconId);

        readyToStartQuest = false;

        currentQuest = SecondQuest;

        questFinishedCallback.Invoke(QuestReturnState.Completed);
    }

    private QuestIconInfo IconInfo()
    {
        List<int> goalItemIds = new List<int>();
        List<int> goalItemProgress = new List<int>();
        List<int> goalItemMax = new List<int>();

        foreach (var goal in currentQuest.goals) {
            switch (goal.goaltpye) { 
                case GoalType.GatherItems:
                    goalItemIds.Add(goal.itemId);
                    playerManager.InventoryContains(goal.itemId, goal.count, out int currentAmount);
                    goalItemProgress.Add(currentAmount);
                    break;
            }
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

    public void EnableQuest(Action<QuestReturnState> callback)
    {
        questFinishedCallback = callback;
        readyToStartQuest = true;
    }

    public void DisableQuest()
    {
        readyToStartQuest = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out _)) {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out _)) {
            playerInRange = false;
        }
    }

    private void Cheat_RecieveCurrentQuestMaterials()
    {
        if (playerOnQuest) {
            foreach (Goal questGoal in currentQuest.goals) {
                playerManager.inventoryManager.AddItems(questGoal.itemId, questGoal.count);

            }
        }

    }
}
