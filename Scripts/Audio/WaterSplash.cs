using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{
    public AudioManager audioManager;
    public bool playerFTIW; // FTIW - first time in water (for tutorial script)

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Steve" ||  other.tag == "Chick")
        {
            audioManager.Play("JumpingInWater");
            Debug.Log("Played jump in water");
        }
        if (other.tag == "Player")
        {
            playerFTIW = true;
        }


    }
}
