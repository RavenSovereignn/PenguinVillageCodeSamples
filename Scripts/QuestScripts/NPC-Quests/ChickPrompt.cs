using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickPrompt : MonoBehaviour
{
    
   
    public bool chickpromtActivate = false;

    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" )
        {
            chickpromtActivate = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            chickpromtActivate = false;
        }
    }
}
