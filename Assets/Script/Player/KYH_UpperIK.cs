using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class KYH_UpperIK : MonoBehaviour
{
    public KYH_WeaponBobbing bob;
    Animator anim; 
    public Transform LHtarget, RHtarget, LElbow, RElbow;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (bob)
        {
            bob.WeaponBobbing();
        }
        HandIK();
    }
    void HandIK()
    {
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LElbow.position);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, RElbow.position);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, LHtarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightHand, RHtarget.rotation);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, LHtarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, RHtarget.position);
    }
}
