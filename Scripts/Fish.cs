using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FishType { Cod,Snapper,Salmon, N_A}

public class Fish : MonoBehaviour
{
    [Header("References")]
    public GameObject Player;

    [Header("Fish Stats")]
    public FishType fishType;
    public float speed;

    Vector2 direction;
    Vector2 adjustedDirection;
    Vector2 toKeepInbounds;

    //fish stay inside box
    public Transform BoundingTopLeft;
    public Transform BoundingBottomRight;

    //when true fish seek player
    private bool FishDebuging = false;

    void Start()
    {
        direction = new Vector2(1 - 2 * Random.value, 1 - 2 * Random.value);
        direction.Normalize();

        adjustedDirection = direction;

        toKeepInbounds = new Vector2(0, 0);
    }

    void Update()
    {
        KeepInBounds();
        AvoidPlayer();

        transform.Translate(adjustedDirection * speed * Time.deltaTime,Space.World);

        //cheat - fish seek player
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.X)) { FishDebuging = !FishDebuging; }
    }

    private void AvoidPlayer()
    {
        if(Player != null)
        {
            Vector2 playerOffset = transform.position - Player.transform.position;

            if (playerOffset.magnitude < 5)
            {
                direction = playerOffset.normalized * (FishDebuging? -1.0f : 1.0f);
                toKeepInbounds = new Vector2(0, 0);
            }
        }
    }


    private void ChangeSwimDirection(float x)
    {
        Vector3 rot = new Vector3(270, 180, 0);

        if (x < 0) {
            rot.z = 180;
        }

        transform.rotation = Quaternion.Euler(rot);
    }

    private void KeepInBounds()
    {
        Vector2 pos = transform.position;

        if (pos.y > BoundingTopLeft.position.y)
        {
            toKeepInbounds.y = -1;
        }
        else if (pos.y < BoundingBottomRight.position.y)
        {
            toKeepInbounds.y = 1;
        }

        if (pos.x > BoundingTopLeft.position.x)
        {
            toKeepInbounds.x = -1;
            ChangeSwimDirection(-1);
        }
        else if (pos.x < BoundingBottomRight.position.x)
        {
            toKeepInbounds.x = 1;
            ChangeSwimDirection(1);
        }

        adjustedDirection = direction;

        if(toKeepInbounds.y != 0)
        {
            adjustedDirection.y = adjustedDirection.y * Mathf.Sign(adjustedDirection.y) * toKeepInbounds.y;
        }
        if (toKeepInbounds.x != 0)
        {
            adjustedDirection.x = adjustedDirection.x * Mathf.Sign(adjustedDirection.x) * toKeepInbounds.x;
        }

    }
}