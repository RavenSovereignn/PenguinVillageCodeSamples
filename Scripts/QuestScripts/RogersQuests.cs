using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogersQuests : BaseQusetGiver
{
    protected override void CompleteQuest()
    {
        base.CompleteQuest();

        Destroy(GetComponent<DialogueTrigger>());
        GetComponent<BoxCollider>().enabled = false;
    }

}
