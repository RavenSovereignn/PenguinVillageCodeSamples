using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnderwaterBreathin : MonoBehaviour
{
    public Penguin3DController penguinC;
    [Range(0,100)]
    public float playerBreath;
    public float coef;
    private float refillCoef = 5f;
    public GameObject Player;
    public HealthSystem healthSystem;
    public float cooldownTime = 2f;
    private float nextDmgTime = 0;
    private bool inBubbleAura;
    public AudioManager audioManager;

   
    public CanvasGroup breathVignette;
    private bool repeat = true;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private float coefVignette = 0.5f;

    public TextMeshProUGUI BreathUI;
    public Image breathIconUIFill;

    //can toggle breathing for testing
    public bool breathingEnabled = true;

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        penguinC = GetComponent<Penguin3DController>();
        audioManager = FindObjectOfType<AudioManager>();
        breathIconUIFill.fillAmount = 1;
        playerBreath = 100f;
    }
    void Update()
    {
        if(penguinC.movingUnderwater == true)
        {
            coef = 4f;
            //Debug.Log("Moving Coef");
        }
        else 
        { 
            coef = 1.5f; 
            //Debug.Log("Standing coef");
                
        }
        if (penguinC.swimming == true)
        {
            if (inBubbleAura == false)
            {
                BreathCount();
            }
            if(inBubbleAura == true)
            {
                BreathRefill();
            }
            breathIconUIFill.fillAmount = playerBreath / 100;
        }
        if(penguinC.swimming == false)
        {
            playerBreath = 100f;
            breathIconUIFill.fillAmount = 1;
            
        }
        if(playerBreath <= 0)
        {
            
           if(Time.time > nextDmgTime)
           {
                audioManager.Play("Drowning");
                //Debug.Log("Player drowns taking dmg");
                healthSystem.TakeDmg(10);
                nextDmgTime = Time.time + cooldownTime;
           }
        }
        BreathUI.text = playerBreath.ToString("F0");
        if(playerBreath > 30)
        {
            breathVignette.alpha = 0;
        }
        if (playerBreath <= 30 && playerBreath != 0 && repeat == true)
        {
            repeat = false;
            fadingIn = true;
            
        }
       
        //disable breathing for testing
        if(Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha1)) {
            breathingEnabled = !breathingEnabled;
        }
        //die cheat code
        if (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Alpha9)) {
            healthSystem.TakeDmg(90);
            playerBreath = 1;
        }

        if (fadingIn)
        {
            if (breathVignette.alpha < 1)
            {
                breathVignette.alpha += Time.deltaTime * coefVignette;
                if (breathVignette.alpha >= 1)
                {
                    fadingIn = false;
                    fadingOut = true;
                }
            }
        }
        if (fadingOut)
        {
            
            if (breathVignette.alpha >= 0)
            {
                breathVignette.alpha -= Time.deltaTime * coefVignette;
                if (breathVignette.alpha >= 0.55f && breathVignette.alpha <= 0.7f)
                {
                    breathVignette.alpha = 0;
                    fadingOut = false;
                    repeat = true;
                }
            }
        }

    }

    public void BreathCount()
    {
        if(playerBreath >= 0 && inBubbleAura == false)
        {
            playerBreath -= coef * Time.deltaTime * ((breathingEnabled) ? 1 : 0);
           
            //Debug.Log(breathIconUIFill.fillAmount);

        }
    }
    private void BreathRefill()
    {
        if(playerBreath <= 100)
        {
            playerBreath += refillCoef * Time.deltaTime;
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag == "Bubble")
        {
            playerBreath = 100f;
            other.gameObject.SetActive(false);
        }
        if (other.transform.tag == "BubbleAura" && playerBreath <= 100)
        {
            inBubbleAura = true;
            
            //Debug.Log("Im in bubble aura");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.tag == "BubbleAura")
        {
            inBubbleAura = false;
        }
    }
}
