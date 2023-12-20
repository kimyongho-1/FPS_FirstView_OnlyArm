using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public static MouseLook Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] Transform characterBody;
    public Transform cameraParent;
    [Header("")]
    [SerializeField][Range(0, 10)] float sensitivity;
    float x; float y;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }
    private void FixedUpdate()
    {
        MouseControl();
    }
    void MouseControl()
    {
        x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        y += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        y = Mathf.Clamp(y,-80f,80f);
        cameraParent.localRotation = Quaternion.Euler(-y,0,0);
        characterBody.Rotate(Vector3.up * x);
    }
}
