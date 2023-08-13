using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventoryreset : MonoBehaviour
{
    public List<Items> itemList;
    public Transform ItemContent;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var invItem in itemList)
        {
            //Debug.Log(invItem);
            invItem.amount = 0;
            //Debug.Log("Reset item count");
        }
        
    }

}