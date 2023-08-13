using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIShrinkExpand : MonoBehaviour
{
    private RectTransform rect;
    private float currentTracker = 0.0f;
    private Vector3 currentScale; 

    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (true) {
            currentTracker += 0.03f;
            float scale = Mathf.Sin(currentTracker) * 0.1f;
            currentScale = Vector3.one + new Vector3(scale, scale, scale);

            rect.localScale = currentScale;
        }
    }


}
