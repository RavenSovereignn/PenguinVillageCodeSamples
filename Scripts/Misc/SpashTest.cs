using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpashTest : MonoBehaviour
{
    public GameObject objPrefab;

    public float maxDist;
    public int objSpawned;
    public float delayTime;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl)) {
            StartCoroutine(spawner());
        }

    }

    IEnumerator spawner()
    {
        WaitForSeconds delay = new WaitForSeconds(delayTime);

        for (int i = 0; i < objSpawned; i++) {
            Vector3 offsetPos = Random.insideUnitSphere * maxDist;
            GameObject g = Instantiate(objPrefab, transform.position + offsetPos, Random.rotation);
            g.AddComponent<Rigidbody>();
            Destroy(g, 5);

            yield return delay;
        }

    }


}
