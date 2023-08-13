using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public static bool InputAccept()
    {
        return Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("Fire1");
    }

    public static bool InputDecline()
    {
        return Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("Fire2") || Input.GetKeyDown(KeyCode.Escape);
    }


}
