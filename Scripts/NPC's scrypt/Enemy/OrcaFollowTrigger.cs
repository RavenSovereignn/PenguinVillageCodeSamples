using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcaFollowTrigger : MonoBehaviour
{
    public bool playerInRange;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerInRange = false;
        }
    }
}
