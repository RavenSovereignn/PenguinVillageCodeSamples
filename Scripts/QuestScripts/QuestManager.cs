using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using JetBrains.Annotations;
using UnityEngine.UI;
using System;

public class QuestManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    private PlayerManager playerManager;
    public QuestIconUI questIconUI;
    public ProgressManager levelProgression;
    public ItemSystem ItemSystem;

    [Header("Quest UI")]
    public GameObject questWindow;
    public GameObject questHintPopup;
    public PauseMenu pauseMenu;

    public GameObject questIcon;
    public GameObject requiredItemPrefab;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI acceptText;
    public TextMeshProUGUI itemrewardText;
    public TextMeshProUGUI coinrewardText;
    public Transform requiredItemListContent;

    public GameObject acceptButton;
    public GameObject declineButton;

    public static Action AcceptButtonClickedEvent;
    public static Action DeclineButtonClickedEvent;

    public quest2 q2;

    public delegate void DisableUICallBack();

    public bool UIAtive;
    private bool SpokeToKing = false;

    public enum QuestUIStatus { UnAccepted, CanComplete, InProgress}

    private string acceptQuest = "E: Accept";
    private string finishQuset = "E: Complete";

    private int iconIDCounter = 0;

    void Start()
    {
        playerManager = Player.GetComponent<PlayerManager>();
        pauseMenu = GetComponent<PauseMenu>();

        QuestIconInfo meetKingQuest = new QuestIconInfo("Speak to King Penguin", new List<int>() { }, new List<int>() { }, new List<int>() { });
        SetQuestIcon(meetKingQuest);
        
    }

    private void Update()
    {
    }



    public void AcceptButtonClick()
    {
        AcceptButtonClickedEvent.Invoke();
    }
    public void DeclineButtonClick()
    {
        DeclineButtonClickedEvent.Invoke();
    }

    public bool CheckLevelProgression(bool mainLevel, int sideLevel, Action cinematicFinsihedCallBack)
    {
        //Debug.Log($"side level: {sideLevel},");

        //progresses the main level
        if (mainLevel) {
            levelProgression.ProgressMainLevel(cinematicFinsihedCallBack);
            return true;
        }

        //progresses the given side level
        if(sideLevel > 0) {
            levelProgression.ProgressSideLevel(sideLevel - 1, cinematicFinsihedCallBack);
            return true;
        }
        return false;
    }

    public DisableUICallBack SetQuestUI(quest2 quest, QuestUIStatus questStatus)
    {
        //enable cursor
        pauseMenu.playerIsInUI = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0;
        questWindow.SetActive(true);
        
        titleText.text = quest.QuestTitle;
        descriptionText.text = quest.QuestDescription;
        //itemrewardText.text = item;
        coinrewardText.text = quest.CoinReward.ToString();

        if(questStatus == QuestUIStatus.UnAccepted) {
            acceptButton.SetActive(true);
            declineButton.SetActive(true);
            acceptText.text = acceptQuest;
            acceptText.enabled = true;
        }
        else if(questStatus == QuestUIStatus.CanComplete) {
            acceptButton.SetActive(true);
            declineButton.SetActive(true);
            acceptText.text = finishQuset;
            acceptText.enabled = true;
        }
        else if(questStatus == QuestUIStatus.InProgress){
            acceptButton.SetActive(false);
            declineButton.SetActive(false);
            acceptText.enabled = false;
        }

        foreach (Transform item in requiredItemListContent) {
            Destroy(item.gameObject);
        }

        foreach (Goal questGoal in quest.goals) {
            int itemID = questGoal.itemId;
            int itemCount = questGoal.count;

            GameObject uiComponent = Instantiate(requiredItemPrefab, requiredItemListContent);

            uiComponent.GetComponentInChildren<UnityEngine.UI.Image>().sprite = ItemSystem.GetIcon(itemID);
            uiComponent.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = itemCount.ToString();
        }

        UIAtive = true;

        return DisableUI;
    }

    public void DisableUI()
    {
        Time.timeScale = 1;
        pauseMenu.playerIsInUI = (Input.GetKey(KeyCode.Escape))? true : false;
        questWindow.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        titleText.text = string.Empty;
        descriptionText.text = string.Empty;
        coinrewardText.text = string.Empty;
        UIAtive = false;
        //itemrewardText.text = string.Empty;
    }

    public int SetQuestIcon(QuestIconInfo info)
    {
        questIconUI.AddQuest(info);
        info.QuestIconID = ++iconIDCounter;

        return iconIDCounter;
    }

    public void RemoveQuestIcon(int id)
    {
        questIconUI.RemoveQuest(id);
        return;
    }

    public void UpdateQuestIcon(int id, QuestIconInfo info)
    {
        info.QuestIconID = id;

        List<QuestIconInfo> currentQuests = questIconUI.QuestIcons;

        currentQuests.RemoveAll(x => x.QuestIconID == id);

        questIconUI.QuestIcons = currentQuests;
        questIconUI.AddQuest(info);

        //Debug.Log(questIconUI.QuestIcons.Count);
    }

    //finishes first quest
    public void SpokeToTheKing()
    {
        if (!SpokeToKing) {
            RemoveQuestIcon(1);
        }
    }

}

[System.Serializable]
public class quest2 {
    public string QuestTitle;

    [TextArea(5, 5)]
    public string QuestDescription;

    //when player can't complete the quest this shows eg. "Catch 5 salmon"
    public string UnfinishedQuestHint;

    public int CoinReward;

    [NonReorderable]
    public List<Goal> goals;

    [Tooltip("Progress the main level")]
    public bool ProgressMainLevel = false;

    [Tooltip("Id of the side progression which should be activated on quest completion. -1 for none")]
    public int ProgressSideLevel = -1;
}

public class QuestIconInfo {
    public int QuestIconID;

    public string Name;
    public List<int> goalsItemIds;
    public List<int> goalsItemProgress;
    public List<int> goalsItemMax;

    public QuestIconInfo(string name, List<int> _goalsItemIds, List<int> _goalsItemProgress, List<int> _goalsItemMax)
    {
        Name = name;
        goalsItemIds = _goalsItemIds;
        goalsItemProgress = _goalsItemProgress;
        goalsItemMax = _goalsItemMax;
    }
}


[System.Serializable]
public struct Goal {
    public GoalType goaltpye;
    public FishType fishType;
    public int itemId;
    public int count;
}

