using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Steve : MonoBehaviour
{
    private bool ready;
    private bool jumping;
    void Start()
    {
        
    }

    void Update()
    {
        if (ready) {
            if(Input.GetKeyDown(KeyCode.B) && jumping == false || Input.GetButtonDown("Fire1") && jumping == false) {
                StartCoroutine(Jump());
            }
        }
    }

    IEnumerator Jump()
    {
        jumping = true;
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        Rigidbody rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        //rb.AddForce(new Vector3(-0.8f, 0.8f, (0.4f * Random.value) - 0.2f) * 5, ForceMode.Impulse);

        rb.AddForce((-transform.up  + new Vector3(0,1,0)) * 4, ForceMode.Impulse);

        rb.angularVelocity = Random.insideUnitSphere * 5;

        yield return new WaitForSeconds(2.0f);

        rb.isKinematic = true;

        transform.position = pos;
        transform.rotation = rot;
        jumping = false;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") {
            ready = true;
            TextMeshProUGUI text = other.GetComponent<PlayerManager>().GetPrompt();
            //text.text = "Press B to Throw Steve";
            text.text = "";
            text.enabled = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player") {
            ready = false;
            TextMeshProUGUI text = other.GetComponent<PlayerManager>().GetPrompt();
            text.text = "";
            text.enabled = false;
        }
    }

}
