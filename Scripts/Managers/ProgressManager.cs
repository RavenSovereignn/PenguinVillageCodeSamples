using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using UnityEngine.Events;

public class ProgressManager : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public Cinemachine.CinemachineBrain camBrain;
    private Cinemachine.CinemachineVirtualCamera cinematicCamera;
    public List<Cinemachine.CinemachineVirtualCamera> cinematicCameras;

    public ConstructionFade fading;
    public AudioManager audioManager;

    [Header("Cinematic")]
    public float camBlendTime = 2.0f;
    public float buildTime = 1.5f;
    public float fadeToBlackTime = 1.0f;

    [Header("Progression")]
    [NonReorderable]
    //main level states are upgrades from the main quests, must be set in order
    public List<LevelUpgradeInfo> MainLevelProgression;
    [NonReorderable]
    //side level states are independent from the main states, can be set in any order
    public List<LevelUpgradeInfo> SideLevelProgression;
    [NonReorderable]
    //if any level changes need to call a function, this maps a level change id to function 
    public Dictionary<int, Action> LevelProgressionBehaviour;

    public static bool InCinemtatic = false;

    private static int levelProgressionTracker; 

    public static int CurrentLevel { get { return levelProgressionTracker; } set { levelProgressionTracker = value; } }

    public static UnityAction LevelPogressed;

    Action currentCinematicFinishCallback;


    void Start()
    {

        audioManager = FindObjectOfType<AudioManager>();
        //first level
        levelProgressionTracker = 0; 

        //set level info ids
        for (int i = 0; i < MainLevelProgression.Count; i++) {
            MainLevelProgression[i].levelId = i;
        }

        for (int i = 0; i < SideLevelProgression.Count; i++) {
            SideLevelProgression[i].levelId = i;
        }

        LevelProgressionBehaviour = new Dictionary<int, Action>();
        LevelProgressionBehaviour.Add(2, TestingBehaviour);

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            ProgressMainLevel(null);
        }
    }

    public void ProgressMainLevel(Action cinematicfinihsedCallback)
    {
        currentCinematicFinishCallback = cinematicfinihsedCallback;

        //move to next level
        levelProgressionTracker++;

        //already on last level
        if(levelProgressionTracker >= MainLevelProgression.Count) {
            levelProgressionTracker--;
            return;
        }

        //get level progression info
        LevelUpgradeInfo levelInfo = MainLevelProgression[levelProgressionTracker];

        StartCoroutine(CinematicBuild(BuildProgression, levelInfo));
    }

    public void ProgressSideLevel(int levelId, Action cinematicfinihsedCallback)
    {
        currentCinematicFinishCallback = cinematicfinihsedCallback;

        //get level progression info
        LevelUpgradeInfo levelInfo = SideLevelProgression[levelId];

        IEnumerator coroutine = CinematicBuild(BuildProgression, levelInfo);

        StartCoroutine(coroutine);

    }

    private void BuildProgression(LevelUpgradeInfo levelInfo)
    {
        //activate, enables and disables objs
        levelInfo.Activate();

        //if the level needs to call an extra function 
        if (LevelProgressionBehaviour.ContainsKey(levelInfo.levelId)) {
            LevelProgressionBehaviour[levelInfo.levelId]?.Invoke();
        }

        Debug.Log("built");
    }

    private IEnumerator CinematicBuild(Action<LevelUpgradeInfo> buildFunction, LevelUpgradeInfo levelInfo)
    {
        InCinemtatic = true;

        float defaultBlendTime = camBrain.m_DefaultBlend.m_Time;
        camBrain.m_DefaultBlend.m_Time = camBlendTime;

        //choose the right cinematic camera
        if(levelInfo.camIndex > 0) {
            cinematicCamera = cinematicCameras[levelInfo.camIndex];
        }
        else {
            cinematicCamera = cinematicCameras[0];
        }

        //cinematicCamera.LookAt = levelInfo.CameraLookPosition();

        //restrict player
        playerManager.RestricPlayer();

        //change camera, wait until changed before continuing
        playerManager.playerCamera.enabled = false;
        cinematicCamera.enabled = true;
        yield return new WaitForSeconds(camBlendTime);

        ////fade in 
        //fading.FadeInUI(true);

        ////play sound
        //audioManager.Play("UpgradeHouseSound");

        ////check each frame if finshed fading
        WaitForEndOfFrame endOfFrameDelay = new WaitForEndOfFrame();
        //while (fading.fadeIn) {
        //    yield return endOfFrameDelay;
        //}

        //build level 
        buildFunction.Invoke(levelInfo);

        //wait on black screen
        //yield return new WaitForSeconds(buildTime);

        //fade out 
        //fading.FadeOutUI();

        ////check each frame if finshed fading
        //while (fading.fadeOut) {
        //    yield return endOfFrameDelay;
        //}

        //slightly animate the building size
        List<GameObject> addedBuildings = levelInfo.levelPropsAppear;
        List<Vector3> scalingVecs = new List<Vector3>();

        //calculate the scaling factors
        foreach (GameObject building in addedBuildings) {
            scalingVecs.Add(building.transform.localScale / 60.0f);
        }

        //because low frame rate of rendering entire island, just update scale each frame 
        for (int i = 0; i < 20; i++) {
            for (int j = 0; j < addedBuildings.Count; j++) {
                addedBuildings[j].transform.localScale += scalingVecs[j];
            }
            yield return endOfFrameDelay;
        }

        for (int i = 0; i < 20; i++) {
            for (int j = 0; j < addedBuildings.Count; j++) {
                addedBuildings[j].transform.localScale -= scalingVecs[j];
            }
            yield return endOfFrameDelay;
        }


        //switch cam and wait to changed
        cinematicCamera.enabled = false;
        playerManager.playerCamera.enabled = true;
        yield return new WaitForSeconds(camBlendTime);

        //put blend time back to normal
        camBrain.m_DefaultBlend.m_Time = defaultBlendTime;

        playerManager.UnRestrictPlayer();   
        
        if(currentCinematicFinishCallback != null) { currentCinematicFinishCallback.Invoke(); }

        InCinemtatic = false;
        LevelPogressed?.Invoke();
    }

    private void TestingBehaviour()
    {
        Debug.Log("Frendrick jr.");

    }


}


[System.Serializable]
public class LevelUpgradeInfo {
    public string Info;

    public int camIndex = 0;

    [HideInInspector]
    public int levelId;

    public List<GameObject> levelPropsAppear;
    public List<GameObject> levelPropsDisappear;


    public void Activate()
    {
        for (int i = 0; i < levelPropsAppear.Count; i++) {
            levelPropsAppear[i].SetActive(true);
        }

        for (int i = 0; i < levelPropsDisappear.Count; i++) {
            levelPropsDisappear[i].SetActive(false);
        }
    }   
    
    public Transform CameraLookPosition()
    {
        GameObject g = new GameObject();

        Vector3 averagePositon = Vector3.zero;

        foreach(var prop in levelPropsAppear) {
            averagePositon += prop.transform.position;
        }

        float count = (float)levelPropsAppear.Count;
        averagePositon = new Vector3(averagePositon.x / count, averagePositon.y / count, averagePositon.z / count);

        g.transform.position = averagePositon;

        return g.transform;
    }

}