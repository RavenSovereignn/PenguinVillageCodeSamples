using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAIManager : MonoBehaviour
{
    [Header("Spawning")]
    public GameObject fishPrefab;
    public int spawnCount = 100;
    public LevelBounds spawnBounds;


    public FishStats fishStats;



    private List<FishAI> allFishes;



    void Awake()
    {
        SpawnFish();

        allFishes = new List<FishAI>(Object.FindObjectsOfType<FishAI>());
        
        foreach(FishAI fish in allFishes) {
            fish.allFishes = allFishes;
        }

    }

    void Update()
    {
        foreach (FishAI fish in allFishes) {
            fish.FishUpdate(Time.deltaTime);
        }

    }

    private void SpawnFish()
    {
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = spawnBounds.RandomPointInBounds();

            GameObject fishy = Instantiate(fishPrefab, pos, Quaternion.identity);

            FishAI fishAI = fishy.GetComponent<FishAI>();
            fishAI.maxSpeed = fishStats.maxSpeed;
            fishAI.maxForce = fishStats.maxForce;

            fishAI.localDist = fishStats.localDist;
            fishAI.groupingDist = fishStats.groupingDist;
        }

    }

}

[System.Serializable]
public struct FishStats
{
    public float maxSpeed;
    public float maxForce;

    public float localDist;
    public float groupingDist;

    public float avoidanceStrength;
    public float alignmentStrength;
    public float centeringStrength;
}