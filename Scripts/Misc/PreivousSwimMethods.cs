using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreivousSwimMethods : MonoBehaviour
{

    /*
    //right stick to rotate penguin, left to move
    private Vector3 SwimInput2()
    {
        transform.Rotate(Vector3.right, -Input.GetAxis("RightStick Y") * Time.deltaTime * swimTurnSpeed, Space.Self);
        transform.Rotate(Vector3.forward, -Input.GetAxis("RightStick X") * Time.deltaTime * swimTurnSpeed, Space.Self);

        Vector3 moveInput = new Vector3();

        moveInput += transform.up * Input.GetAxis("Vertical");
        //moveInput += transform.right * Input.GetAxis("Horizontal");
        moveInput.Normalize();

        return moveInput;
    }

    //mario like look around horizontal not vertical and use a and b to go up and down
    private Vector3 SwimInput6()
    {
        Vector3 moveInput = transform.up * Input.GetAxis("Vertical");
        moveInput += transform.right * Input.GetAxis("Horizontal");

        float rotUpangle = 0;

        //up
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) {
            moveInput += -transform.forward;
            rotUpangle = -45;
        }
        //down
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Fire2")) {
            moveInput += transform.forward;
            rotUpangle = 45;
        }


        if (moveInput.magnitude > 0.1) {
            Vector3 offset = transform.position - SwimCam1.transform.position;

            float trig = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            Vector3 newRot = new Vector3(90 + rotUpangle, trig, 0);
            transform.rotation = Quaternion.Euler(newRot);

        }


        moveInput.Normalize();

        return moveInput;
    }

        //mouse to rotate penguin, w to move
    private Vector3 SwimInput3()
    {
        transform.Rotate(Vector3.right, Input.GetAxis("Mouse Y") * Time.deltaTime * swimTurnSpeed * 3, Space.Self);
        transform.Rotate(Vector3.forward, -Input.GetAxis("Mouse X") * Time.deltaTime * swimTurnSpeed * 2, Space.Self);

        Vector3 moveInput = new Vector3();

        //moveInput += (transform.position - vCam.transform.position) * Input.GetAxis("Vertical");
        moveInput += transform.up * Input.GetAxis("Vertical");
        moveInput.Normalize();

        return moveInput;
    }

    

    private Vector3 SwimInputMario(bool dashing)
    {
        //penguinBody.transform.localRotation = Quaternion.Euler(new Vector3(-90, 180, 0));

        Vector3 moveInput = penguinBody.up * Input.GetAxis("Vertical");
        moveInput += -penguinBody.right * Input.GetAxis("Horizontal");

        float rotUpangle = 0;

        //up
        if (Input.GetKey(KeyCode.Space) || Input.GetButton("Fire1")) {
            moveInput += new Vector3(0, 0.6f, 0.4f);
            rotUpangle = 45;
        }
        //down
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Fire2")) {
            moveInput += new Vector3(0, -0.6f, 0.4f);
            rotUpangle = -45;
        }

        if (moveInput.magnitude > 0.1) {
            Vector3 offset = transform.position - SwimCam2_KeyboardAlt.transform.position;

            float trig = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
            Vector3 newRot = new Vector3(-90 + rotUpangle, 180 + trig, 0);
            penguinBody.rotation = Quaternion.Euler(newRot);
        }

        //look up and down
        Vector3 camFollowOffset = SwimCam2_KeyboardAlt.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset;
        camFollowOffset.y += -Input.GetAxis("Mouse Y") * Time.deltaTime * 30.0f;
        camFollowOffset.y = Mathf.Clamp(camFollowOffset.y, -3, 6);
        SwimCam2_KeyboardAlt.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset = camFollowOffset;

        moveInput.Normalize();

        return moveInput;
    }


    private Vector3 SwimInputKeyboardMain(bool dashing)
    {
        Vector3 moveInput = transform.forward * Input.GetAxis("Vertical");

        if (true || moveInput.magnitude > 0.1) {
            Vector3 offset = transform.position - SwimCam1_KeyboardMain.transform.position;

            transform.LookAt(transform.position + offset.normalized);
            //transform.Rotate(Vector3.right, 90, Space.Self);
            //transform.Rotate(Vector3.up, 180, Space.Self);

            lookPoint.position = transform.position + offset;
        }

        //moveInput += (transform.position - vCam.transform.position) * Input.GetAxis("Vertical");
        moveInput += transform.forward * Input.GetAxis("Vertical") + transform.right * -Input.GetAxis("Horizontal");
        moveInput.Normalize();

        //LevelOutPenguin();

        return moveInput;
    }

    private Vector3 SwimInputControllerAlternate(bool dashing)
    {
        Vector3 penguinTilt = transform.right;

        float tiltAngle = Mathf.Atan2(1, penguinTilt.y) * Mathf.Rad2Deg - 90.0f;

        if (Mathf.Abs(tiltAngle) > 5) {
            tiltAngle = Mathf.Clamp(tiltAngle, -10, 10);
            transform.Rotate(Vector3.up, tiltAngle, Space.Self);
        }

        Vector3 rotinput = new Vector2(Input.GetAxis("Vertical"), -Input.GetAxis("Horizontal"));
        rotinput.Normalize();

        Vector3 playerRot = transform.rotation.eulerAngles;

        Debug.Log($"{Mathf.Abs(playerRot.x)} , {Mathf.Abs(playerRot.x - 180)}");

        //how close to facing upwards, to stop spinning
        float facingUpwards = Mathf.Min(Mathf.Abs(playerRot.x), Mathf.Abs(playerRot.x - 180));
        //Debug.Log(facingUpwards);

        if (facingUpwards < 30) {
            if(facingUpwards == 0) {
                rotinput.y /= 10.0f;
            }
            else {
                rotinput.y /= 10.0f * (1 - facingUpwards / 30.0f);
            }
        }

        transform.Rotate(transform.right, rotinput.x * Time.deltaTime * swimTurnSpeed, Space.World);
        transform.Rotate(transform.forward, rotinput.y * Time.deltaTime * swimTurnSpeed, Space.World);

        Vector3 moveInput = new Vector3();

        float ForwardInput()
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                return 1.0f;
            }
            else {
                return Input.GetAxis("RightTrigger");
            }
        }

        moveInput += transform.up * ForwardInput();
        
        return moveInput;
    }



   */
}
