using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Reflection;
using System;
using UnityEngine.Rendering;
using Cinemachine;

public class PlayerManager : MonoBehaviour
{
    [Header("UI References")]
    public PauseMenu PM;
    public PickupPopUp PopUpManager;
    public TextMeshProUGUI currencyUI;
    public TextMeshProUGUI PlayerPrompt;
    public Animator animator;

    [Header("References")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera playerSwimCamera;
    public UnderwaterBreathin underwaterBreathing;

    [Header("Inventory")]
    public InventoryManager inventoryManager;
    public int PenguinCash;

    [Header("Quest's & Dialogue")]
    [HideInInspector]public bool inDialogue = false;
    [HideInInspector]public bool inQuestScreen = false;
    [HideInInspector]public bool onQuest = false;
    public PickupChick pickupScript;

    private Penguin3DController playerConrtoller;

    private CinemachineVirtualCamera cachedCam;

    private void Start()
    {
        playerConrtoller = gameObject.GetComponent<Penguin3DController>();
    }

    void Update()
    {
        //if(cachedCam == null) {
        //    Debug.Log($"Cached Camera: null");
        //}
        //else {
        //    Debug.Log($"Cached Camera: {cachedCam.transform.parent.name}");
        //}

        if (Input.GetKeyDown(KeyCode.T)) {
            ResetToPlayerCamera();
        }

        currencyUI.text = PenguinCash.ToString();
        
        if(PenguinCash == 1000000)
        {
            PM.YouWinScreen();
        }

        CheatCodes();
    }

    public void RestricPlayer()
    {
        playerConrtoller.MovementRestricted = true;
        //Debug.Log("Player Restricted");
    }

    public void UnRestrictPlayer()
    {
        playerConrtoller.MovementRestricted = false;
    }

    public void StopPlayerAudio()
    {
        playerConrtoller.audioManager.Stop("SnowCrunch");

    }
    public void ToggleUnderWaterBreathMeter(bool state)
    {
        underwaterBreathing.breathingEnabled = state;
    }
    public void PlayIdle()
    {
        animator.SetBool("IsWalk", false);
    }

    //returns dict of fish caught used to check quest completion
    public Dictionary<FishType,int> FishCaught()
    {
        Dictionary<FishType, int> fishCaught = new Dictionary<FishType, int>();

        List<Items> playersInventory = inventoryManager.ItemsList;

        foreach (var item in playersInventory) {
            //if item is a fish
            if(item.itemType == Items.ItemType.Fish) {
                //get the fish type from the id map and add the ammount
                fishCaught.Add(InventoryManager.IdFishMap[item.id], item.amount);
            }
        }

        return fishCaught;
    }

    public bool InventoryContains(int itemId, int count, out int currentAmount)
    {
        List<Items> playersInventory = inventoryManager.ItemsList;

        currentAmount = 0;

        foreach (var item in playersInventory) {
            if(item.id == itemId ) {
                currentAmount = item.amount;

                if(currentAmount >= count) {
                    return true;
                }
            }
        }

        return false;
    }

    private void CheatCodes()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                PenguinCash += 550;
            }
        }

    }

    public bool PlayerHoldingChick()
    {
        return pickupScript.holdingChick;
    }

    public TextMeshProUGUI GetPrompt()
    {
        return PlayerPrompt;
    }

    public Action SetActiveCamera(Cinemachine.CinemachineVirtualCamera cam)
    {
        RestricPlayer();

        cam.enabled = true;
        playerCamera.enabled = false;

        cachedCam = cam;

        return ResetCamera;
    }

    public void ResetCamera()
    {
        UnRestrictPlayer();

        if (cachedCam != null) { cachedCam.enabled = false; }
        playerCamera.enabled = true;
    }

    //reset all cameras and set player's cam active
    public void ResetToPlayerCamera()
    {
        CinemachineVirtualCamera[] vcams = FindObjectsOfType<CinemachineVirtualCamera>();

        foreach (var vcam in vcams) {
            //Debug.Log($"{vcam.transform.parent.name} - {vcam.enabled}");
            vcam.enabled = false;
        }

        UnRestrictPlayer();

        playerSwimCamera.enabled = true;
        playerCamera.enabled = true;

        playerConrtoller.SetWalkCam();
    }

}


