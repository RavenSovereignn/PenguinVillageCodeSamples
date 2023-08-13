using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class MarketTrigger : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera MarketCam;
    public CinemachineVirtualCamera PlayerCam;

    [Header("References")]
    public Market marketScript;
    public DialogueTrigger rogersDialogue;

    bool playerInMarket = false;
    bool playerInRange = false;
    PlayerManager cachedPlayerManager;
    Action cachedCamResetCallBack;

    [HideInInspector] public bool inDialogue = false;
    [HideInInspector] public bool ignoreOneFrame = false;



    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && ignoreOneFrame) {
            ignoreOneFrame = false;
            return;
        }

        if (!ProgressManager.InCinemtatic && Input.GetKeyDown(KeyCode.E) && playerInRange && !playerInMarket && !inDialogue) {
            if (!playerInMarket) {
                EnterMarket();
            }
        }
        else if(playerInMarket && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))) {
            ExitMarket();
        }
        
    }

    private void EnterMarket()
    {
        cachedCamResetCallBack = cachedPlayerManager.SetActiveCamera(MarketCam);

        playerInMarket = true;
        marketScript.SetupMarket();

        if (rogersDialogue.smallTalkMark != null) { rogersDialogue.smallTalkMark.SetActive(false); }
    }

    public void ExitMarket()
    {
        cachedCamResetCallBack.Invoke();

        playerInMarket = false;
        marketScript.StopMarket();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player)) {
            player.GetPrompt().text = "E: Enter Shop";
            player.GetPrompt().enabled = true;
            cachedPlayerManager = player;
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player)) {
            playerInRange = false;
            player.GetPrompt().enabled = false;
            cachedPlayerManager = null;
        }
    }

}

