using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickFollow : MonoBehaviour { 

    private Queue<Vector3> movePoints;
    private Vector3 currentMovePoint;
    private Vector3 latestMovePoint;

    [Header("Following Settings")]
    public float movePointDistances = 1.0f;
    public float jumpForce = 5.0f;
    public float upForceMultiplier = 2.0f;
    public float timeBetweenHops = 0.2f;

    private float lastJumpTime = 0;

    [Header("Picked-Up Settings")]
    public Vector3 pickedupRot;

    [Header("References")]
    public Transform homePoint;
    public Collider hitBox;
    Rigidbody rb;
    Transform target;
    

    [HideInInspector]
    public bool follow = false;
    [HideInInspector]
    public bool home = false;
    bool pickedUp = false;

    Transform heldPoint;

    private float oceanHeight = 0.8f;

    private void OnTriggerEnter(Collider other)
    {
        rb = GetComponent<Rigidbody>();
        movePoints = new Queue<Vector3>();

        if (other.CompareTag("Player")) {
            target = other.transform;
            follow = true;
            currentMovePoint = target.position;
            latestMovePoint = target.position;
        }
    }

    private void Update()
    {
        if (follow && !pickedUp && !home) {
            //find move points of players position
            if (Vector3.Distance(latestMovePoint, target.position) > movePointDistances) {
                movePoints.Enqueue(target.position);
                latestMovePoint = target.position;
            }

            //stop when close to home
            if (Vector3.Distance(transform.position, homePoint.position) < 3) {
                ReturnHome();
                return;
            }

            if(transform.position.y < oceanHeight + 0.2) {
                rb.useGravity = false;
                rb.AddForce(Vector3.up * 0.4f * Time.deltaTime);
            }
            else {
                rb.useGravity = true;
            }

            //stop when close to player
            if (Vector3.Distance(transform.position, target.position) < 2) {
                return;
            }

            //jump towards closest move point
            if (Time.time - lastJumpTime > timeBetweenHops && Vector3.Distance(transform.position, currentMovePoint) > 0.1f) {
                Vector3 force = (target.position - transform.position).normalized;
                force += Vector3.up * upForceMultiplier;

                rb.AddForce(force.normalized * jumpForce);

                lastJumpTime = Time.time - (0.05f * Random.value);
            }

            //once reached current move point get the next one
            else {
                if(movePoints.Count == 0) {
                    currentMovePoint = target.position;
                }
                else {
                    currentMovePoint = movePoints.Dequeue();
                }
            }

            //look at player
            transform.LookAt(target);
            transform.Rotate(-90, 0, 0);

        }
        if (pickedUp) {
            transform.position = heldPoint.position;
            transform.rotation = heldPoint.rotation;
            transform.Rotate(pickedupRot);
        }

    }

    private void ReturnHome()
    {
        follow = false;
        transform.position = homePoint.position;
        home = true;
    }

    public void Pickup(Transform point)
    {
        pickedUp = true;
        heldPoint = point;
        rb.isKinematic = true;
        hitBox.enabled = false;
    }

    public void PutDown(Vector3 throwDir)
    {
        Debug.Log("Thrown");

        pickedUp = false;

        rb.isKinematic = false;
        hitBox.enabled = true;

        rb.AddForce(-throwDir * jumpForce );
    }

}
