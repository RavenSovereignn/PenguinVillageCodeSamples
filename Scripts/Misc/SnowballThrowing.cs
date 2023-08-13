using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballThrowing : MonoBehaviour
{
    [Header("References")]
    public GameObject snowball;
    public Cinemachine.CinemachineVirtualCamera WalkCam;
    private Transform throwPoint;

    [Header("Settings")]
    public float throwForce;
    public float throwAngle;

    private void Start()
    {
        throwPoint = transform;
    }


    void Update()
    {
        if(Input.GetMouseButtonDown(0)) {
            ThrowSnowball();   
        }

    }

    private void ThrowSnowball()
    {
        GameObject ball = Instantiate(snowball, throwPoint.position, throwPoint.rotation);

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        Vector3 throwForceVec = new Vector3(0, throwForce / Mathf.Tan(throwAngle) , throwForce);

        rb.AddRelativeForce(throwForceVec, ForceMode.Impulse);

        Destroy(ball, 2.0f);
    }

}
