
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class WoodPickup : MonoBehaviour
{
    public Items item;
    private int WoodCollected;

    public float power;
    public float maxPower = 6;
    private float chargeSpeed = 3;
    private bool CanPickUp;
    public bool WoodPickedUp;

    private TMPro.TextMeshProUGUI cachedPlayerPrompt;
    public Image progressBarFill;
    public TMPro.TextMeshProUGUI progressBarText;
    public GameObject progressBar;
   
    private void Awake()
    {
        progressBar = GameObject.Find("GatherProgressBar");
        progressBarFill = progressBar.transform.GetChild(1).GetComponent<Image>();
        progressBarText = progressBar.GetComponentInChildren<TMPro.TextMeshProUGUI>();

    }
    private void Start()
    {
        progressBar.SetActive(false);
    }

    private void Update()
    {
        if (CanPickUp && Input.GetKey(KeyCode.E) && WoodPickedUp == false)
        {
            progressBar.SetActive(true);
            power += Time.deltaTime * chargeSpeed;
            progressBarFill.fillAmount = power / maxPower;
            progressBarText.text = power.ToString("F0");
        }

        if (power >= maxPower && WoodPickedUp == false)
        {
            
            progressBar.SetActive(false);
            Pickup();
            
        }
    }
    void Pickup()
    {
        if(this.gameObject.tag == "WoodLog")
        {
            this.gameObject.SetActive(false);
        }
        else{ WoodPickedUp = true; }
       
        CanPickUp = false;         
        WoodCollected = Random.Range(1, 6);
        InventoryManager.Instance.Add(item, WoodCollected);
        cachedPlayerPrompt.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && WoodPickedUp == false)
        {
            PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
            cachedPlayerPrompt = playerMan.GetPrompt();
            cachedPlayerPrompt.enabled = true;
            cachedPlayerPrompt.text = "Hold E to gather wood";
        }
    }

    private void OnTriggerStay(Collider other)
    {
        /*if (other.transform.tag == "Player")
        {
                PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
                cachedPlayerPrompt = playerMan.GetPrompt();
                cachedPlayerPrompt.enabled = true;
                cachedPlayerPrompt.text = item.itemName + ", E to Pickup";
        }
        */
        if (other.transform.tag == "Player")
        {
            CanPickUp = true;

        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
            cachedPlayerPrompt.enabled = false;
            CanPickUp = false;
            progressBar.SetActive(false);
            power = 0;
        }
    }
}
