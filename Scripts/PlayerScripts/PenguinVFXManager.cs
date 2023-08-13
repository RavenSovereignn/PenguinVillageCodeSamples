using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinVFXManager : MonoBehaviour
{

    private PenguinController controller;
    private Penguin3DController controller3d;
    bool using3d;

    private Rigidbody rb;

    [Header("Bubble Stream")]
    public GameObject bubbles;
    private ParticleSystem bubblesPS;
    public float bubblesMaxLength;
 
    private bool bubblesActive;
    private float bubblesStartLength;

    private void Start()
    {
        if(TryGetComponent<PenguinController>(out controller)) {
            using3d = false;
        }
        else {
            controller3d = GetComponent<Penguin3DController>();
            using3d = true;
        }
        
        
        rb = GetComponent<Rigidbody>();

        BubblesSetup();


    }


    void Update()
    {
        ManageSwimmingBubbleStream();

    }

    private void BubblesSetup()
    {
        bubblesPS = bubbles.GetComponent<ParticleSystem>();
        bubblesPS.Stop();
    }

    private void ToggleBubbles(bool state)
    {
        if (state) {
            bubblesPS.Clear();
            bubblesPS.Play();
        }else {
            bubblesPS.Stop();
        }

        bubblesActive = state;
    }

    private void ManageSwimmingBubbleStream()
    {
        //turn on bubbles when swimming and moving fast enough
        if (((!using3d && controller.swimming)||(using3d && controller3d.swimming)) && rb.velocity.magnitude > 1) {
            //start bubbles if not already
            if (!bubblesActive) {
                ToggleBubbles(true);
            }
        }
        //stop bubbles on land
        else if(bubblesActive) {
            ToggleBubbles(false);
        }
    }

    

}
