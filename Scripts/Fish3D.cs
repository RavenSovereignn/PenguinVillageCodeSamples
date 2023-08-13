using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Fish3D : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public Transform BoundingTopLeftFront;
    public Transform BoundingBottomRightBack;
    public AudioManager audioManager;

    [Header("Fish Stats")]
    public FishType fishType;
    public float cruiseSpeed;
    public float avoidPlayerSpeed;
    public float avoidPlayerDist = 5.0f;

    [Header("Player Chase")]
    [HideInInspector] public Slider dashSlider;
    [HideInInspector] public GameObject dashTargetUIPrefab;
    [HideInInspector] public GameObject dashTargetFishUIPrefab;
    [HideInInspector] public GameObject hitPopUpPrefab;
    [HideInInspector] public Image tutorialDashSliderHandle;
    private GameObject dashTargetsParent;

    private FishChaseMiniGame chaseSettings;

    private float currentChaseDistance;
    private float chaseTimer = 0;
    private List<float> targetTimes;
    private Action<bool> chaseMiniGameEndCallBack;

    private IEnumerator dashSliderCoroutine;

    private bool BeingChased = false;

    //direction vectors
    Vector3 direction;
    Vector3 swimDirection;
    Vector3 toKeepInbounds;
    Vector3 avoidPlayerDir;
 
    private bool avoidingFromPlayer = false;


    //when true fish seek player
    private bool SwimTowardsPlayerCheat = false;


    void Start()
    {
        //Get audio manager
        audioManager = FindObjectOfType<AudioManager>();
        direction = new Vector3(1 - 2 * UnityEngine.Random.value, 1 - 2 * UnityEngine.Random.value, 1 - 2 * UnityEngine.Random.value);
        direction.Normalize();

        swimDirection = direction;

        toKeepInbounds = new Vector3(0, 0);
    }

    void Update()
    { 
        //calculate vectors to keep inside bounds and avoid player, both used to alter the fish's direction
        KeepInBounds();
        AvoidPlayer();
        CalculateSwimDirection();

        if (!BeingChased) {
            transform.Translate(swimDirection * (avoidingFromPlayer ? avoidPlayerSpeed : cruiseSpeed) * Time.deltaTime, Space.World);
        }
        else {
            ChasingSwimming();
        }

        //rotate fish to look in the direction it swims
        RotateFishToSwimDirection();

        //cheat - fish seek player
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha2)) { SwimTowardsPlayerCheat = !SwimTowardsPlayerCheat; }
    }

    //changes the swim direction to keep in bounds and avoid player
    private void CalculateSwimDirection()
    {
        Vector3 wantedDirection = direction;

        //change direction vector if outside of bounds
        if (toKeepInbounds.y != 0) {
            wantedDirection.y = swimDirection.y * Mathf.Sign(swimDirection.y) * toKeepInbounds.y;
        }
        if (toKeepInbounds.x != 0) {
            wantedDirection.x = swimDirection.x * Mathf.Sign(swimDirection.x) * toKeepInbounds.x;
        }
        if (toKeepInbounds.z != 0) {
            wantedDirection.z = swimDirection.z * Mathf.Sign(swimDirection.z) * toKeepInbounds.z;
        }

        if (avoidingFromPlayer) {
            swimDirection = swimDirection = Vector3.Lerp(swimDirection, (avoidPlayerDir * 0.4f) + (wantedDirection * 0.6f), 0.2f);
        }
        else {
            swimDirection = Vector3.Lerp(swimDirection, wantedDirection, 0.6f);
        }

        swimDirection.Normalize();
        direction = swimDirection;
    }

    private void ChasingSwimming()
    {
        float distanceCorrection = 0.2f * (Mathf.Clamp(currentChaseDistance - Vector3.Distance(Player.transform.position, transform.position), -currentChaseDistance, currentChaseDistance) + currentChaseDistance);
        Vector3 swimVec = swimDirection * avoidPlayerSpeed * distanceCorrection * Time.deltaTime;

        Debug.Log($"{currentChaseDistance}, { Vector3.Distance(Player.transform.position, transform.position)}");

        transform.Translate(swimVec, Space.World);
    }

    //calculates direction to avoid player
    private void AvoidPlayer()
    {
        if (Player != null) {
            //if player is within detection range
            Vector3 playerOffset = transform.position - Player.transform.position;
            if (playerOffset.magnitude < avoidPlayerDist * (SwimTowardsPlayerCheat ? 5.0f : 1.0f)) {

                //vector away from the player normalised
                avoidPlayerDir = playerOffset.normalized * (SwimTowardsPlayerCheat ? -1.0f : 1.0f);

                avoidingFromPlayer = true;
            }
            else {
                avoidingFromPlayer = false;
            }
        }
    }

    //rotate to look in swim direction 
    private void RotateFishToSwimDirection()
    {
        transform.LookAt(transform.position + swimDirection);

        transform.Rotate(new Vector3(0, 1, 0), 90, Space.Self);
        transform.Rotate(new Vector3(1, 0, 0), -90, Space.Self);
    }


    //adds the direction to swim away from boundaries to tokeepinbounds vector
    private void KeepInBounds()
    {
        Vector3 pos = transform.position;
        float borderAdjustment = (BeingChased) ? 5.0f : 0.0f;

        //if outisde of X bounds
        if (pos.x > BoundingBottomRightBack.position.x - borderAdjustment) {
            toKeepInbounds.x = -1;
        }
        else if (pos.x < BoundingTopLeftFront.position.x + borderAdjustment) {
            toKeepInbounds.x = 1;
        }

        //if outisde of Y bounds
        if (pos.y > BoundingTopLeftFront.position.y - borderAdjustment) {
            toKeepInbounds.y = -1;
        }
        else if (pos.y < BoundingBottomRightBack.position.y + borderAdjustment) {
            toKeepInbounds.y = 1;
        }  

        //if outisde of Z bounds
        if (pos.z > BoundingTopLeftFront.position.z - borderAdjustment) {
            toKeepInbounds.z = -1;
        }
        else if (pos.z < BoundingBottomRightBack.position.z + borderAdjustment) {
            toKeepInbounds.z = 1;
        }  
    }

    public void StartChase(Action<bool> miniGameEndCallBack)
    {
        chaseMiniGameEndCallBack = miniGameEndCallBack;

        GetComponent<Collider>().enabled = false;
        
        LevelManager levelMan = GameObject.FindObjectOfType<LevelManager>();
        chaseSettings = levelMan.GetChaseSettings(fishType);

        dashSlider.gameObject.SetActive(true);

        targetTimes = chaseSettings.GetDashTimings();
        targetTimes.Sort();

        BeingChased = true;
        currentChaseDistance = 5.0f;

        dashSliderCoroutine = DashMeter(chaseSettings.ChaseTime);
        StartCoroutine(dashSliderCoroutine);

        SetDashTargets();
    }

    public void EndChase()
    {
        dashSlider.gameObject.SetActive(false);

        BeingChased = false;
        targetTimes = null;
        Destroy(dashTargetsParent);

        if(dashSliderCoroutine != null) { StopCoroutine(dashSliderCoroutine); }
    }

    public void Dash()
    {
        StartCoroutine(SpacePressedFeedback());

        float dashTime = chaseTimer;
        float hitTime = chaseSettings.hitTimeDuration / 2.0f;
        bool hitTarget = dashTime > targetTimes[0] - hitTime && dashTime < targetTimes[0] + hitTime;

        HitFeedback(hitTarget);
        //change text to pop up

        //missed the target
        if (!hitTarget) {
            audioManager.Play("MinigameMiss");
            chaseMiniGameEndCallBack.Invoke(false);
            return;
        }

        //hit the target
        audioManager.Play("MinigameHit");
        targetTimes.RemoveAt(0);

        if (targetTimes.Count == 2) {
            currentChaseDistance = 4.0f;
        }
        else if(targetTimes.Count == 1) {
            currentChaseDistance = 2.5f;
        }
        else if (targetTimes.Count == 0) {
            chaseMiniGameEndCallBack.Invoke(true);
        }

    }

    private IEnumerator DashMeter(float totalTime)
    {
        //fill up dash meter 50 times per second
        WaitForSeconds delay = new WaitForSeconds(0.02f);
        int steps = (int)(totalTime * 50);

        //duration each side of the target time
        float hitTime = chaseSettings.hitTimeDuration / 2.0f;

        for (int i = 0; i < steps; i++) {
            chaseTimer += 0.02f;
            dashSlider.value = chaseTimer / totalTime;

            //if missed the maximum possible time to hit the target
            if(chaseTimer > targetTimes[0] + hitTime) {
                audioManager.Play("MinigameMiss");
                HitFeedback(false);
                chaseMiniGameEndCallBack.Invoke(false);
            }

            yield return delay;
        }

        chaseMiniGameEndCallBack.Invoke(false);
    }

    private void HitFeedback(bool hit)
    {
        GameObject g = Instantiate(hitPopUpPrefab, dashSlider.transform);
        g.GetComponent<RectTransform>().localPosition = new Vector3(0,100,0);
        g.transform.parent = dashSlider.transform.parent;
        g.GetComponent<QTEHitPopUp>().Setup(hit);
    }

    private void SetDashTargets()
    {
        dashTargetsParent = Instantiate(dashTargetUIPrefab, dashSlider.transform);
        dashTargetsParent.transform.SetSiblingIndex(2);
        dashTargetsParent.GetComponent<Image>().enabled = false;
        dashTargetsParent.GetComponent<RectTransform>().localPosition = Vector3.zero;

        //create a dash target on the slider in the position for each time
        for (int i = 0; i < targetTimes.Count; i++) {
            float dashTime = targetTimes[i];
            bool lastTarget = i + 1 == targetTimes.Count;

            //set the last target as a fish image, otherwise use regular targets
            //GameObject g = Instantiate((lastTarget) ? dashTargetFishUIPrefab : dashTargetUIPrefab, dashTargetsParent.transform);
            GameObject g = Instantiate(dashTargetUIPrefab, dashTargetsParent.transform);
            g.GetComponent<RectTransform>().localPosition = SliderCoords(dashTime / chaseSettings.ChaseTime);
        }

    }

    //converts slider percent to UI pos
    private Vector3 SliderCoords(float percent)
    {
        Vector3 pos = new Vector3();

        RectTransform sliderRect = dashSlider.GetComponent<RectTransform>();
        pos = sliderRect.localPosition + Vector3.left * (sliderRect.rect.width / 2.0f);
        pos.x += percent * sliderRect.rect.width;
        pos.y = 9.5f;

        return pos;
    }

    IEnumerator SpacePressedFeedback()
    {
        RectTransform littlePenguinRect = tutorialDashSliderHandle.GetComponent<RectTransform>();

        littlePenguinRect.localScale = Vector3.one;

        for (int i = 0; i < 10; i++) {
            littlePenguinRect.localScale += new Vector3(0.02f, 0.02f, 0.02f);
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < 20; i++) {
            littlePenguinRect.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
            yield return new WaitForEndOfFrame();
        }
    }
}

[System.Serializable]
public struct FishChaseMiniGame {
    //total time of the chase
    public float ChaseTime;

    //approximate timings for dashes
    public List<float> DashTimings;

    //how long the target is active
    public float hitTimeDuration;

    public List<float> GetDashTimings()
    {
        List<float> adjustedTimes = new List<float>();

        //add a random offset to each time
        foreach (float curTime in DashTimings) {
            float timeRandom = curTime + UnityEngine.Random.Range(-0.2f, 0.2f);
            timeRandom = Mathf.Clamp(timeRandom, 0.25f, ChaseTime - 0.1f);
            adjustedTimes.Add(timeRandom);
        }
        return adjustedTimes;
    }

}