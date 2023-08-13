using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishShaderController : MonoBehaviour
{
    [Header("Targetting Fish")]
    public Color HighlightColour;
    public float HighlightWidth = 45.0f;

    //to reset back to original
    private Material cachedFishMat;
    private float cachedStartWidth;
    private Color cachedStartColour;

    public Action SetFishTargetted(GameObject fish)
    {
        Material fishMatInstance = fish.GetComponent<MeshRenderer>().material;

        cachedFishMat = fishMatInstance;
        cachedStartWidth = fishMatInstance.GetFloat("_Outline_Width");
        cachedStartColour = fishMatInstance.GetColor("_Outline_Color");

        fishMatInstance.SetFloat("_Outline_Width", HighlightWidth);
        fishMatInstance.SetColor("_Outline_Color", HighlightColour);

        return ResetFishShader;
    }

    public void ResetFishShader()
    {
        if(cachedFishMat != null) {
            cachedFishMat.SetFloat("_Outline_Width", cachedStartWidth);
            cachedFishMat.SetColor("_Outline_Color", cachedStartColour);
        }  
    }



}

