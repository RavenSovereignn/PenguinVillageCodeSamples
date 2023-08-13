using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFloat : MonoBehaviour
{
    public float moveDistance;
    public float moveSpeed;

    private Vector3 startPosition;
    private Vector3 pos;

    float offsetX;
    float offsetY;
    float offsetZ;

    void Start()
    {
        startPosition = transform.position;

        offsetX = (Random.value * 2.0f) - 1.0f;
        offsetY = (Random.value * 2.0f) - 1.0f;
        offsetZ = (Random.value * 2.0f) - 1.0f;
    }

    void Update()
    {
        pos.x = Mathf.Sin((Time.time + offsetX) * moveSpeed) * moveDistance ;
        pos.y = Mathf.Sin((Time.time + offsetY) * moveSpeed) * moveDistance ;
        pos.z = Mathf.Sin((Time.time + offsetZ) * moveSpeed) * moveDistance ;

        transform.position = startPosition + pos;
    }


}
