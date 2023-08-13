using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    public float health;
    public Image HealthBarFill;
    public float cooldownTime = 10f;
    private float nextHealTime = 0;
    public TextMeshProUGUI HealthCountUI;

    public Penguin3DController penguinC;

    public CanvasGroup dmgVignette;
    private bool repeat = true;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private bool takingDmg = false;

    [HideInInspector]
    public bool dead = false;

    void Start()
    {
        health = 100f;
    }

    
    void Update()
    {
        HealthBarFill.fillAmount = health/100;
        HealthCountUI.text =health.ToString("F0");
        if (health < 100 && penguinC.swimming == false)
        {
            if (Time.time > nextHealTime)
            {
                HealDmg(10);
                nextHealTime = Time.time + cooldownTime;
            }
        }

        if(health <= 0 && !dead)
        {
            FindObjectOfType<PlayerDeath>().PlayerDies();
            dead = true;
            //Destroy(this.gameObject);
        }

        if (takingDmg == true && health != 0 && repeat == true)
        {
            repeat = false;
            fadingIn = true;

        }

        if (fadingIn)
        {
            if (dmgVignette.alpha < 1)
            {
                dmgVignette.alpha += Time.deltaTime;
                if (dmgVignette.alpha >= 1)
                {
                    fadingIn = false;
                    fadingOut = true;
                }
            }
        }
        if (fadingOut)
        {

            if (dmgVignette.alpha >= 0)
            {
                dmgVignette.alpha -= Time.deltaTime;
                if (dmgVignette.alpha == 0)
                {

                    fadingOut = false;
                    takingDmg = false;
                    repeat = true;
                }
            }
        }
    }

    public void TakeDmg(int amount)
    {
        health -= amount;
        takingDmg = true;
    }
    public void HealDmg(int amount)
    {
        health += amount;
    }
}
