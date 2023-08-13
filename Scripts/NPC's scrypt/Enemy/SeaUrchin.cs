using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaUrchin : MonoBehaviour
{
    public HealthSystem healthSystem;
    public int damage;

    public void Start()
    {
        healthSystem = FindObjectOfType<HealthSystem>();
        damage = 5;
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            healthSystem.TakeDmg(damage);
        }
    }
}
