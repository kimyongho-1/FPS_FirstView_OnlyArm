using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandIK : MonoBehaviour
{
    public Transform LeftHandTarget, RightHandTarget;
    Animator armAnim;
    private void Awake()
    {
        armAnim = GetComponent<Animator>();
    }
    private void OnAnimatorIK(int layerIndex)
    {
        armAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        armAnim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
        armAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        armAnim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);

        armAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        armAnim.SetIKPosition(AvatarIKGoal.RightHand,RightHandTarget.position);
        armAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        armAnim.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
    }
}
