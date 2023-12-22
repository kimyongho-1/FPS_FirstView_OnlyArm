using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public Transform YawRotator;
    public Transform cameraParent;
    public float sensitivity;
    public float x, y;

    private void FixedUpdate()
    {
        MouseControl();
    }
    void MouseControl()
    {
        x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        y += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        y = Mathf.Clamp(y, -75f, 75f);
        cameraParent.localRotation = Quaternion.Euler(-y, 0, 0);
        YawRotator.Rotate(Vector3.up * x);
    }
}
