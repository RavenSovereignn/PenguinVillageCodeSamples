using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

public class TutorialManager : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public PauseMenu pauseMenu;
    public Canvas UICanvas;
    public TMPro.TextMeshProUGUI tutorialText;


    [Header("Tutorial Stuff")]
    public bool RunTutorial = false;
    private bool tutorialActive = false;
    [NonReorderable]
    public List<TutorialInfo> Tutorials;
    private int currentTutorial = 1;
    private int currentTutorialPart = 0;

    public Image TutorialUI;
    public Image QuestTrackerCover;


    private List<GameObject> ActiveTutorialUI;
    private bool doScaleUI;
    private float currentTracker;
    private Vector3 currentScale;

    private bool waterTutorialCompleted = false;


    [Header("Fish MiniGame tutorial")]
    public UnityEngine.UI.Slider tutorialDashSlider;
    public Image tutorialDashSliderHandle;
    public GameObject hitPopupPrefab;
    public GameObject dashTargetUIPrefab;
    public GameObject dashTargetFishUIPrefab;
    private GameObject dashTargetsParent;
    private List<float> targetTimes;

    private bool spacePressed = false;
    private bool QTETutorialActive = false;


    void Start()
    {
        if (!RunTutorial) {
            return;
        }

        //setup the list of ui elements to scale. runs coroutine to shrink and largen the active ui elements (slight animation) 
        ActiveTutorialUI = new List<GameObject>();
        StartCoroutine(ScaleUI());

        //run the quest tracker tutorial first 
        QuestTrackerTutorial();

        //listen to when the player enters the water and the first time will trigger the tutorial
        Penguin3DController.PlayerEnteredWater += UnderwaterTutorial;

    }

    private void OnDestroy()
    {
        Penguin3DController.PlayerEnteredWater -= UnderwaterTutorial;
    }

    void Update()
    {
        //one slide in the customisation tutorial continues with p
        if (currentTutorial == 3 && currentTutorialPart == 1) {
            if (Input.GetKeyDown(KeyCode.P)) {
                Next();
            }
        }

        //on E go to the next tutorial part
        else if (tutorialActive && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))) {
            Next();
        }

        if(QTETutorialActive && Input.GetKeyDown(KeyCode.Space)) {
            spacePressed = true;
        }
    }

    void Next()
    {
        //move to the next part of the tutorial, ie text about another mechanic and-or  new image highlighting a ui component
        currentTutorialPart++;

        //if in qte tutorial stop it
        if (QTETutorialActive) { QTETutorialActive = false; }

        //if reached the end of this current tutorial
        if(currentTutorialPart >= Tutorials[currentTutorial].MaxParts()) {
            SetTutorialUIActive(false);

            if (WardrobeTutorial.inTutorial) {
                StartCoroutine(DelayedSet());
            }
            return;
        }

        //qte mini game tutorial
        if(currentTutorial == 2 && currentTutorialPart == 5) {
            QTETutorialActive = true;
            StartCoroutine(FishCatchingTutorial());
        }

        //update the text to the next part of the tutorial
        tutorialText.text = Tutorials[currentTutorial].NextTutorialPart(currentTutorialPart) + "\n\nE to continue";
        
        //one slide in the customisation tutorial continues with p
        if(currentTutorial == 3 && currentTutorialPart == 1) {
            tutorialText.text = Tutorials[currentTutorial].NextTutorialPart(currentTutorialPart) + "\n\nP to continue";
        }

        //if the next tutorial-part has an image highlighting ui add it to the scaling ui list
        if (Tutorials[currentTutorial].CoverImages[currentTutorialPart].gameObject.name != "Blank Cover") {
            ActiveTutorialUI.Clear();
            ActiveTutorialUI.Add(Tutorials[currentTutorial].CoverImages[currentTutorialPart].gameObject);
        }

    }

    private IEnumerator DelayedSet()
    {
        yield return new WaitForSeconds(5.0f);
        WardrobeTutorial.inTutorial = false;
    }

    //starting tutorial introducing quests
    private void QuestTrackerTutorial()
    {
        SetTutorialUIActive(true);
        StartTutorial(currentTutorial);
    }

    //tutorial starts under water
    private void UnderwaterTutorial(bool playerEnteredWater)
    {
        //only play the tutorial on entering the water for the first time
        if(!playerEnteredWater || waterTutorialCompleted == true) {
            return;
        }
        StartCoroutine(DelayStart());
        

        //wont start the tutorial again
        waterTutorialCompleted = true;
    }
    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.75f);
        //set trackers
        currentTutorial = 2;

        SetTutorialUIActive(true);
        StartTutorial(currentTutorial);
    }

    //set the starting text to the given tutorial
    private void StartTutorial(int tutorialIndex)
    {
        //reset tutorial part tracker
        currentTutorialPart = 0;

        tutorialText.text = Tutorials[tutorialIndex].NextTutorialPart(0) + "\n\nE to continue";
    }

    public void WardrobeTutorialStart()
    {
        //turn off dash slider
        tutorialDashSlider.transform.parent.gameObject.SetActive(false);

        //set trackers
        currentTutorial = 3;

        SetTutorialUIActive(true);
        StartTutorial(currentTutorial);
    }

    private void SetTutorialUIActive(bool state)
    {
        tutorialActive = state;

        TutorialUI.gameObject.SetActive(state);
        Time.timeScale = (state)? 0 : 1;
        pauseMenu.playerIsInUI = state;
        pauseMenu.tutorialActive = state;
        doScaleUI = state;
    }

    //scales up and down the given ui components to add a bit of animation
    IEnumerator ScaleUI()
    {
        while (true) {
            if (doScaleUI) {
                currentTracker += 0.01f;
                float scale = Mathf.Sin(currentTracker) * 0.03f;
                currentScale = Vector3.one + new Vector3(scale, scale, scale);

                foreach (var UIComp in ActiveTutorialUI) {
                    UIComp.GetComponent<RectTransform>().localScale = currentScale;
                }
            }
            yield return new WaitForEndOfFrame();
        }    
    }

    private IEnumerator FishCatchingTutorial()
    {
        float prevSpaceTime = 0.0f;

        while (QTETutorialActive) {
            targetTimes = new List<float>() { 0.75f, 1.5f, 2.25f };
            float totalTime = 3.0f;
            float hitTime = 0.3f;

            SetDashTargets(totalTime);

            float chaseTimer = 0.0f;
            WaitForEndOfFrame delay = new WaitForEndOfFrame();

            while (chaseTimer < totalTime) {
                chaseTimer += Time.unscaledDeltaTime;
                tutorialDashSlider.value = chaseTimer / totalTime;

                if (spacePressed && Time.unscaledTime - prevSpaceTime >= 0.2f && targetTimes.Count > 0) {
                    //scale little penguin 
                    StartCoroutine(SpacePressedFeedback());

                    bool hitTarget = chaseTimer > targetTimes[0] - hitTime && chaseTimer < targetTimes[0] + hitTime;

                    if (hitTarget) {
                        FindObjectOfType<AudioManager>().Play("MinigameHit");
                        HitFeedback(true);
                        if (targetTimes.Count > 0) { targetTimes.RemoveAt(0); }
                    }
                    else {
                        FindObjectOfType<AudioManager>().Play("MinigameMiss");
                        HitFeedback(false);
                    }

                    prevSpaceTime = Time.unscaledTime;
                    spacePressed = false;
                }
                yield return delay;

            }

            yield return null;
        }
    }

    IEnumerator SpacePressedFeedback()
    {
        RectTransform littlePenguinRect = tutorialDashSliderHandle.GetComponent<RectTransform>();

        for (int i = 0; i < 10; i++) {
            littlePenguinRect.localScale += new Vector3(0.02f, 0.02f, 0.02f);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 20; i++) {
            littlePenguinRect.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            yield return new WaitForEndOfFrame();
        }
    }

    private void HitFeedback(bool hit)
    {
        GameObject g = Instantiate(hitPopupPrefab, tutorialDashSlider.transform);
        g.GetComponent<RectTransform>().localPosition = new Vector3(0, 100, 0);
        g.GetComponent<QTEHitPopUp>().Setup(hit);
    }

    private void SetDashTargets(float chaseTime)
    {
        dashTargetsParent = Instantiate(dashTargetUIPrefab, tutorialDashSlider.transform);
        dashTargetsParent.transform.SetSiblingIndex(2);
        dashTargetsParent.GetComponent<Image>().enabled = false;
        dashTargetsParent.GetComponent<RectTransform>().localPosition = Vector3.zero;

        foreach (float dashTime in targetTimes) {
            bool lastTarget = targetTimes.IndexOf(dashTime) + 1 == targetTimes.Count;

            //create a dash target on the slider in the position
            //GameObject g = Instantiate((lastTarget) ? dashTargetFishUIPrefab : dashTargetUIPrefab, dashTargetsParent.transform);
            GameObject g = Instantiate(dashTargetUIPrefab, dashTargetsParent.transform);
            g.GetComponent<RectTransform>().localPosition = SliderCoords(dashTime / chaseTime);
        }
    }
    private Vector3 SliderCoords(float percent)
    {
        Vector3 pos = new Vector3();

        RectTransform sliderRect = tutorialDashSlider.GetComponent<RectTransform>();
        pos = sliderRect.localPosition + Vector3.left * (sliderRect.rect.width / 2.0f);
        pos.x += percent * sliderRect.rect.width;
        pos.y = 9.5f;

        return pos;
    }

}

[System.Serializable]
public struct TutorialInfo {
    public List<string> Sentences;
    public List<Image> CoverImages;

    public string NextTutorialPart(int index)
    {
        //enable the correct image
        foreach (var image in CoverImages) {
            image.gameObject.SetActive(false);
        }
        //set the cover active and return the prompt
        CoverImages[index].gameObject.SetActive(true);
        return Sentences[index];
    }

    public int MaxParts()
    {
        return Sentences.Count;
    }

}