using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcaTriggers : MonoBehaviour
{
    public GameObject orcaObj;



    
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            orcaObj.SetActive(true);
        }
    }
}
