using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


public class PenguinCustomization : MonoBehaviour
{
    Slider hueSlider;

    public Texture2D penguinCoatTexure;
    public Texture2D penguin;


    void Start()
    {

        Texture2D colourchange = new Texture2D(penguinCoatTexure.width, penguinCoatTexure.height, penguinCoatTexure.format, false);

        Graphics.CopyTexture(penguinCoatTexure, colourchange);

        Color[] pixels2 = colourchange.GetPixels();

        int counter = 0;

        foreach (var colour in colourchange.GetPixels()) {

            float alpha = colour.a;
            float hue, sat, val;

            Color.RGBToHSV(colour, out hue, out sat, out val);

            hue += 0.30f;

            Mathf.Clamp(hue, 0, 1.0f);

            pixels2[counter] = Color.HSVToRGB(hue, sat, val, true);
            pixels2[counter].a = alpha;

            counter++;
        }

        colourchange.SetPixels(pixels2);
        colourchange.Apply();

        byte[] bytes = colourchange.EncodeToPNG();
        File.WriteAllBytes("Assets/Generated/" + "test2" + ".png", bytes);
    }

    void Update()
    {
        

    }
}
