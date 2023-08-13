using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class ConstructionFade : MonoBehaviour
{
    public CanvasGroup UpgadePanel;
    public GameObject UpgradeText;

    [HideInInspector]
    public bool fadeIn = false;
    [HideInInspector]
    public bool fadeOut = false;

    private bool ShowText = false;

    // Update is called once per frame
    void Update()
    {
        if(fadeIn)
        {
            if(UpgadePanel.alpha < 1)
            {
                UpgadePanel.alpha += Time.deltaTime;
                if(UpgadePanel.alpha >= 1)
                {
                    UpgradeText.SetActive(ShowText);
                    fadeIn = false;
                }
            }
        }
        if (fadeOut)
        {
            UpgradeText.SetActive(false);
            if (UpgadePanel.alpha >= 0)
            {
                UpgadePanel.alpha -= Time.deltaTime;
                if (UpgadePanel.alpha == 0)
                {
                    
                    fadeOut = false;
                }
            }
        }
    }

    public void FadeInUI(bool _showUpgradeText = false)
    {
        ShowText = _showUpgradeText;
        fadeIn = true;
        //Debug.Log("Starting to fadeIn");
    }
    public void FadeOutUI()
    {
        fadeOut = true;
        //Debug.Log("Starting to fadeOut");
    }
}
