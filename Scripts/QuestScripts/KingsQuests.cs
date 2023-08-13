using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingsQuests : MonoBehaviour, IQuestGiver
{
    [Header("References")]
    public bool readyToStartQuest = false;
    public QuestManager questManager;
    public DonaldHouseManager donaldHouseManager;

    public AudioManager audioManager;

    public PlayerManager playerManager;
    private TMPro.TextMeshProUGUI textPromt;
    private QuestManager.DisableUICallBack currentQuestCallback;

    private bool playerInRange = false;
    private bool playerInUI = false;
    private bool playerOnQuest = false;

    public GameObject questMark;
    public GameObject questmarkAccepted;

    private Action<QuestReturnState> questFinishedCallback;

    [Header("Quests")]
    private int playersCurrentQuest = 1;

    public quest2 firstQuest;
    public quest2 secondQuest;
    public quest2 thirdQuest;
    public quest2 fourthQuest;
    public quest2 fifthQuest;

    private quest2 currentQuest;

    [HideInInspector]
    public bool startQuest = false;

    private int currentQuestIconId;

    private bool acceptButtonHit = false;
    private int framesActiveAcc = 0;
    private bool DeclineButtonHit = false;
    private int framesActiveDec = 0;


    void Start()
    {
        currentQuest = firstQuest;
        InventoryManager.ItemPickedUp += UpdateQuestProgress;
        audioManager = FindObjectOfType<AudioManager>();

        QuestManager.AcceptButtonClickedEvent += AcceptButtonClick;
        QuestManager.DeclineButtonClickedEvent += DeclineButtonClick;
    }

    private void OnDestroy()
    {
        QuestManager.AcceptButtonClickedEvent -= AcceptButtonClick;
        QuestManager.DeclineButtonClickedEvent -= DeclineButtonClick;

    }


    //returns true if the player presses accept button or key
    private bool InputAccept()
    {
        if (startQuest) {
            startQuest = false;
            return true;
        }

        if (playerInUI && acceptButtonHit) { return true; }

        return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1");
    }

    //returns true if the player presses decline button or key
    private bool InputDecline()
    {
        if (playerInUI && DeclineButtonHit) { return true; }

        return Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape);
    }

    public void EnableQuest(Action<QuestReturnState> callback)
    {
        questFinishedCallback = callback;
        readyToStartQuest = true;
        startQuest = true;
    }

    public void DisableQuest()
    {
        readyToStartQuest = false;
        questFinishedCallback = null;
    }

    void Update()
    {
        if (InputAccept() && playerInRange && !playerInUI && readyToStartQuest) {
            bool playerCanCompleteQuest = CheckCanCompleteQuest(currentQuest);

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
            //textPromt.enabled = false;

        }
        else if (playerInUI) {
            bool playerCancompleteQuest = CheckCanCompleteQuest(currentQuest);          
            ManageActiveUI(playerCancompleteQuest);
        }

        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha4)) {
            Cheat_RecieveCurrentQuestMaterials();
        }
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Minus)) {
            Cheat_SkipToEnd();
        }

        ManageButtonFlags();

        UpdateQuestProgress();
        //Debug.Log($"Current quest: {playersCurrentQuest}");
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
        if(InputAccept() && canComplete) {
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

    private void AcceptQuest()
    {
        questManager.SpokeToTheKing();

        currentQuestCallback.Invoke();
        audioManager.Play("QuestAccept");
        questMark.SetActive(false);
        questmarkAccepted.SetActive(true);
        playerInUI = false;
        playerOnQuest = true;
        playerManager.inQuestScreen = false;

        currentQuestIconId = questManager.SetQuestIcon(IconInfo(currentQuest));

        questFinishedCallback.Invoke(QuestReturnState.Accepted);
    }

    private void CompleteQuest()
    {
        //diable quest UI screen
        currentQuestCallback.Invoke();
        questManager.RemoveQuestIcon(currentQuestIconId);
        audioManager.Play("QuestComplete");
        playerInUI = false;
        playerOnQuest = false;
        playerManager.inQuestScreen = false;
        readyToStartQuest = false;

        //add quest reward
        playerManager.PenguinCash += currentQuest.CoinReward;
        GiveAccessoryRewards();

        if(playersCurrentQuest != 5) {
            //take the quest items away from inventory
            foreach (var goal in currentQuest.goals) {
                playerManager.inventoryManager.RemoveItems(goal.itemId, goal.count);
            }
        }     

        //check if need to progress the level
        if(!questManager.CheckLevelProgression(currentQuest.ProgressMainLevel, currentQuest.ProgressSideLevel, CinematicFinsihedCallBack)) {
            //if dont need to progress level instantly talk to the king, otherwise wait for the build camera to finsih
            
            //reset dialogue manager so can talk to king again 
            questFinishedCallback.Invoke(QuestReturnState.Completed);
        }        

        //move onto next quest, doesn't start untill player speaks with king again
        NextQuest();
    }

    private void CinematicFinsihedCallBack()
    {
        //reset dialogue manager so can talk to king again 
        questFinishedCallback.Invoke(QuestReturnState.Completed);
    }

    private void GiveAccessoryRewards()
    {
        if(playersCurrentQuest == 3) {
            FindObjectOfType<WardrobeInventory>().AddAccessoryToInventory(CustomizationGift.IndieHat);
        }

    }


    private void NextQuest()
    {
        playersCurrentQuest++;

        //turn back on the quest indicator
        if(playersCurrentQuest != 4){
            questMark.SetActive(true);
            questmarkAccepted.SetActive(false);
        }

        if (playersCurrentQuest == 2) {
            currentQuest = secondQuest;         
        }
        if (playersCurrentQuest == 3) {
            currentQuest = thirdQuest;    
        }
        if (playersCurrentQuest == 4) {
            currentQuest = fourthQuest;
        }
        if (playersCurrentQuest == 5) {
            currentQuest = fifthQuest;
        }

    }

    //returns true if player can complete the quest
    private bool CheckCanCompleteQuest(quest2 quest)
    {
        if(currentQuest != fifthQuest) {
            int goalsCompleted = 0;

            foreach (Goal questGoal in quest.goals) {
                if (playerManager.InventoryContains(questGoal.itemId, questGoal.count, out int _)) {
                    goalsCompleted++;
                }
            }

            return goalsCompleted >= quest.goals.Count;
        }
        else {
            return donaldHouseManager.VillageComplete;
        }

    }

    private QuestIconInfo IconInfo(quest2 quest)
    {
        Dictionary<FishType, int> playersCurrentFish = playerManager.FishCaught();
        List<int> goalItemIds = new List<int>();
        List<int> goalItemProgress = new List<int>();
        List<int> goalItemMax = new List<int>();

        foreach (var goal in quest.goals) {
            goalItemIds.Add(goal.itemId);
            playerManager.InventoryContains(goal.itemId, goal.count, out int currentAmount);
            goalItemProgress.Add(currentAmount);
            goalItemMax.Add(goal.count);

            if(playersCurrentQuest == 5) {
                goalItemProgress[0] = (donaldHouseManager.VillageComplete) ? 1 : 0;
            }
        }

        QuestIconInfo iconInfo = new QuestIconInfo(
            quest.QuestTitle,
            goalItemIds,
            goalItemProgress,
            goalItemMax
            );
        iconInfo.QuestIconID = currentQuestIconId;

        return iconInfo;
    }

    private void UpdateQuestProgress()
    {
        if (playerOnQuest) {
            questManager.UpdateQuestIcon(currentQuestIconId, IconInfo(currentQuest));
        }
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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerManager>(out _) ) {
            if (readyToStartQuest) {
                textPromt = playerManager.GetPrompt();
                //textPromt.enabled = true;
                textPromt.text = "I have a Quest Young Penguin,\n Press E to view";
            }
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<PlayerManager>(out _)) {
            textPromt = playerManager.GetPrompt();
            textPromt.enabled = false;
            textPromt.text = "";

            playerInRange = false;
        }
    }

    private void Cheat_RecieveCurrentQuestMaterials()
    {
        if (playerOnQuest) {
            //special quest 5
            if(playersCurrentQuest == 5) {
                donaldHouseManager.Cheat_FinishAllHouses();

            }
            else {
                foreach (Goal questGoal in currentQuest.goals) {
                    playerManager.inventoryManager.AddItems(questGoal.itemId, questGoal.count);
                }
            }       
        }      
    }

    private void Cheat_SkipToEnd()
    {
        GetComponent<DialogueTrigger>().dialogueTracker = 26;
        ProgressManager.CurrentLevel = 2;
        ProgressManager.LevelPogressed.Invoke();
        currentQuest = fifthQuest;
        playersCurrentQuest = 5;
    }


}
