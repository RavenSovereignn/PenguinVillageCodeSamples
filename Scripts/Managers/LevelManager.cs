using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;
    public GameObject BubbleParentObj;
    public GameObject fishParentObj;
    public Level1Prefabs L1_Prefabs;
    public LevelBounds L1_Bounds;

    [Header("Chase mini game")]
    public Slider dashSlider;
    public TMPro.TextMeshProUGUI targetMessage;
    public GameObject dashTargetUIPrefab;

    public GameObject blueDashTargetFishUIPrefab;
    public GameObject yellowDashTargetFishUIPrefab;
    public GameObject redDashTargetFishUIPrefab;

    public GameObject hitPopUpPrefab;
    public Image tutorialDashSliderHandle;

    public FishChaseMiniGame blueFishChasing;
    public FishChaseMiniGame yellowFishChasing;
    public FishChaseMiniGame redFishChasing;

    private Dictionary<FishType, int> startingFish;
    private Quaternion fishStartRot;

    private int startingBubbles = 10;

    void Start()
    {
        startingFish = GetCountOfActiveFish();
        startingBubbles = CountBubblesInScene();

        Penguin3DController.PlayerEnteredWater += PlayerWaterState;

    }

    private void OnDestroy()
    {
        Penguin3DController.PlayerEnteredWater -= PlayerWaterState;

    }


    //listens to when player enters and exits water
    private void PlayerWaterState(bool state)
    {
        if(state == false) {
            Debug.Log("spawning level");
            RespawnLevel();
        }
    }

    private void FishSetup(ref Fish3D _fish)
    {
        _fish.dashTargetUIPrefab = dashTargetUIPrefab;
        _fish.hitPopUpPrefab = hitPopUpPrefab;
        _fish.dashSlider = dashSlider;
        _fish.tutorialDashSliderHandle = tutorialDashSliderHandle;

        //set the fish target prefab, used as the last dash target 
        _fish.dashTargetFishUIPrefab = (_fish.fishType == FishType.Salmon) ? blueDashTargetFishUIPrefab : (_fish.fishType == FishType.Cod) ? yellowDashTargetFishUIPrefab : redDashTargetFishUIPrefab;
    }

    private Dictionary<FishType, int> GetCountOfActiveFish()
    {
        Dictionary<FishType, int> fishDict = new Dictionary<FishType, int>
        {
            { FishType.Cod, 0 },
            { FishType.Salmon, 0 },
            { FishType.Snapper, 0 }
        };

        Fish3D[] activeFish = fishParentObj.GetComponentsInChildren<Fish3D>();

        for (int i = 0; i < activeFish.Length; i++) {

            Fish3D curFish = activeFish[i];
            fishDict[curFish.fishType]++;
            fishStartRot = curFish.transform.rotation;

            FishSetup(ref curFish);
        }

        return fishDict;
        //Debug.Log($"cod: {fishDict[FishType.Cod]}, snapper: {fishDict[FishType.Snapper]}, salmon: {fishDict[FishType.Salmon]} ");
    }


    private int CountBubblesInScene()
    {
        BubbleFloat[] bubbles = BubbleParentObj.GetComponentsInChildren<BubbleFloat>();
        return bubbles.Length;
    }

    private void RespawnLevel()
    {
        Dictionary<FishType, int> currentFish = GetCountOfActiveFish();

        foreach (var fish in currentFish) {
            Debug.Log($"{fish.Key}, {fish.Value}, {startingFish[FishType.Cod] - currentFish[FishType.Cod]}");
        }

        SpawnFish(L1_Prefabs.Cod, L1_Bounds, startingFish[FishType.Cod] - currentFish[FishType.Cod]);
        SpawnFish(L1_Prefabs.Snapper, L1_Bounds, startingFish[FishType.Snapper] - currentFish[FishType.Snapper]);
        SpawnFish(L1_Prefabs.Salmon, L1_Bounds, startingFish[FishType.Salmon] - currentFish[FishType.Salmon]);

        SpawnItems(L1_Prefabs.Bubble, L1_Bounds, startingBubbles - CountBubblesInScene());
    }

    private void SpawnFish(GameObject obj, LevelBounds bounds, int count)
    {
        for (int i = 0; i < count; i++) {
            Vector3 pos = bounds.RandomPointInBounds();

            GameObject fishy = Instantiate(obj, pos, fishStartRot);
            fishy.transform.parent = fishParentObj.transform;
            Fish3D fishScript = fishy.GetComponent<Fish3D>();

            fishScript.Player = Player;
            fishScript.BoundingTopLeftFront = L1_Bounds.TopLeftFront;
            fishScript.BoundingBottomRightBack = L1_Bounds.BotRightBack;
        }
    }

    private void SpawnItems(GameObject obj, LevelBounds bounds, int count)
    {
        for (int i = 0; i < count; i++) {
            Vector3 pos = bounds.RandomPointInBounds();

            GameObject item = Instantiate(obj, pos, Quaternion.identity);
            item.transform.parent = BubbleParentObj.transform;
        }

    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha3) && Input.GetKey(KeyCode.Z)) {
            RespawnLevel();
        }

    }

    public FishChaseMiniGame GetChaseSettings(FishType fishType)
    {
        if (fishType == FishType.Salmon) {
            return blueFishChasing;
        }
        else if (fishType == FishType.Cod) {
            return yellowFishChasing;
        }
        else {
            return redFishChasing;
        }
    }


}

[System.Serializable]
public struct Level1Prefabs {
    public GameObject Cod;
    public GameObject Salmon;
    public GameObject Snapper;

    public GameObject Bubble;
    public GameObject BlueSeaWeed;
    public GameObject GreenSeaWeed;

}

[System.Serializable]
public struct LevelBounds {
    public Transform TopLeftFront;
    public Transform BotRightBack;

    public Vector3 RandomPointInBounds() 
    {
        float x = Random.Range(BotRightBack.position.x, TopLeftFront.position.x);
        float y = Random.Range(TopLeftFront.position.y, BotRightBack.position.y);
        float z = Random.Range(BotRightBack.position.z, TopLeftFront.position.z);

        return new Vector3(x, y, z); ;
    }

}

