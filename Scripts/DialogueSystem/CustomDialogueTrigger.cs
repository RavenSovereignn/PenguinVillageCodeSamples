using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDialogueTrigger : MonoBehaviour
{
    private DialogueTrigger dialogueTrigger;
    private void Awake()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();  
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            dialogueTrigger.TriggerDialogue2(0);
            gameObject.SetActive(false);
        }
    }
}
