using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSpin : MonoBehaviour
{
    public float power= 15f;
    public Transform YawRotator, target;
    public float addRotate;
    Animator anim;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        anim = GetComponentInChildren<Animator>();
    }
    private void Update()
    {
        // 현재 회전진행중인지
        if (anim.GetBool("PelvisRotating") == false)
        {
            sideRotating += Input.GetAxis("Horizontal") * power;
            nextA = sideRotating;
           // anim.SetFloat("y", sideRotating);
            // 회전 해야하는 기준
            if (anim.GetFloat("y") > 44.9f) //YawRotator.localRotation.eulerAngles.y
            {
                anim.SetBool("PelvisRotating", true);
                anim.PlayInFixedTime("RightRotatePose", 1);
                
            }
            else if (anim.GetFloat("y") < -44.9f)
            {
                anim.SetBool("PelvisRotating", true);
                anim.PlayInFixedTime("LeftRotatePose", 1);

            }
        }
    }
    public float nextA;
    public float sideRotating;
    private void LateUpdate()
    {
        anim.SetFloat("y",nextA);
    }
    
}
