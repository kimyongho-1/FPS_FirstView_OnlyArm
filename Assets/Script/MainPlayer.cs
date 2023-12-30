using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPlayer : MonoBehaviour
{
    public Transform YawRotator;
    public Transform cameraParent;
    public float sensitivity;
    public float x, y;
    public float moveSpeed = 0.2f;
    public IKhandling IK;
    Rigidbody rb;

    public Transform CamProxy, ItemPorxy;
    public Transform WeaponRoot;
    
    public float mul, camMul;
    Vector3 weaponRootFixedVector;
    Vector3 camRootFixedVector;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Vector3 dir; float dist;
        dir =  WeaponRoot.position - ItemPorxy.position;
        dist = dir.magnitude;
        dir = dir.normalized;
        weaponRootFixedVector = dir * dist;
        dir = cameraParent.position -  CamProxy.position;
        camRootFixedVector = dir.normalized * dir.magnitude;
    }
    Vector3 move;
    private void FixedUpdate()
    {
        rb.MovePosition(move);
        return;
        MouseControl();
        KeyBoardControl();

    }
    private void Update()
    {
        MouseControl();
        KeyBoardControl();
    }
    public HandIK LeftHandIK,RightHandIK;
    
    public void Do()
    {

    }
    private void LateUpdate()
    {
        WeaponRoot.position = ItemPorxy.TransformPoint(weaponRootFixedVector * mul);

        cameraParent.position = CamProxy.TransformPoint(camRootFixedVector * camMul);
        //LeftHandIK.DoHandIK();
        //RightHandIK.DoHandIK();
        //LeftHandIK.leafBone.position = LeftHandIK.Target.position;
        //RightHandIK.leafBone.position = RightHandIK.Target.position;
        IK.LookTarget.position = cameraParent.forward * 5f + cameraParent.position;


    }

    void MouseControl()
    { 
        x = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        y += Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        y = Mathf.Clamp(y, -75f, 75f);
        cameraParent.localRotation = Quaternion.Euler(-y, 0, 0);
        YawRotator.Rotate(Vector3.up * x);
    }
    void KeyBoardControl()
    {
        float forward = Input.GetAxisRaw("Vertical");
        float side = Input.GetAxisRaw("Horizontal");
        IK.anim.SetFloat("Forward", forward);
        IK.anim.SetFloat("Side", side);
        Vector3 moveVector = forward * IK.transform.forward + side * IK.transform.right;
        Vector3 dir = moveVector.normalized;
        move = new Vector3(transform.position.x, IK.gravity , transform.position.z) + dir * moveSpeed * Time.deltaTime;
        return;
        rb.velocity = dir * moveSpeed;
        rb.MovePosition(
            new Vector3(transform.position.x, IK.gravity
             , transform.position.z)
            + dir * moveSpeed * Time.deltaTime);
    }
}
