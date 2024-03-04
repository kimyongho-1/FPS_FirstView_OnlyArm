using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Windows;
using static UnityEngine.UI.GridLayoutGroup;

// 1인칭 | FPS 암 모델의 절차적 애니메이션 진행
public class CharacterSpineRotate : SpineRotate
{
    public FpsArmSpringTransform fast;

    #region AnimRigging Data
    public MultiAimConstraint spineConstraint, upperChestConstraint, headConstraint;
    public MultiPositionConstraint gunPosConstraint;
    public List<SpineOffsetData> offsetData;
    Dictionary<HumanBodyBones, SpineOffsetData> offsetDictionary;
    #endregion

    #region OnAnimatorIK()


    #region HandIK VARIABLE
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
                // 다중 레이캐스트
                fid.CheckRay(groundLayer);

                Nullable<RaycastHit> hit = fid.GetHit();
                if (hit != null)
                {
                    RaycastHit hits = hit.Value;
                    float weight = fid.GetWeight;
                    // 캐릭터 루트포지션 (0벡터)를 기준으로 상대적인 현재 발의 위치값 찾기 : 애니메이션이 현재 적용된 발의 위치
                    Vector3 footPos = anim.GetBoneTransform(fid.bodyPart).position;
                    // Height Offset 찾기 : weight가 1이면 지면에 붙어야하기에 offset은 기본으로 고정
                    float height = Mathf.Lerp(add + fid.heightOffset, fid.heightOffset, weight);

                    // 발이 지면으로 부터 얼마나 떨어져있어야하는지 적용
                    Vector3 newFootPos = new Vector3(footPos.x, hits.point.y + height, footPos.z);
                    anim.SetIKPositionWeight(fid.bodyPartGoal, 1f); // weight를 강제 1로 고정
                    anim.SetIKPosition(fid.bodyPartGoal, newFootPos);

                    // rotation
                    anim.SetIKRotationWeight(fid.bodyPartGoal, weight);// weight
                    Vector3 rotAxis = Vector3.Cross(Vector3.up, hits.normal);
                    float angle = Vector3.Angle(Vector3.up, hits.normal);
                    Quaternion rot = Quaternion.AngleAxis(angle, rotAxis);
                    anim.SetIKRotation(fid.bodyPartGoal, rot * anim.GetIKRotation(fid.bodyPartGoal));
                }

            }

        }

        void HandIK()
        {
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


    public override void Init(MyBaseController p, InputReceiver pi)
    {
        anim = GetComponent<Animator>();
        p.Model = anim;
        PC = p;
        myInput = pi;

        offsetDictionary = new Dictionary<HumanBodyBones, SpineOffsetData>();
        offsetDictionary.Add(HumanBodyBones.Spine, offsetData.Find(x => x.boneType == HumanBodyBones.Spine));
        offsetDictionary.Add(HumanBodyBones.UpperChest, offsetData.Find(x => x.boneType == HumanBodyBones.UpperChest));
        offsetDictionary.Add(HumanBodyBones.Head, offsetData.Find(x => x.boneType == HumanBodyBones.Head));
        LF_IK.Init(anim);
        RF_IK.Init(anim);

        footIKfunc = new Dictionary<string, Action<FootIKData>>();
        footIKfunc.Add("SlopedPlane", OnSlopedPlane);
        footIKfunc.Add("Stairs", OnStairs);
    }

    public AnimationCurve upperWheelRatio;
    public override void ModelUpdate()
    {
        // 0 ~ 1의 값으로 평균화하기
        float nomalizedRatio = (((myInput.pitchVal) / -85f) + 1f) * 0.5f;
        Debug.Log(nomalizedRatio);
        fast.TransformRotate(nomalizedRatio); 
        spineConstraint.weight = upperWheelRatio.Evaluate(nomalizedRatio);
        
        
    }
    
 
}
