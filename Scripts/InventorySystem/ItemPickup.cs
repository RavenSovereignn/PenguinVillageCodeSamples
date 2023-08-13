using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Items item;

    [Tooltip("When true player needs to press E to pickup")]
    public bool ActionPickup = false;
    public float respawnTime = -1.0f;
    private bool resawpning = false;

    private bool canPickup = false;
    private TMPro.TextMeshProUGUI cachedPlayerPrompt;
    public AudioManager audioManager;

    private void Update()
    {
        if(ActionPickup && canPickup && Input.GetKeyDown(KeyCode.E)) {
            Pickup();
        }


    }


    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }
    
    void Pickup()
    {
        
       
        
            audioManager.Play("ItemAdd");
            InventoryManager.Instance.Add(item, 1);

            if(respawnTime > 0) {
                StartCoroutine(Respawn(respawnTime));
                canPickup = false;
            }
            else {
                Destroy(gameObject);
            }
        
        
        if (cachedPlayerPrompt != null) {
            cachedPlayerPrompt.enabled = false;
        }
    }

    public void PickupDontDestroy()
    {
      
            audioManager.Play("ItemAdd");
            InventoryManager.Instance.Add(item, 1);

        
    }

    private IEnumerator Respawn(float respawnTime)
    {
        GetComponent<Renderer>().enabled = false;
        resawpning = true;

        yield return new WaitForSeconds(respawnTime);

        resawpning = false;
        GetComponent<Renderer>().enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!ActionPickup) {
                Pickup();
                Debug.Log("Player PickedUp: " + gameObject.name);
            }
            else if (resawpning == false) {
                PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
                cachedPlayerPrompt = playerMan.GetPrompt();
                cachedPlayerPrompt.enabled = true;
                cachedPlayerPrompt.text =  "Press E to pickup " + item.itemName;
                canPickup = true;
            }
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player" && ActionPickup) {
                PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
                cachedPlayerPrompt.enabled = false;
                canPickup = false;
        }
    }
}
