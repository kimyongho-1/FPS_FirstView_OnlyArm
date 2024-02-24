using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UIElements;

public class CharRot : MonoBehaviour
{
    public Transform LookY, LookX;
    public DirPivot LeftTarget, RightTarget, ForwardTarget;
    public Transform YawRotater, PitchRotater;
    public Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnDrawGizmos()
    {
        if (anim == null) { anim = GetComponent<Animator>(); }
        Gizmos.color = Color.blue;
        Transform head = anim.GetBoneTransform(HumanBodyBones.Head);
        Gizmos.DrawRay(head.position, head.forward * 100f);
        return;
        Transform upper = anim.GetBoneTransform(HumanBodyBones.UpperChest);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(upper.position, upper.forward * 100f);

        
        
    }

    public float yaws,pitchs;
    private void Update()
    {
        return;
        yaws += Input.GetAxis("Horizontal");
        pitchs = Mathf.Clamp(pitchs + Input.GetAxis("Vertical"), -80f, 80f);
        YawRotater.localRotation = Quaternion.Euler(0, yaws ,0);
        PitchRotater.localRotation = Quaternion.Euler(0, pitchs , 0);
    }

    public float clamps;
    public float bodys;
    public AnimationCurve bodyWeight; 
    public float animationStartTime;
    private float animationDuration = 0.5f;
    public Quaternion startYrot, destYrot;
    public float dirY;
    public RootSpin RS;
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim.GetBool("PelvisRotating"))
        {
            float timeSinceStarted = Time.time - animationStartTime;
            float percentageComplete = timeSinceStarted / animationDuration;
            RS.addRotate += Input.GetAxis("Horizontal") * RS.power;
            RS.nextA = Mathf.Lerp(dirY, 0, percentageComplete) + RS.addRotate;
            Quaternion q = Quaternion.Slerp(startYrot, destYrot, percentageComplete);// 
            anim.bodyRotation = q;
        }
        
     //float angleX = PitchRotater.localRotation.eulerAngles.x;
     //if (angleX > 180f)
     //{
     //    angleX -= 360.0f;
     //}
     ////bodys = bodyWeight.Evaluate(angleX / 90f);
     //// Vector3 test = Vector3.Slerp(LookX.position, LookY.position, 0.5f);

     //xRatio = ((-angleX / 90f) + 1) * 0.5f;
     //bodys = bodyWeight.Evaluate(xRatio);
     //anim.SetLookAtWeight(1f, bodys, 1f, 1f, clamps);
     //anim.SetLookAtPosition(LookX.position);
     //yRatio = YawRotater.localRotation.eulerAngles.y / 90f;
     //return;
     return ;
         float
        angleX = PitchRotater.localRotation.eulerAngles.x;
        if (angleX > 180f)
        {
            angleX -= 360.0f;
        }
        angleX = ((-angleX / 90f) + 1) * 0.5f;
        float angleY = YawRotater.localRotation.eulerAngles.y / 90f;
        Debug.Log($"X : {angleX} , Y : {angleY}");
        bodys = bodyWeight.Evaluate(angleX / 90f);
        anim.SetLookAtWeight(1f, bodys, 1f, 1f, clamps);
        return;
        // 정면일떄의 Pitch비율에 맞는 룩타겟 위치 찾기
        Vector3 ForwardPosition = ForwardTarget.GetHeight(angleX);
        // 좌우 회전 없을시, 위 벡터를 룩타겟으로 고정
        if (angleY == 0) { anim.SetLookAtPosition(ForwardPosition); return; }

        // 좌우 회전 존재시 방향 찾기
        DirPivot sideDirection = (angleY > 0f) ? RightTarget : LeftTarget ;
       
        // Pitch회전 비율에 맞는 위치 찾기
        Vector3 Pose = sideDirection.GetHeight(angleX);
        // 최종 룩타겟 위치 구하기
        Vector3 pos = Vector3.Lerp(ForwardPosition, Pose, angleY);

        anim.SetLookAtPosition(pos); 
   
    }
    [Range(0, 1)] public float ratio = 1f;

    public float xRatio, yRatio;
    Vector3 offsets1,offsets2, offsets3;
    // 오른쪽 기준
    public AnimationCurve valX, valY, valZ;
    public AnimationCurve ChestOffsetX, ChestOffsetY, ChestOffsetZ;
    public AnimationCurve UpperChestOffsetX, UpperChestOffsetY, UpperChestOffsetZ;

    // 왼쪽쪽 기준
    [Header("왼쪽 Pitch")]
    public AnimationCurve SpineLOffsetX, SpineLOffsetY, SpineLOffsetZ;
    public AnimationCurve ChestLOffsetX, ChestLOffsetY, ChestLOffsetZ;
    public AnimationCurve UpperChestLOffsetX, UpperChestLOffsetY, UpperChestLOffsetZ;
    private void LateUpdate()
    {
        return;
        Transform upper = anim.GetBoneTransform(HumanBodyBones.Spine);
        Quaternion curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(SpineLOffsetX.Evaluate(xRatio), SpineLOffsetY.Evaluate(xRatio), SpineLOffsetZ.Evaluate(xRatio));

        upper = anim.GetBoneTransform(HumanBodyBones.Chest);
        curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(ChestLOffsetX.Evaluate(xRatio), ChestLOffsetY.Evaluate(xRatio), ChestLOffsetZ.Evaluate(xRatio)) ;

        upper = anim.GetBoneTransform(HumanBodyBones.UpperChest);
        curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(UpperChestLOffsetX.Evaluate(xRatio), UpperChestLOffsetY.Evaluate(xRatio), UpperChestLOffsetZ.Evaluate(xRatio));

        return;
        upper = anim.GetBoneTransform(HumanBodyBones.Spine);
        curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(valX.Evaluate(xRatio), valY.Evaluate(xRatio), valZ.Evaluate(xRatio));

        upper = anim.GetBoneTransform(HumanBodyBones.Chest);
        curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(ChestOffsetX.Evaluate(xRatio), ChestOffsetY.Evaluate(xRatio), ChestOffsetZ.Evaluate(xRatio));

        upper = anim.GetBoneTransform(HumanBodyBones.UpperChest);
        curr = upper.rotation;
        upper.rotation = curr * Quaternion.Euler(UpperChestOffsetX.Evaluate(xRatio), UpperChestOffsetY.Evaluate(xRatio), UpperChestOffsetZ.Evaluate(xRatio));
        //upper.rotation = Quaternion.Euler(0, YawRotater.localRotation.eulerAngles.y ,0);
    }
}
