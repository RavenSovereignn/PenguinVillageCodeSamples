using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCalls : MonoBehaviour
{
    public HealthSystem healthSystem;
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            healthSystem.TakeDmg(10);
        }
    }
}
