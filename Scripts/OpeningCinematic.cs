using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class OpeningCinematic : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    CinemachineTrackedDolly dolly;

    public Transform LookAtPostion;
    public Transform LookAtPostionfinal;

    private Vector3 lookAtStart;

    public float travelTime;

    //tracks progress 0-1
    public float progress;

    [HideInInspector]
    public bool startMoving = false;
    public bool finished = false;

    private SkipStartCutscene skipCutscene;

    void Start()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        dolly = vCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        lookAtStart = LookAtPostion.position;
        skipCutscene = FindObjectOfType<SkipStartCutscene>();   
    }

    void Update()
    {
        if (startMoving) {
            progress += Time.deltaTime * (1 / travelTime) * (Input.GetKey(KeyCode.Space) ? 3.0f : 1.0f);
            dolly.m_PathPosition = progress;
            LookAtPostion.position = Vector3.Lerp(lookAtStart, LookAtPostionfinal.position, progress);
            skipCutscene.phase2 = true;
        }

        if(progress > 1.1f && !finished) {
            FindObjectOfType<OpeningDialogue>().TriggerDialogue(0);
            finished = true;
        }
    }



}
