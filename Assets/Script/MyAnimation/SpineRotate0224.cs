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
            // Test �ϱ�
            // 1. weight < 1 : ���� ����� ������ ���� ��¡�� ����� �׸�ŭ ���� ����
            // => �ڽ����̿� �ɸ��� �ſ� ������ �Ÿ��̹Ƿ�,  ���� ������ŭ �ڷ� �о���Ѵ� (��� �ұ�)
            // otherwise ? => �Ȱ��� �Ϲ� ���������� �߻��ؼ� ã�Ƴ� �浹ü�� �븻���⸸ŭ �ڷ� �б�?
            if (Physics.BoxCast(RF_IK.rayOrigin.transform.position + (RF_IK.rayOrigin.up * 0.01f),
                new Vector3(0.05f, 0.01f, 0.125f), RF_IK.rayOrigin.up, RF_IK.rayOrigin.transform.rotation , 0.3f))
            { 
                
            }


            // 2. weight > 0.9 : ���� ���� ��� ���� 
            // => �Ϲ� ���� �߻��Ͽ� Ȯ��
            Debug.DrawRay(RF_IK.rayOrigin.transform.position + (RF_IK.rayOrigin.up * 0.01F), RF_IK.rayOrigin.up * 2f, Color.red);
            if (Physics.Raycast(RF_IK.rayOrigin.transform.position + (-RF_IK.rayOrigin.up * rayPosOffset) 
                , RF_IK.rayOrigin.up, out RaycastHit hit , 2f, groundLayer))
            {
                Transform rf = anim.GetBoneTransform(HumanBodyBones.RightFoot);   
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
        Vector3 rayOriginPosition = RF_IK.rayOrigin.transform.position+ (RF_IK.rayOrigin.up * 0.01f);
        Vector3 downDirection = RF_IK.rayOrigin.up;
        Vector3 boxCenter = rayOriginPosition ;
        Vector3 halfExtents = new Vector3(0.05f, 0.01f, 0.125f);
        Quaternion orientation = RF_IK.rayOrigin.transform.rotation; // ���� ��ü�� ȸ���� �ݿ�
        float maxDistance = 0.3f;

        // ������ ���� ��������� ��ġ�� ����׷� �׸���
        Gizmos.color = Color.red;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, orientation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
        if (RF_IK.drawGizmo)
        {
          //  RF_IK.OnDrawGizmos(rayPosOffset, rayDistance);
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
