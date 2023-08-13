using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RandomQuests : BaseQusetGiver 
{
    private enum Difficulty { Easy, Medium, Hard};
    private const int ItemCount = 10;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

    }

    public override void EnableQuest(Action<QuestReturnState> callback)
    {
        base.EnableQuest(callback);

        //set a new random quest as the current quest 
        currentQuest = RandomQuest(Difficulty.Easy);
    }

    protected override void CompleteQuest()
    {
        base.CompleteQuest();
        //GetComponent<DialogueTrigger>().dialogueTracker = 0;
    }

    protected override void SetNextQuest()
    {
        //need this blank as own quest iterating behaviour added
    }


    private quest2 RandomQuest(Difficulty difficulty)
    {
        quest2 q = new quest2();

        //quest description
        q.QuestTitle = "Charles' Exquisite Quest";
        q.QuestDescription = "With unwavering precision, you are called upon to gather a unique assortment of items.";

        //quest reward
        q.CoinReward = 500;

        //make sure quest doesn't progress the level
        q.ProgressMainLevel = false;
        q.ProgressSideLevel = -1;

        q.goals = EasyRandomGoals();

        return q;
    }

    private List<Goal> EasyRandomGoals()
    {
        List<Goal> goalList = new List<Goal>();

        //add fish
        Goal fishGoal = new Goal();
        fishGoal.itemId = UnityEngine.Random.Range(1 , 4);
        fishGoal.count = 5;
        goalList.Add(fishGoal);

        //add item
        Goal itemGoal = new Goal();
        itemGoal.itemId = UnityEngine.Random.Range(4, ItemCount);
        itemGoal.count = 2;
        goalList.Add(itemGoal);
        
        return goalList;
    }

    private List<Goal> MediumRandomGoals()
    {
        List<Goal> goalList = new List<Goal>();

        //implement

        return goalList;
    }

    private List<Goal> HardRandomGoals()
    {
        List<Goal> goalList = new List<Goal>();

        //implement

        return goalList;
    }


}


