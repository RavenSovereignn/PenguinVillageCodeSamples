using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessoryDescription : MonoBehaviour {
    
    public CustomizationGift Item;

    [HideInInspector]
    public string description;
    [HideInInspector]
    public string location;
    [HideInInspector]
    public bool playerOwns;
}
