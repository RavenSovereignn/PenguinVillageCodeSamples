using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EndGameCinematic : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public Cinemachine.CinemachineVirtualCamera cam;

    public Transform LookAtPostion;
    public Transform lookAtStart;
    public Transform lookAtend;

    [Header("UI References")]
    public Image background;
    public Image creditsBackground;

    public Image Logo;
    public RectTransform creditsParents;

    public Button exitButton;
    public Button continueButton;

    private Action cachedCamSwitchCallBack;
    CinemachineTrackedDolly dolly;

    public float cinematicduration = 5.0f;
    private float progress = 0.0f;
    [HideInInspector] public bool cinematicActive = false;
    private bool startCinematic = false; 
    private bool uiStarted = false;

    [HideInInspector] public bool endingPlayed = false;

    private void Start()
    {
        dolly = cam.GetCinemachineComponent<CinemachineTrackedDolly>();
    }


    public void BeginEndOfGame()
    {
        cam.enabled = true;
        cachedCamSwitchCallBack = playerManager.SetActiveCamera(cam);
        startCinematic = true;
        cinematicActive = true;
    }

    private void Update()
    {
        if (startCinematic) {
            progress += Time.deltaTime * (1 / cinematicduration) * (Input.GetKey(KeyCode.Space) ? 3.0f : 1.0f);
            dolly.m_PathPosition = progress;
            LookAtPostion.position = Vector3.Lerp(lookAtStart.position, lookAtend.position, progress);
        }
        if (progress >= 0.65f && !uiStarted) {
            StartCoroutine(StartUISequence());
            uiStarted = true;   
        }

    }

    private IEnumerator StartUISequence()
    {
        background.enabled = true;
        Logo.enabled = true;

        WaitForSeconds delay = new WaitForSeconds(0.01f);

        Color colour = new Color(1, 1, 1, 0);

        for (int i = 0; i < 100; i++) {
            colour.a += 0.01f;

            background.color = colour;
            Logo.color = colour;

            yield return delay;
        }

        yield return new WaitForSeconds(0.8f);

        colour = new Color(1, 1, 1, 1);

        for (int i = 0; i < 50; i++) {
            colour.a -= 0.02f;
            Logo.color = colour;

            yield return delay;

        }

        creditsBackground.enabled = true;
        creditsParents.gameObject.SetActive(true);
        TextMeshProUGUI[] credits = creditsParents.GetComponentsInChildren<TextMeshProUGUI>();
        colour = new Color(1, 1, 1, 0);

        for (int i = 0; i < 50; i++) {
            colour.a += 0.02f;

            foreach (TextMeshProUGUI credit in credits) {
                Color col = credit.color;
                col.a = colour.a;

                credit.color = col;
            }

            creditsBackground.color = colour;

            yield return delay;

        }
        exitButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);

        FindObjectOfType<PauseMenu>().playerIsInUI = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void BUTTON_ContinueGame()
    {
        cachedCamSwitchCallBack.Invoke();
        startCinematic = false;
        cinematicActive = false;

        creditsBackground.enabled = false;
        background.enabled = false;
        Logo.enabled = false;
        creditsParents.gameObject.SetActive(false);

        exitButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        FindObjectOfType<PauseMenu>().playerIsInUI = false;
        endingPlayed = true;

        ProgressManager.CurrentLevel = 3;
        ProgressManager.LevelPogressed.Invoke();
    }


}
