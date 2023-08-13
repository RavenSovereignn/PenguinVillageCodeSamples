using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WardrobeTrigger : MonoBehaviour
{

    [Header("Cameras")]
    public CinemachineVirtualCamera WardrobeCam;
    public CinemachineVirtualCamera PlayerCam;

    [Header("References")]
    public Wardrobe WardrobeScript;
    public ConstructionFade fading;
    public Transform playerStandPosition;
    public Transform designPenguinBody;
    public Transform camLookPositon;

    bool playerInWardrobe = false;
    bool playerInRange = false;
    PlayerManager cachedPlayerManager;
    Action cachedCamResetCallBack;

    bool coroutineRunning = false;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P) && !playerInWardrobe && !coroutineRunning) {
            Button_EnterWardrobe();
        }
        else if (Input.GetKeyDown(KeyCode.E) && playerInRange && !playerInWardrobe && !coroutineRunning) {
            if (!playerInWardrobe) {
                EnterWardrobe();
            }
        }
        else if (playerInWardrobe && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) && !coroutineRunning) {
            ExitWardrobe();
        }

        if (playerInWardrobe) {
            designPenguinBody.Rotate(Vector3.up, Input.GetAxis("Horizontal") * Time.deltaTime * 80.0f);
        }

    }

    public void Button_EnterWardrobe()
    {
        if (!playerInWardrobe) {
            cachedPlayerManager = FindObjectOfType<PlayerManager>();
            FindObjectOfType<PauseMenu>().DeactivateInventoryMenu();
            EnterWardrobe();
        }
    }

    private IEnumerator EnterCinematic()
    {
        coroutineRunning = true;

        cachedPlayerManager.StopPlayerAudio();
        cachedPlayerManager.gameObject.SetActive(false);

        //fade screen to black, so can teleport player model out of view
        fading.FadeInUI();

        //check each frame if finshed fading
        WaitForEndOfFrame endOfFrameDelay = new WaitForEndOfFrame();
        while (fading.fadeIn) {
            yield return endOfFrameDelay;
        }

        designPenguinBody.gameObject.SetActive(true);
        cachedCamResetCallBack = cachedPlayerManager.SetActiveCamera(WardrobeCam);

        //check each frame if finshed fading
        fading.FadeOutUI();
        while (fading.fadeOut) {
            yield return endOfFrameDelay;
        }

        playerInWardrobe = true;
        WardrobeScript.SetupWardrobe();

        coroutineRunning = false;
    }


    private IEnumerator ExitCinematic()
    {
        coroutineRunning = true;

        designPenguinBody.gameObject.SetActive(false);
        playerInWardrobe = false;
        WardrobeScript.StopWardrobe();

        fading.FadeInUI();

        //check each frame if finshed fading
        WaitForEndOfFrame endOfFrameDelay = new WaitForEndOfFrame();
        while (fading.fadeIn) {
            yield return endOfFrameDelay;
        }

        cachedPlayerManager.gameObject.SetActive(true);
        //cachedPlayerManager.GetComponentInChildren<ParticleSystem>().Stop();
        cachedCamResetCallBack.Invoke();

        //check each frame if finshed fading
        fading.FadeOutUI();
        while (fading.fadeOut) {
            yield return endOfFrameDelay;
        }

        coroutineRunning = false;
    }

    private void EnterWardrobe()
    {        
        StartCoroutine(EnterCinematic());
        cachedPlayerManager.GetPrompt().enabled = false;
    }

    public void ExitWardrobe()
    {
        StartCoroutine(ExitCinematic());      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerManager>(out PlayerManager player)) {
            player.GetPrompt().text = "E: Enter Player Wardrobe";
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
