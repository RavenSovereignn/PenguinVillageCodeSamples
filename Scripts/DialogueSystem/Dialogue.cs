using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [NonReorderable]
    public string[] charName;
    [TextArea(3,10)]
    //[NonReorderable]
    public string[] sentences;
    [NonReorderable]
    public Sprite[] Icons;

    [HideInInspector]
    public int latestDialogueIndex = 0;
}
