using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaryQuest : BaseQusetGiver
{

    public List<GameObject> sideChicks;

    public List<ChickFollow> lostChicks;
    public PickupChick pickupChicks;

    protected override void Start()
    {
        base.Start();
        foreach (var chick in lostChicks) {
            chick.gameObject.SetActive(false);
        }

        ProgressManager.LevelPogressed += SetupQuest;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ProgressManager.LevelPogressed -= SetupQuest;
    }

    protected override bool InputAccept()
    {
        if (pickupChicks.holdingChick) {
            return false;
        }

        if (playerInUI && acceptButtonHit) { return true; }

        return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1");
    }

    protected override bool CheckCanCompleteQuest()
    {
        bool gotAll = true;
        foreach (var chick in lostChicks) {
            //if one chick is still lost
            if (chick.home == false) {
                gotAll = false;
            }
        }

        return gotAll;
    }

    protected override void AcceptQuest()
    {
        //set the lost chicks active
        foreach (var chick in lostChicks) {
            chick.gameObject.SetActive(true);
        }

        base.AcceptQuest();
    }

    //will be called each level progression
    private void SetupQuest()
    {
        //diable the chicks by Mary's side when on the quest level
        if (ProgressManager.CurrentLevel == 2) {
            foreach (var chick in sideChicks) {
                chick.SetActive(false);
            }            
        }
    }

    protected override void CompleteQuest()
    {
        base.CompleteQuest();

        FindObjectOfType<WardrobeInventory>().AddAccessoryToInventory(CustomizationGift.PinkTopHat);

        foreach (var chick in lostChicks) {
            chick.gameObject.SetActive(false);
        }

        foreach (var chick in sideChicks) {
            chick.gameObject.SetActive(true);
        }
    }

    protected override QuestIconInfo IconInfo()
    {
        List<int> itemIds = new List<int>() { 99 };

        int count = 0;
        foreach (var chick in lostChicks) {
            if (chick.home == true) {
                count++;
            }
        }

        List<int> caughtCount = new List<int>() { count };
        List<int> maxCount = new List<int>() { lostChicks.Count };

        QuestIconInfo iconInfo = new QuestIconInfo("Bring Lost Chicks Back to Momma", itemIds, caughtCount, maxCount);

        return iconInfo;
    }

    protected override void Cheat_RecieveCurrentQuestMaterials()
    {
        if (playerOnQuest) {
            foreach (var chick in lostChicks) {
                chick.home = true;
            }
        }
        
    }

}
