using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

public class PenguinController : MonoBehaviour
{
    [Header("References")]
    public GameObject Camera;
    public Cinemachine.CinemachineVirtualCamera vCam;
    private CinemachineOrbitalTransposer orbitalCam;
    public Transform penguinBody;
    private Vector3 camOffset;
    private Rigidbody rb;
    private Collider playerCollider;
    public Image DashIcon;

    [Header("Stats")]
    public float swimSpeed = 10.0f;
    public float swimTurnSpeed = 2.0f;
    public float walkSpeed = 1.0f;
    public float walkTurnSpeed = 100.0f;
    public float jumpCooldownTime = 0.1f;
    private float prevJumpTime = 0.0f;

    [HideInInspector]
    public bool swimming = false;
    private float prevAngle;

    [Header("Dash Stats")]
    public float dashSpeedMultiplier = 14f;
    public float dashTurnSpeed = 100;
    public float dashTime = 0.5f;
    public float cooldownTime;

    private bool isDashing = false;
    private bool canDash = true;

    [Header("Belly Sliding")]
    public float bellySlideSpeed;

    //scripts can listen for when player enters or exits water
    public static UnityAction<bool> PlayerEnteredWater;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();

        orbitalCam = vCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();

        camOffset = Camera.transform.position - transform.position;
        prevAngle = transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        if (swimming) {
            Swim();
            ManageCamera();

            ////other dashing method
            //if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) {
            //    DashSwim();
            //}
            //else {
            //    rb.drag = 0;
            //    Swim();
            //}
        }
        else {
            Walk3rdPerson();
        }

        //jump and dash
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Fire3")) {
            if (swimming && canDash) {
                StartCoroutine(Dash());
            }
            //jump with cooldown
            else if(Time.time - prevJumpTime > jumpCooldownTime) {     
                rb.AddForce(new Vector3(0, 5, 0), ForceMode.Impulse);
                prevJumpTime = Time.time;
            }
        }

    }

    private void ManageCamera()
    {
        Vector3 camPos = transform.position + camOffset;

        if (swimming) {
            camPos.y = Mathf.Clamp(camPos.y, -100, 0);
        }

        Camera.transform.position = camPos;
       
    }

    private void Walk()
    {
        Vector3 Moveinput = new Vector3();

        Moveinput += Vector3.forward * -Input.GetAxis("Vertical");
        Moveinput += Vector3.right * -Input.GetAxis("Horizontal");

        transform.position += Moveinput * walkSpeed * Time.deltaTime;
    }

    private void Walk3rdPerson()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if(input.magnitude > 0.1) {
            //Vector3 newRot = new Vector3(-90, orbitalCam.m_XAxis.Value, 0);
            //penguinBody.rotation = Quaternion.Euler(newRot);

            Vector3 offset = transform.position - vCam.transform.position;
            float trig = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            Vector3 newRot = new Vector3(-90, trig, 0);
            penguinBody.rotation = Quaternion.Euler(newRot);

        }

        Vector3 Moveinput = new Vector3();

        Moveinput += -penguinBody.up * input.y;
        Moveinput += penguinBody.right * input.x;

        transform.position += Moveinput * walkSpeed * Time.deltaTime;


        //transform.Rotate(new Vector3(0, Input.GetAxis("Horizontal") * walkTurnSpeed * Time.deltaTime, 0));
    }

    private void Swim()
    {
        Vector3 moveInput = new Vector3();

        moveInput += Vector3.up * Input.GetAxis("Vertical");
        moveInput += Vector3.right * -Input.GetAxis("Horizontal");
        moveInput.Normalize();

        //transform.position += moveInput * swimSpeed * Time.deltaTime * ((isDashing) ? dashSpeedMultiplier : 1);
        rb.velocity = (moveInput * swimSpeed * ((isDashing) ? dashSpeedMultiplier : 1));

        Rotate(moveInput);
    }

    private void BellySlide()
    {
        GetComponent<Collider>().material.dynamicFriction = 0.1f;
        Vector3 rot = new Vector3(90, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        transform.rotation = Quaternion.Euler(rot);

        Vector3 Moveinput = new Vector3();

        Moveinput += Vector3.forward * -Input.GetAxis("Vertical");
        Moveinput += Vector3.right * -Input.GetAxis("Horizontal");

        rb.AddForce(Moveinput * bellySlideSpeed);
    }

    //swim faster but restrict sideways movement
    private void DashSwim()
    {
        rb.drag = 10;

        Vector3 dashVec = transform.up * swimSpeed * dashSpeedMultiplier;
        rb.AddForce(dashVec);

        transform.Rotate(Vector3.forward, Input.GetAxis("Horizontal") * dashTurnSpeed * Time.deltaTime);
    }

    private void Rotate(Vector3 moveInput)
    {
        float trig = Mathf.Atan2(-moveInput.x, moveInput.y) * Mathf.Rad2Deg;
        float trig2 = trig - 360;

        //Debug.Log($"trig 1: {trig}, trig2: {trig2}");

        if (Mathf.Abs(prevAngle - trig2) < Mathf.Abs(prevAngle - trig)) {
            trig = trig2;
        }

        float angle = Mathf.Lerp(prevAngle, trig, swimTurnSpeed * Time.deltaTime);

        Vector3 newRot = new Vector3(0, 0, angle);
        transform.rotation = Quaternion.Euler(newRot);
        prevAngle = angle;
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        yield return new WaitForSeconds(dashTime);

        isDashing = false;

        DashIcon.fillAmount = 1f;
        WaitForSeconds delay = new WaitForSeconds(cooldownTime / 100.0f);

        for (int i = 0; i < 100; i++) {

            DashIcon.fillAmount -= 0.01f;

            yield return delay;
        }

        canDash = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "WaterTop") {
            swimming = !swimming;

            PlayerEnteredWater.Invoke(swimming);


            //playerCollider.isTrigger = swimming;
            rb.useGravity = !swimming;
            //rb.isKinematic = swimming;

            if (swimming) {
                //set camera to 'swim mode'
                orbitalCam.m_FollowOffset = new Vector3(0, 1, 8.5f);
                orbitalCam.m_XAxis.m_MaxSpeed = 0;
                orbitalCam.m_XAxis.Value = 0;

                //reset player rotation
                Vector3 newRot = new Vector3(-90, 0, 0);
                penguinBody.rotation = Quaternion.Euler(newRot);

                //align to fishes z pos
                Vector3 pos = new Vector3(transform.position.x, transform.position.y - 5, 10);
                transform.position = pos;

                rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
            else {
                //set camera to regular 3rd person
                orbitalCam.m_FollowOffset = new Vector3(0, 2, -5);
                orbitalCam.m_XAxis.m_MaxSpeed = 200;

                //move the player onto the ice
                Vector3 pos = new Vector3(transform.position.x, 2.3f, 8.5f);
                transform.position = pos;

                // reset rotation
                Vector3 rot = new Vector3(0, 180, 0);
                transform.rotation = Quaternion.Euler(rot);

                //constrain rotation in x and z axis
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

                Vector3 vel = new Vector3(0, 5, -1.5f);
                rb.velocity = vel;
            }


        }
    }



}
