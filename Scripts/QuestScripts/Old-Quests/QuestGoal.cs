using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestGoal 
{
   public GoalType goalType;

    public int requiredAmountBF; //BF - blue fish, YF - yellow fish, RF - red fish
    public int requiredAmountYF;
    public int requiredAmountRF;
    public int currentAmountBF;
    public int currentAmountYF;
    public int currentAmountRF;

    public int requiredAmountGSW; // GSW - green seaweed
    public int requiredAmountBSW; // BSW - blue seaweed
    public int currentAmountGSW;
    public int currentAmountBSW;

    public bool IsReached()
    {
        return (currentAmountBF>=requiredAmountBF && requiredAmountYF <= currentAmountYF && requiredAmountRF <= currentAmountRF && requiredAmountGSW <= currentAmountGSW && requiredAmountBSW <= currentAmountBSW);
    }
}
public enum GoalType
{
    GatherFish,
    GatherItems
    //Fishing - wip
}

