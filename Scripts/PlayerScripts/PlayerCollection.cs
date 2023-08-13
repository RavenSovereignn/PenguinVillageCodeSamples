using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollection : MonoBehaviour
{
    public PlayerManager playerMg;
    // Start is called before the first frame update
    void Start()
    {
        playerMg = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
/*
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "BlueFish")
        {
            Debug.Log("I collect blue fish");
            playerMg.updateBlueFish(1);
            Destroy(other.gameObject);
        }
        if (other.transform.tag == "YellowFish")
        {
            Debug.Log("I collect yellow fish");
            playerMg.updateYellowFish(1);
            Destroy(other.gameObject);
        }
        if (other.transform.tag == "RedFish")
        {
            Debug.Log("I collect red fish");
            playerMg.updateRedFish(1);
            Destroy(other.gameObject);
        }
        if (other.transform.tag == "GreenSeaweed")
        {
            Debug.Log("I collect GreenSeaweed");
            playerMg.updateGreenSeaweed(1);
            Destroy(other.gameObject);
        }
        if (other.transform.tag == "BlueSeaweed")
        {
            Debug.Log("I collect BlueSeaweed");
            playerMg.updateBlueSeaweed(1);
            Destroy(other.gameObject);
        }
    }
*/
}
