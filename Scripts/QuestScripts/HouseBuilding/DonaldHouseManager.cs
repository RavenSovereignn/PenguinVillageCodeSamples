using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DonaldHouseManager : BaseQusetGiver
{
    [Header("Houses")]
    /* #region HousesGameObjects
     public GameObject daisyHouse;
     public GameObject MaryHouse;
     public GameObject DonaldHouse;
     public GameObject CharlesHouse;
     public GameObject FrederickHouse;
     public GameObject Townhall;
     #endregion */

    [Header("Quests")]
    #region Quests
    public quest2 houseQDaisy;
    public quest2 houseQMary;
    public quest2 houseQDonald;
    public quest2 houseQCharles;
    public quest2 houseQFrederick;
    public quest2 houseQTownhall;
    #endregion
    [Header("Buttons")]
    #region Quest Buttons
    public Button DaisyButton;
    public Button MaryButton;
    public Button DonaldButton;
    public Button CharlesButton;
    public Button FrederickButton;
    public Button TownhallButton;
    #endregion
    [Header("Ticks")]
    #region TicksComplete
    public GameObject tickDaisy;
    public GameObject tickMary;
    public GameObject tickDonald;
    public GameObject tickCharles;
    public GameObject tickFrederick;
    public GameObject tickTownhall;
    #endregion

    [Header("References")]
    public PauseMenu pauseMenu;
    private int QuestNumberDone;
    public bool VillageComplete = false;

    public GameObject HouseQuestUI;
    public DialogueTrigger dialogueTrigger;
    public ProgressManager progressManager;

    public bool DonaldUIActive = false;

    public PlayerManager PM;
    public Cinemachine.CinemachineVirtualCamera donaldCam;

    private bool enabledTownHall = false;

    protected override void SetNextQuest()
    {
        //this needs to be empty as quests are set in this script
    }

    protected override void Start()
    {
        PM = FindObjectOfType<PlayerManager>();
        dialogueTrigger = GetComponent<DialogueTrigger>();
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
        if (QuestNumberDone >= 5 && !enabledTownHall)
        {
            TownhallButton.interactable = true;
            enabledTownHall = true;
        }

        if(playerInRange && Input.GetKeyDown(KeyCode.Escape) && pauseMenu.isPausedMenu == false && pauseMenu.isPausedInventory == false && playerInUI == true)
        {
            closeUI();
        }
    }
    public void ActiveUI()
    {
        donaldCam.enabled = true;
        DonaldUIActive = true;
        HouseQuestUI.SetActive(true);
        pauseMenu.playerIsInUI = true;
        
        Debug.Log("Activated UI Donald");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerManager.RestricPlayer();
    }

    public void DaisyHouseQ()
    {
        closeUI();
        currentQuest = houseQDaisy;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
        Debug.Log("DaisyQ activated");
    }

    public void MaryHouseQ()
    {
        closeUI();
        currentQuest = houseQMary;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
        Debug.Log("MaryQ activated");
    }

    public void DonaldHouseQ()
    {
        closeUI();
        currentQuest = houseQDonald;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
        Debug.Log("DonaldQ activated");
    }

    public void CharlesHouseQ()
    {
        closeUI();
        currentQuest = houseQCharles;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
        Debug.Log("CharlesQ activated");

    }

    public void FrederickHouseQ()
    {
        closeUI();
        currentQuest = houseQFrederick;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
        Debug.Log("FrederickQ activated");

    }

    public void KingHouseQ() //Last quest for winning game
    {
        closeUI();
        currentQuest = houseQTownhall;
        dialogueTrigger.TriggerDialogue2(dialogueTrigger.dialogueTracker);
    }

    protected override void CompleteQuest()
    {
        base.CompleteQuest();
        if(currentQuest == houseQDaisy)
        {
            DaisyButton.interactable = false;
            tickDaisy.SetActive(true);
            QuestNumberDone++;
            EnableThankYouDialogue("Daisy");
        }
        if (currentQuest == houseQDonald)
        {
            DonaldButton.interactable = false;
            tickDonald.SetActive(true);
            QuestNumberDone++;
            EnableThankYouDialogue("Donald");
        }
        if (currentQuest == houseQMary)
        {
            MaryButton.interactable = false;
            tickMary.SetActive(true);
            QuestNumberDone++;
            EnableThankYouDialogue("Mary");
            EnableThankYouDialogue("Susan");
        }
        if (currentQuest == houseQFrederick)
        {     
            FrederickButton.interactable = false;
            tickFrederick.SetActive(true);
            QuestNumberDone++;
            EnableThankYouDialogue("Frederick");
        }
        if (currentQuest == houseQTownhall)
        {
            TownhallButton.interactable = false;
            tickTownhall.SetActive(true);
            VillageComplete = true;

            //EnableThankYouDialogue("King");
        }
        if (currentQuest == houseQCharles)
        {
            CharlesButton.interactable = false;
            tickCharles.SetActive(true);
            QuestNumberDone++;
            EnableThankYouDialogue("Charles");
        }
    }

    private void EnableThankYouDialogue(string charName)
    {
        DialogueTrigger[] dialogues = FindObjectsOfType<DialogueTrigger>();

        foreach (DialogueTrigger penguindialogue in dialogues) {
            if (penguindialogue.dialogue.charName[0].Contains(charName)) {
                penguindialogue.shouldThankForHouse = true;

            }
        }
    }

    public void Cheat_FinishAllHouses()
    {
        foreach (var item in progressManager.SideLevelProgression) {
            item.levelPropsAppear[0].SetActive(true);
        }
        QuestNumberDone = 6;
        VillageComplete = true;
    }


    public void closeUIMouseClick()
    {
        dialogueTrigger.dialogueTracker = 0;
        closeUI();
    }

    public void closeUI()
    {
        donaldCam.enabled = false;
        DonaldUIActive = false;
        Debug.Log("Donald close UI");

        pauseMenu.playerIsInUI = false;
        HouseQuestUI.SetActive(false);
        playerManager.UnRestrictPlayer();

        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false ;
    }       
}
