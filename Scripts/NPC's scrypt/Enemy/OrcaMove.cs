using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcaMove : MonoBehaviour
{
    [SerializeField] Transform endPosition;
    [SerializeField] Transform startPosition;
    [SerializeField] float OrcaSpeed;
    public Vector3 orcaFollowDestination;
    public HealthSystem healthSystem;
    public OrcaTriggers orcaTrigger;
    public OrcaFollowTrigger orcaFollowTrigger;
    public Transform Player;
    public Penguin3DController penguin3DController;


    private void Start()
    {
        penguin3DController = FindObjectOfType<Penguin3DController>();
    }

    void Update()
    {

        if (orcaFollowTrigger.playerInRange == false || penguin3DController.swimming == false)
        {
            //Debug.Log("Orca going to end");
            orcaFollowDestination = endPosition.position;

            Quaternion currentRot = transform.rotation;

            transform.LookAt(endPosition);
            transform.Rotate(-90, -90, 0);

            transform.rotation = Quaternion.Lerp(currentRot, transform.rotation, 0.015f);

        }
        if (orcaFollowTrigger.playerInRange == true && penguin3DController.swimming == true)
        {
            //Debug.Log("Orca going to player");
            orcaFollowDestination = Player.position;

            Quaternion currentRot = transform.rotation;

            transform.LookAt(Player);
            transform.Rotate(-90, -90 ,0);

            transform.rotation = Quaternion.Lerp(currentRot, transform.rotation, 0.015f);
        }
        transform.position = Vector3.MoveTowards(transform.position, orcaFollowDestination, OrcaSpeed * Time.deltaTime);
        
        if(transform.position == endPosition.position)
        {
            //Debug.Log("Orca hit collider");
            transform.position = startPosition.position;
            //Debug.Log("Orca moved back");
            StopOrca();
        }
    }

    public void StopOrca()
    {
        gameObject.SetActive(false);
        //Debug.Log("stoppedOrca");
    }

    IEnumerator ChopCooldown()
    {
        healthSystem.TakeDmg(20);
        yield return new WaitForSeconds(5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            StartCoroutine(ChopCooldown());
        }
    }
}
