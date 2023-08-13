using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PenguinColourCustomization : MonoBehaviour
{
    [Header("References")]
    public Slider hueSlider;
    private Material mat;

    public float HueShift;

    void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        mat = renderer.material;
        
    }

    void Update()
    {
        HueShift = hueSlider.value;
        mat.SetFloat("_HueShift", HueShift);
       
    }
}
