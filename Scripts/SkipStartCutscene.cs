using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipStartCutscene : MonoBehaviour
{
    public float power;
    private float maxPower = 5;
    private float chargeSpeed = 3;

    public Image SkipImage;
    public OpeningCinematic openingCinematic;
    public GameObject IntroText;

    [HideInInspector]public bool phase2 = false;

    void Update()
    {
        if (!phase2) {
            if (Input.GetKey(KeyCode.Space)) {
                power += Time.deltaTime * chargeSpeed;
                SkipImage.fillAmount = power / maxPower;
            }
            else {
                power = Mathf.Clamp(power - Time.deltaTime * chargeSpeed, 0.0f, maxPower);
                SkipImage.fillAmount = power / maxPower;
            }
            {
                if (power >= maxPower) {
                    SkipIntroText();
                }
            }
        }
        else {
            if (Input.GetKey(KeyCode.Space)) {
                SkipImage.enabled = true;
                SkipImage.fillAmount = openingCinematic.progress;
            }
            else {
                SkipImage.enabled = false;
            }
        }
        
    }


    private void SkipIntroText()
    {
        IntroText.SetActive(false);
        openingCinematic.startMoving = true;
    }
}
