using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupChick : MonoBehaviour
{
    public List<ChickFollow> chicksInRange;

    //[HideInInspector]
    public bool holdingChick = false;
    private ChickFollow heldChick = null;

    public TMPro.TextMeshProUGUI pickupPrompt;
    public GameObject getchickbactPrompt;
    public GameObject chickPromptUI;
    public ChickPrompt chickPrompt;

    private void Start()
    {
        chicksInRange = new List<ChickFollow>();
        chickPrompt = FindObjectOfType<ChickPrompt>();
    }

    private void Update()
    {
        if(chickPrompt.chickpromtActivate == true && holdingChick)
        {
            chickPromptUI.SetActive(true);
        }
        else { chickPromptUI.SetActive(false); }
        if(!holdingChick && chicksInRange.Count > 0) {
            pickupPrompt.enabled = true;

        }
        else {
            pickupPrompt.enabled = false;
        }

        //pickup chick
       if(Input.GetKeyDown(KeyCode.E) && chicksInRange.Count > 0 && !holdingChick) {
            if (!chicksInRange[0].home) {
                chicksInRange[0].Pickup(transform);
                heldChick = chicksInRange[0];
                holdingChick = true;
                getchickbactPrompt.SetActive(true);
            }      
       }
       //put down chick
       else if ((Input.GetKeyDown(KeyCode.E)) && holdingChick) {
            heldChick.PutDown(transform.forward);

            getchickbactPrompt.SetActive(false);
            StartCoroutine( DelayBoolChange());
            if (chicksInRange.Contains(heldChick)) {
                chicksInRange.Remove(heldChick);
            }
        }

    }

    IEnumerator DelayBoolChange()
    {
        //wait two frames before switching bool
        for (int i = 0; i < 2; i++) {
            yield return new WaitForEndOfFrame();
        }
        holdingChick = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ChickFollow>(out ChickFollow chick)) {
            if (!chick.home) {
                chicksInRange.Add(chick);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ChickFollow>(out ChickFollow chick)) {
            if (chicksInRange.Contains(chick)) {
                chicksInRange.Remove(chick);
            }
        }
    }
}
