using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVFX : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<Penguin3DController>().cameras.WalkCam.transform;

    }

    void Update()
    {
        transform.LookAt(player);
        transform.Rotate(0, -90, 0);
    }
}
