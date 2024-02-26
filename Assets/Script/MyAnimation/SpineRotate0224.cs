using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class SpineRotate0224 : MonoBehaviour
{
    public Transform  GunPivot,GunDirection;
    public Transform Look, YawRotator, PitchRotator;
    public float pitchVal, yawVal, rotSpeed;

    #region AnimRigging Data
    public AnimationCurve spineWeightRatio, gunPositionOffsetsX, gunPositionOffsetsY;
    public MultiAimConstraint spineConstraint, upperChestConstraint, headConstraint;
    public List<SpineOffsetData> offsetData;
    Dictionary<HumanBodyBones, SpineOffsetData> offsetDictionary;
    #endregion

    public bool UseGizmo;
    Animator anim;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
        offsetDictionary = new Dictionary<HumanBodyBones, SpineOffsetData>();
        offsetDictionary.Add(HumanBodyBones.Spine, offsetData.Find(x=>x.boneType == HumanBodyBones.Spine));
        offsetDictionary.Add(HumanBodyBones.UpperChest, offsetData.Find(x => x.boneType == HumanBodyBones.UpperChest));
        offsetDictionary.Add(HumanBodyBones.Head, offsetData.Find(x => x.boneType == HumanBodyBones.Head));

        footIKfunc = new Dictionary<string, Action<FootIKData>>();
        footIKfunc.Add("SlopedPlane", OnSlopedPlane);
        footIKfunc.Add("Stairs", OnStairs);
    }
    public float moveSpeed;
    public void Update()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;

        yawVal += Input.GetAxis("Horizontal") * rotSpeed * Time.deltaTime;
        pitchVal = Mathf.Clamp(pitchVal - Input.GetAxis("Vertical") * rotSpeed * Time.deltaTime, -85f,85f);

        YawRotator.localRotation = Quaternion.AngleAxis(yawVal, Vector3.up);
        PitchRotator.localRotation =  Quaternion.AngleAxis(pitchVal, Vector3.right);

        spineConstraint.weight = spineWeightRatio.Evaluate(pitchVal);
        spineConstraint.data.offset = new Vector3(offsetDictionary[HumanBodyBones.Spine].pitchOffsets.Evaluate(pitchVal),
            spineConstraint.data.offset.y, spineConstraint.data.offset.z);

        upperChestConstraint.data.offset = new Vector3(offsetDictionary[HumanBodyBones.UpperChest].pitchOffsets.Evaluate(pitchVal),
            upperChestConstraint.data.offset.y, upperChestConstraint.data.offset.z);

        headConstraint.data.offset = new Vector3(headConstraint.data.offset.x, headConstraint.data.offset.y,
            offsetDictionary[HumanBodyBones.Head].pitchOffsets.Evaluate(pitchVal));


        if (anim.GetBool("PelvisRotating") == false)
        {
            float angle = Vector3.SignedAngle(transform.forward, YawRotator.forward, Vector3.up);
            //Debug.Log(angle);
            if (angle > 45f)
            {
                anim.PlayInFixedTime("RightRotate", 1);
            }
            else if (angle < -35f)
            {
                anim.PlayInFixedTime("LeftRotate", 1);
            }
        }


    }

    #region OnAnimatorIK()

    #region HandIK VARIABLE
    public AnimationCurve gunRotateOffset;
    public Transform LT, RT, LE, RE;
    #endregion

    #region FootIK VARIABLE
    public FootIKData LF_IK, RF_IK;
    public float rayPosOffset, rayDistance;
    public LayerMask groundLayer;
    public AnimationCurve RF_heightOffset; // 현재 안쓰이고, 어떻게 쓸지 고민중
    Dictionary<string, Action<FootIKData>> footIKfunc;


    void OnSlopedPlane(FootIKData ikData)
    {
        Debug.Log(anim.rightFeetBottomHeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(ikData.propertyName));
        anim.SetIKPosition(AvatarIKGoal.RightFoot, ikData.hitPoint+ Vector3.up * anim.rightFeetBottomHeight);
        //new Vector3(0, RF_heightOffset.Evaluate(currRatio), 0)
    }
    void OnStairs(FootIKData ikData)
    {
        float animHeight = anim.GetBoneTransform(HumanBodyBones.RightFoot).localPosition.y;
        float y = Mathf.Lerp(ikData.hitDist + animHeight, ikData.hitPoint.y + anim.rightFeetBottomHeight, anim.GetFloat(ikData.propertyName));
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat(ikData.propertyName));
        anim.SetIKPosition(AvatarIKGoal.RightFoot,
            new Vector3(ikData.hitCollider.transform.position.x,
             y,
             ikData.hitCollider.transform.position.z));

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
            default: return;
        }

        void FootIK()
        {
            float currRatio = anim.GetCurrentAnimatorStateInfo(1).normalizedTime % 1f;
            float weight = anim.GetFloat(RF_IK.propertyName);
            anim.bodyPosition = new Vector3(anim.bodyPosition.x,
                1f + RF_IK.hitPoint.y , anim.bodyPosition.z);

            //Debug.Log(currRatio);
            if (Physics.Raycast(RF_IK.rayOrigin.position + (Vector3.up * rayPosOffset), Vector3.down, out RaycastHit hit, rayDistance + weight + rayPosOffset, groundLayer))
            {
                if (weight == 1)
                {
                    footIKfunc[RF_IK.tag]?.Invoke(RF_IK);
                }
                else
                {// weight값이 0이라도, 현재 장애물이 가까울시 현재 애니메이션 발 높이만큼 띄우기
                    RF_IK.hitDist = hit.distance;
                    RF_IK.hitPoint = hit.point;
                    RF_IK.hitNormal = hit.normal;
                    RF_IK.hitCollider = hit.collider;
                    RF_IK.tag = hit.transform.gameObject.tag;

                    footIKfunc[hit.transform.gameObject.tag]?.Invoke(RF_IK);
                }
                
                
                anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, weight);
                Vector3 rotAxis = Vector3.Cross(anim.GetBoneTransform(HumanBodyBones.RightFoot).forward, hit.normal);
                float angle = Vector3.Angle(Vector3.up, hit.normal);

                Quaternion rot = Quaternion.AngleAxis(angle, rotAxis);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, rot * anim.GetIKRotation(AvatarIKGoal.RightFoot));
            }
            
        }

        void HandIK()
        {
            GunDirection.LookAt(Look.position);
            GunDirection.localPosition = new Vector3(gunPositionOffsetsX.Evaluate(pitchVal), gunPositionOffsetsY.Evaluate(pitchVal), 0);
          
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

    private void LateUpdate()
    {
    }


    public bool drawFootIK;
    private void OnDrawGizmos()
    {
        if (RF_IK.drawGizmo)
        {
            RF_IK.OnDrawGizmos(rayPosOffset, rayDistance);
        }

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
