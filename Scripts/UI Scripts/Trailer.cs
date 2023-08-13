using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trailer : MonoBehaviour
{
    public Image qrItch;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Picture());
        }
    }
    IEnumerator Picture()
    {
        
        for (int i = 0; i < 100; i++)
        {
            qrItch.color = new Color(1, 1, 1, 0.01f * (i*2));
            yield return null;
        }
    }
 
}
