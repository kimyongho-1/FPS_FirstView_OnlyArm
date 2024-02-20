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
            // ���� ȸ������������
            if (anim.GetBool("PelvisRotating") == false)
            {
                // ȸ�� �ؾ��ϴ� ����
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
