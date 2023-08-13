using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

[CreateAssetMenu(fileName ="New Item", menuName = "Item/Create New Item")]
public class Items : ScriptableObject
{
    public int id;
    public string itemName;
    public int value;
    public int amount = 0;
    public Sprite icon;
    public ItemType itemType;
    public string Description;
    public int ItemHeal;


    public enum ItemType
    {
        Fish,
        Misc,
        Clothes,
        GatherItem
    }
    
}
