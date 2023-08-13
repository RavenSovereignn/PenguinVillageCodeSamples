using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDeath : MonoBehaviour
{
    public PauseMenu menuManager;
    public Transform respawnPoint;
    public ConstructionFade fading;
    public PlayerManager player;

    public Image deathScreen;
    public TMPro.TextMeshProUGUI deathText;

    public Animator penguinAnimator;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PlayerDies()
    {
         StartCoroutine(RespawnSequence());

        //lose items and gold
    }

    private IEnumerator RespawnSequence()
    {
        Time.timeScale = 0;
        menuManager.playerIsInUI = true;
        menuManager.DiablePlayerHUD();

        //play falling anim
        penguinAnimator.SetBool("IsIdle", false);
        penguinAnimator.SetBool("IsFalling", true);

        //make sure transparent so can fade in 
        deathScreen.gameObject.SetActive(true);
        deathScreen.color = new Color(deathScreen.color.r, deathScreen.color.g, deathScreen.color.b, 0);

        //fade out
        WaitForSeconds delay = new WaitForSeconds(0.05f);
        for (int i = 0; i < 50; i++) {
            Color colour = deathScreen.color;
            colour.a += 0.02f;
            deathScreen.color = colour;
            yield return delay;
        }

        //reset player stats and diable the respawn ui
        FindObjectOfType<HealthSystem>().health = 100;
   
        //reset player and move to respawn point
        menuManager.Recall();
        player.transform.position = respawnPoint.position;
        penguinAnimator.SetBool("IsFalling", false);
        penguinAnimator.SetBool("IsIdle", true);

        yield return new WaitForSeconds(1.0f);

        Debug.Log("Fadeing our ");
        
        //fade back in
        for (int i = 0; i < 50; i++) {
            Color colour = deathScreen.color;
            colour.a -= 0.02f;
            deathScreen.color = colour;
            yield return delay;
        }

        menuManager.playerIsInUI = false;
        menuManager.EnablePlayerHUD();

        deathText.gameObject.SetActive(false);
        deathScreen.gameObject.SetActive(false);

        FindObjectOfType<HealthSystem>().dead = false;
    }


}
