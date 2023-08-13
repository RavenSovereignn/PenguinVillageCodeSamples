using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyPickup : MonoBehaviour
{
    private int WoodCollected;
    public PlayerManager PM;

    public float power;
    public float maxPower = 6;
    private float chargeSpeed = 3;
    private bool CanPickUp;
    public bool MoneyPickedUp;
    public int MoneyAmount;

    private TMPro.TextMeshProUGUI cachedPlayerPrompt;
    public Image progressBarFill;
    public TMPro.TextMeshProUGUI progressBarText;
    public GameObject progressBar;

    public GameObject chestOpen;
    public GameObject chestClosed;

    private void Awake()
    {
        PM = FindObjectOfType<PlayerManager>();
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
        if (CanPickUp && Input.GetKey(KeyCode.E) && MoneyPickedUp == false)
        {
            progressBar.SetActive(true);
            power += Time.deltaTime * chargeSpeed;
            progressBarFill.fillAmount = power / maxPower;
            progressBarText.text = power.ToString("F0");
        }

        if (power >= maxPower && MoneyPickedUp == false)
        {

            progressBar.SetActive(false);
           
            Pickup();

        }
    }
    void Pickup()
    {
        chestClosed.SetActive(false);
        chestOpen.SetActive(true);
        cachedPlayerPrompt.enabled = false;
        FindObjectOfType<WardrobeInventory>().AddAccessoryToInventory(CustomizationGift.PirateHat);
        PM.PenguinCash = PM.PenguinCash + MoneyAmount;
        MoneyPickedUp = true;
        CanPickUp = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player" && MoneyPickedUp == false)
        {
            PlayerManager playerMan = other.gameObject.GetComponent<PlayerManager>();
            cachedPlayerPrompt = playerMan.GetPrompt();
            cachedPlayerPrompt.enabled = true;
            cachedPlayerPrompt.text = "You have found chest, hold E to open it!";
        }
    }

    private void OnTriggerStay(Collider other)
    {
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

