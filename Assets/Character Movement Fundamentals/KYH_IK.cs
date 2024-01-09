using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_IK : MonoBehaviour
{
    
    public KYH_Mover mover;
    Animator anim;

    #region HAND
    public Transform LHtarget, RHtarget, LElbow, RElbow;
    #endregion
    #region FOOT
    float leftLegLength, rightLegLength;
    Transform LeftFoot, RightFoot;
    public float feetRayOffset = 0.1f; // 발에서 아래로 쏠 레이 추가 길이
    public float feetOffset = 0.07f; // 발이 지면에 붙을시, 떨어트릴 오차( V3.up 방향)
    public Transform  LKnee, Rknee;
    Vector3 lfPos, rfPos;
    Quaternion lfRot, rfRot;
    #endregion
    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        RightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        LeftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
    }
    private void Update()
    {
        Debug.DrawRay(LeftFoot.position + Vector3.up * 0.2f, Vector3.down* ( mover.stepLength + feetRayOffset ), Color.blue);
        Debug.DrawRay(RightFoot.position + Vector3.up * 0.2f, Vector3.down * (mover.stepLength + feetRayOffset), Color.blue);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        LookIK();
        HandIK();
        FootIK();
    }

    void LookIK() { }
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
    void FootIK()
    {
        float leftFootWeight = anim.GetFloat("LeftFootWeight"); // 애니메이터 파라미터는 애니메이션클립 내부에서 조정중
        if (leftFootWeight > 0f &&
            Physics.Raycast(LeftFoot.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, mover.stepLength + feetRayOffset))
        {
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit.normal);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * leftFootWeight, rotAxis); // 회전해야할 회전량
            lfRot = rot;
            lfPos = hit.point;
            anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootWeight);
            anim.SetIKHintPosition(AvatarIKHint.LeftKnee, LKnee.position);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, lfRot * anim.GetIKRotation(AvatarIKGoal.LeftFoot)); // 경사로가 존재시 그에 맞게 회전량을 더하기
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, lfPos + Vector3.up * feetOffset);

        }

        float rightFootWeight = anim.GetFloat("RightFootWeight"); // 애니메이터 파라미터는 애니메이션클립 내부에서 조정중
        if (rightFootWeight > 0f &&
            Physics.Raycast(RightFoot.position + Vector3.up * 0.2f, Vector3.down, out hit, mover.stepLength + feetRayOffset))
        {
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit.normal);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * leftFootWeight, rotAxis);
            rfRot = rot;
            rfPos = hit.point;
            anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
            anim.SetIKHintPosition(AvatarIKHint.RightKnee, Rknee.position);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, leftFootWeight);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rfRot * anim.GetIKRotation(AvatarIKGoal.RightFoot));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rfPos + Vector3.up * feetOffset);
        }

    }
}
