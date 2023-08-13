using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPenguin : MonoBehaviour
{
    

    public float maxheight = 1.0f;

    private Vector3 startPos;

    private float offset;

    void Start()
    {
        startPos = transform.position;
        offset = Random.value;
    }

    void Update()
    {
        transform.position = startPos + new Vector3(0,maxheight,0) * Mathf.Abs(Mathf.Sin(offset + Time.time));


        
    }
}
