using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_input : MonoBehaviour
{
    public float GetHorizontalMovementInput()
    {
        return Input.GetAxis("Horizontal");
    }

    public float GetVerticalMovementInput()
    {
        return Input.GetAxis("Vertical");
    }
    public float GetMouseX()
    {
        return Input.GetAxis("Mouse X");
    }
    public float GetMouseY()
    {
        return Input.GetAxis("Mouse Y");
    }


    public bool IsJumpKeyPressed()
    {
        return Input.GetKey(KeyCode.Space);
    }
}
