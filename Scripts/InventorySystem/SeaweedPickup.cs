using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SeaweedPickup : MonoBehaviour
{
    public Items item;

    public float power;
    private float maxPower = 5;
    private float chargeSpeed = 3;
    private bool CanPickUp;
    private int SeaweedCollected;

    private TMPro.TextMeshProUGUI cachedPlayerPrompt;
    public Image progressBarFill;
    public TMPro.TextMeshProUGUI progressBarText;
    public GameObject progressBar;
    public Rigidbody PlayerRB;
    public GameObject Player;

    private void Awake()
    {
        progressBar = GameObject.Find("GatherProgressBar");
        progressBarFill = progressBar.transform.GetChild(1).GetComponent<Image>();
        progressBarText = progressBar.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        Player = GameObject.Find("Player");
        PlayerRB = Player.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        progressBar.SetActive(false);
    }

    private void Update()
    {

        if (CanPickUp && Input.GetKey(KeyCode.E))
        {
            PlayerRB.isKinematic = true;
            progressBar.SetActive(true);
            power += Time.deltaTime * chargeSpeed;
            progressBarFill.fillAmount = power / maxPower;
            progressBarText.text = power.ToString("F0");
        }
        if(CanPickUp && !Input.GetKey(KeyCode.E))
        {
            PlayerRB.isKinematic = false;
        }
        
        if (power >= maxPower)
        {
            PlayerRB.isKinematic = false;
            progressBar.SetActive(false);
            Pickup();
        }
    }
    void Pickup()
    {
        CanPickUp = false;       
        SeaweedCollected = Random.Range(1, 6);
        InventoryManager.Instance.Add(item, SeaweedCollected);
        cachedPlayerPrompt.enabled = false;
        gameObject.SetActive(false);
        power = 0;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" )
        {
            PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
            cachedPlayerPrompt = playerMan.GetPrompt();
            cachedPlayerPrompt.enabled = true;
            cachedPlayerPrompt.text = "Hold E to gather seaweed" ;
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
        if(other.transform.tag == "Player")
        {
            PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
            cachedPlayerPrompt.enabled = false;
            CanPickUp = false;
            Debug.Log("Player Exits seaweed");
            progressBar.SetActive(false);
            power = 0;
           
        }
    }
}
