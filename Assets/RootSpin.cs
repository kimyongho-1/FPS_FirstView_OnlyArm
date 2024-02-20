using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootSpin : MonoBehaviour
{
    public float power= 15f;
    public Transform YawRotator, target;
    Animator anim;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        float sideRotating = Input.GetAxis("Horizontal") * power;
        if (sideRotating != 0)
        {
            YawRotator.localRotation *= Quaternion.Euler(0, sideRotating,0);
            // 현재 회전진행중인지
            if (anim.GetBool("PelvisRotating") == false)
            {
                // 회전 해야하는 기준
                if (YawRotator.localRotation.eulerAngles.y > 45f)
                {
                    anim.SetBool("PelvisRotating",true);
                    anim.PlayInFixedTime("RightRotatePose", 2);
                }
                else if (YawRotator.localRotation.eulerAngles.y < -45f)
                {
                    anim.SetBool("PelvisRotating", true);
                    anim.PlayInFixedTime("LeftRotatePose", 2);
                }
            }
        }

    }
    private void LateUpdate()
    {

        anim.SetFloat("y", YawRotator.localRotation.eulerAngles.y);// - transform.rotation.eulerAngles.y
    }
    
}
