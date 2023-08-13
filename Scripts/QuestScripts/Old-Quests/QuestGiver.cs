using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class QuestGiver : MonoBehaviour
{
    [Header("References")]
    public QuestManager questManager;

    public Quest quest;

    public PlayerManager player;

    private bool playerInRange = false;

    public GameObject questWindow;
    public GameObject questHintPopup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI itemrewardText;
    public TextMeshProUGUI coinrewardText;

    [SerializeField] private bool isPaused;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerInRange == true)
        {
            isPaused = !isPaused;
            
        }
        if (playerInRange == true)
        {
            questHintPopup.SetActive(true);
        }
        if (playerInRange == false)
        {
            questWindow.SetActive(false);
            questHintPopup.SetActive(false);
        }
        if(isPaused)
        {
            GiveQuest();
        }
        
    }
    public void GiveQuest()
    {
            Debug.Log("Player asked for quest");
            Time.timeScale = 0;
            questWindow.SetActive(true);
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            titleText.text = quest.title;
            descriptionText.text = quest.description;
            itemrewardText.text = quest.itemReward;
            coinrewardText.text = quest.coinReward.ToString();
            
    }

    public void AcceptQuest()
    {
        Time.timeScale = 1;
        questWindow.SetActive(false);
        isPaused = false;
        quest.isActive = true;
        //player.quest = quest;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
