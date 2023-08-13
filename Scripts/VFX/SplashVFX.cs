using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashVFX : MonoBehaviour
{
    public GameObject SplashEffect;

    private Quaternion spawnRot;

    private void Start()
    {
        //the effect needs to start at this rotation
        spawnRot = Quaternion.Euler(new Vector3(90, 0, 0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Steve>(out Steve _)) {
            //steve has huge trigger hitbox doesn't work well
            return;
        }

        Instantiate(SplashEffect, other.transform.position, spawnRot);
    }

}
