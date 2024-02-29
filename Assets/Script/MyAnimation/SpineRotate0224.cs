using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpineRotate0224 : MonoBehaviour
{
    OtherPlayerController PC; InputReceiver myInput;
    public Transform  GunPivot,GunDirection;
    public Transform Look { get { return PC.Look; } }
    public Transform YawRotater { get { return PC.YawRotator; } }
    public Transform PitchRotator { get { return PC.PitchRotator; } }

    #region AnimRigging Data
    public AnimationCurve spineWeightRatio, gunPositionOffsetsX, gunPositionOffsetsY;
    public MultiAimConstraint spineConstraint, upperChestConstraint, headConstraint;
    public List<SpineOffsetData> offsetData;
    Dictionary<HumanBodyBones, SpineOffsetData> offsetDictionary;
    #endregion

    public bool UseGizmo;
    [HideInInspector]public Animator anim;
    
    public void Init(OtherPlayerController p, InputReceiver pi)
    {
        anim = GetComponent<Animator>();
        p.Model = anim;
        PC = p;
        myInput = pi;

        offsetDictionary = new Dictionary<HumanBodyBones, SpineOffsetData>();
        offsetDictionary.Add(HumanBodyBones.Spine, offsetData.Find(x=>x.boneType == HumanBodyBones.Spine));
        offsetDictionary.Add(HumanBodyBones.UpperChest, offsetData.Find(x => x.boneType == HumanBodyBones.UpperChest));
        offsetDictionary.Add(HumanBodyBones.Head, offsetData.Find(x => x.boneType == HumanBodyBones.Head));
        LF_IK.Init(anim);
        RF_IK.Init(anim);

        footIKfunc = new Dictionary<string, Action<FootIKData>>();
        footIKfunc.Add("SlopedPlane", OnSlopedPlane);
        footIKfunc.Add("Stairs", OnStairs);
    }
    public void ModelUpdate()
    {
        spineConstraint.weight = spineWeightRatio.Evaluate(myInput.pitchVal);
        spineConstraint.data.offset = new Vector3(offsetDictionary[HumanBodyBones.Spine].pitchOffsets.Evaluate(myInput.pitchVal),
            spineConstraint.data.offset.y, spineConstraint.data.offset.z);

        upperChestConstraint.data.offset = new Vector3(offsetDictionary[HumanBodyBones.UpperChest].pitchOffsets.Evaluate(myInput.pitchVal),
            upperChestConstraint.data.offset.y, upperChestConstraint.data.offset.z);

        headConstraint.data.offset = new Vector3(headConstraint.data.offset.x, headConstraint.data.offset.y,
            offsetDictionary[HumanBodyBones.Head].pitchOffsets.Evaluate(myInput.pitchVal));


        // TO DO : IDLE�����ϋ��� �ϵ��� ������Ʈ�ӽ�.CS�� �ű� ����
       //if (anim.GetBool("PelvisRotating") == false)
       //{
       //    float angle = Vector3.SignedAngle(transform.forward, YawRotater.forward, Vector3.up);
       //    //Debug.Log(angle);
       //    if (angle > 45f)
       //    {
       //        anim.PlayInFixedTime("RightRotate", 1);
       //    }
       //    else if (angle < -35f)
       //    {
       //        anim.PlayInFixedTime("LeftRotate", 1);
       //    }
       //}
    }

    #region OnAnimatorIK()

    #region HandIK VARIABLE
    public AnimationCurve gunRotateOffset;
    public Transform LT, RT, LE, RE;
    #endregion

    #region FootIK VARIABLE
    public FootIKData LF_IK, RF_IK;
    public LayerMask groundLayer;
    Dictionary<string, Action<FootIKData>> footIKfunc;
    public float add = 0.2f;
    void OnSlopedPlane(FootIKData ikData)
    {
       //anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(ikData.propertyName));
       //anim.SetIKPosition(AvatarIKGoal.RightFoot, ikData.hitPoint+ Vector3.up * anim.rightFeetBottomHeight);
        //new Vector3(0, RF_heightOffset.Evaluate(currRatio), 0)
    }
    void OnStairs(FootIKData ikData)
    {
       // float animHeight = anim.GetBoneTransform(HumanBodyBones.RightFoot).localPosition.y;
       // float y = Mathf.Lerp(ikData.hitDist + animHeight, ikData.hitPoint.y + anim.rightFeetBottomHeight, anim.GetFloat(ikData.propertyName));
       // anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(ikData.propertyName));
       // anim.SetIKPosition(AvatarIKGoal.RightFoot,
       //     new Vector3(ikData.hitCollider.transform.position.x,
       //      y,
       //      ikData.hitCollider.transform.position.z));

    }
    #endregion
    private void OnAnimatorIK(int layerIndex)
    {
        const int UPPER = 2;
        const int LEGS = 1;
        switch (layerIndex)
        {
            case LEGS: FootIK(); return;
            case UPPER: HandIK(); return;
            default: HandIK(); return;
        }
        void FootIK()
        {
          
            SetFootIK(LF_IK);
            SetFootIK(RF_IK);
            void SetFootIK(FootIKData fid)
            { 
                // ���� ����ĳ��Ʈ
                fid.CheckRay(groundLayer);

                Nullable<RaycastHit> hit = fid.GetHit();
                if (hit != null)
                {
                    RaycastHit hits = hit.Value;
                    float weight = fid.GetWeight;
                    // ĳ���� ��Ʈ������ (0����)�� �������� ������� ���� ���� ��ġ�� ã�� : �ִϸ��̼��� ���� ����� ���� ��ġ
                    Vector3 footPos = anim.GetBoneTransform(fid.bodyPart).position;
                    // Height Offset ã�� : weight�� 1�̸� ���鿡 �پ���ϱ⿡ offset�� �⺻���� ����
                    float height =  Mathf.Lerp(add+ fid.heightOffset, fid.heightOffset, weight);
               
                    // ���� �������� ���� �󸶳� �������־���ϴ��� ����
                    Vector3 newFootPos = new Vector3(footPos.x, hits.point.y+ height, footPos.z);
                    anim.SetIKPositionWeight(fid.bodyPartGoal, 1f); // weight�� ���� 1�� ����
                    anim.SetIKPosition(fid.bodyPartGoal, newFootPos);

                    // rotation
                    anim.SetIKRotationWeight(fid.bodyPartGoal, weight);// weight
                    Vector3 rotAxis = Vector3.Cross(Vector3.up, hits.normal);
                    float angle = Vector3.Angle(Vector3.up, hits.normal);
                    Quaternion rot = Quaternion.AngleAxis(angle, rotAxis);
                    anim.SetIKRotation(fid.bodyPartGoal,  rot * anim.GetIKRotation(fid.bodyPartGoal));
                }

            }
         
        }

        void HandIK()
        {
            //GunDirection.localPosition = new Vector3(gunPositionOffsetsX.Evaluate(myInput.pitchVal), gunPositionOffsetsY.Evaluate(myInput.pitchVal), 0);
            //GunDirection.LookAt(Look.position);
            #region HAND_IK
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKRotation(AvatarIKGoal.RightHand, RT.rotation);
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, RT.position);

            anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
            anim.SetIKHintPosition(AvatarIKHint.RightElbow, RE.position);

            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, LT.rotation);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, LT.position);

            anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
            anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LE.position);
            #endregion
        }
    }
    #endregion



    private void OnDrawGizmos()
    {

        if (UseGizmo)
        {
            Gizmos.color = Color.red;
            Transform bone = GunDirection;
            Gizmos.DrawLine(bone.position, bone.position+ bone.forward * 40f);
            return;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, Vector3.up * 10f);
            Gizmos.DrawRay(transform.position, Vector3.up * 10f);

            Gizmos.color = Color.red;
             bone = spineConstraint.data.constrainedObject;
            Gizmos.DrawLine(bone.position, bone.forward * 20f);

            Gizmos.color = Color.green;
            bone = upperChestConstraint.data.constrainedObject;
            Gizmos.DrawLine(bone.position, bone.forward * 20f);

            Gizmos.color = Color.blue;
            bone = headConstraint.data.constrainedObject;
            Gizmos.DrawLine(bone.position, bone.forward * 20f);

        }
    }
}
