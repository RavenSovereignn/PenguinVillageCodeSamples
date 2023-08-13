using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JasmineQuest : BaseQusetGiver {

    protected override void GiveRewards()
    {
        //give reward on second quest
        if (questIndexTracker == 1) {
            FindObjectOfType<WardrobeInventory>().AddAccessoryToInventory(CustomizationGift.ShellBra);
        }
    }

}
