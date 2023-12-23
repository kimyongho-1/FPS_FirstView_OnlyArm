using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTargetInfo
{
    public RayTargetInfo(RaycastHit hit, Quaternion rot)
    {
        normal = hit.normal;
        position = hit.point;
        rotation = rot;
    }
    Vector3 normal; public Vector3 GetNormal { get { return normal; } }
    Vector3 position; public Vector3 GetPosition { get { return position; } }
    Quaternion rotation; public Quaternion GetRotation { get { return rotation; } }
}
public class IKhandling : MonoBehaviour
{
    Animator anim;

    public float leftFootWeight = 1;
    public float rightFootWeight = 1;

    // RaycastHit 자체를 변수로 현재 바닥오브젝트를 참조하기엔
    // RayCastHit변수에 너무많은 불필요한 변수등을 전부 참조하게되기에
    // 필요한 정보만 기억하여 꺼내 쓰는, 커스텀RaycastTargetInfo클래스를 만들어사용
    RayTargetInfo leftFeetGroundInfo;
    RayTargetInfo rightFeetGroundInfo;
    public Transform leftFootRayOrigin, rightFootRayOrigin;
    public Transform leftIdleOrigin, rightIdleOrigin;
    Transform leftFoot,rightFoot;
    public float animSpeed = 1f;


    public float ikWeight = 1;
    public Transform leftIKTarget;
    public Transform rightIKTarget;
    public Transform leftHint;
    public Transform rightHint;

    public float offsets;
    private void Awake()
    {
        Application.targetFrameRate = 30;
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        rb = GetComponent<Rigidbody>();
    }
    Rigidbody rb;
    

    #region Test
    public float LeftFootRayLen = 0.35f;

    #region DEBUG use
    [Header("Debug 용")]
    public Transform leftFootIKpositionReuslt; public Transform rightFootIKpositionReuslt;
    #endregion

    public void CheckLeftFeetUnderGround()
    {
        if (Physics.Raycast(leftFootRayOrigin.position+Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
        {
            leftFeetGroundInfo = new RayTargetInfo(hit,Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
        }
        else 
        {
            // 바닥이 없다면? 
            leftFeetGroundInfo = null;
        }

    }
    public void CheckRightFeetUnderGround()
    {
        if (Physics.Raycast(rightFootRayOrigin.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
        {
            rightFeetGroundInfo = new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
        }
        else 
        {
            // 바닥이 없다면? 
            rightFeetGroundInfo = null;
        }

    }

    public void InitializeIdle()
    {
        leftFootWeight = rightFootWeight = 1;
        if (anim.GetFloat("Forward") != 0 || anim.GetFloat("Side") != 0)
        {
            leftFeetGroundInfo = rightFeetGroundInfo = null;
            return;
        }
        else if (leftFeetGroundInfo == null && rightFeetGroundInfo == null )
        {
            // 현재 바닥 정보 IDLE에 맞게 초기화
            if (Physics.Raycast(leftFootRayOrigin.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
            {
                // 바닥오브젝트(경사면이 존재할 경우대비)과 수평을 이루도록 Y축회전 + Idle상태의 발 회전값(기본값)
                leftFeetGroundInfo = new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
                //new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
            }
            if (Physics.Raycast(rightFootRayOrigin.position + Vector3.up, -Vector3.up, out hit, 1f + LeftFootRayLen))
            {
                // 바닥오브젝트(경사면이 존재할 경우대비)과 수평을 이루도록 Y축회전 + Idle상태의 발 회전값(기본값)
                rightFeetGroundInfo = new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
            }
            return;
        }
        return;
        if (anim.GetFloat("Forward") != 0 || anim.GetFloat("Side") != 0)
        {
            // 현재 바닥 정보 IDLE에 맞게 초기화
            if (Physics.Raycast(leftIdleOrigin.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
            {
                // 바닥오브젝트(경사면이 존재할 경우대비)과 수평을 이루도록 Y축회전 + Idle상태의 발 회전값(기본값)
                leftFeetGroundInfo = new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * leftIdleOrigin.rotation);
                //new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
            }
            if (Physics.Raycast(rightIdleOrigin.position + Vector3.up, -Vector3.up, out hit, 1f + LeftFootRayLen))
            {
                // 바닥오브젝트(경사면이 존재할 경우대비)과 수평을 이루도록 Y축회전 + Idle상태의 발 회전값(기본값)
                rightFeetGroundInfo = new RayTargetInfo(hit, Quaternion.FromToRotation(transform.up, hit.normal) * rightIdleOrigin.rotation);
            }
            leftFootWeight = rightFootWeight = 1;
            return;
        }

    }
  
    #endregion
    private void FixedUpdate()
    {
      
     
    }
    private void Update()
    {
        #region DEBUG
        if (leftFeetGroundInfo != null)
        { 
            leftFootIKpositionReuslt.position = leftFeetGroundInfo.GetPosition+Vector3.up*offsets;
            Debug.DrawRay(leftFootRayOrigin.position + Vector3.up, -Vector3.up * (1f + LeftFootRayLen), Color.black, 0.1f);
        }
        if (rightFeetGroundInfo != null)
        {
            rightFootIKpositionReuslt.position = rightFeetGroundInfo.GetPosition + Vector3.up * offsets;
            Debug.DrawRay(rightFootRayOrigin.position + Vector3.up, -Vector3.up * (1f + LeftFootRayLen), Color.black, 0.1f);
        }
        #endregion

        anim.speed = animSpeed;
        TestMove();
        void TestMove()
        {
            float forward = Input.GetAxis("Vertical");
            float side = Input.GetAxis("Horizontal");
            Vector3 movementDir = (transform.forward * forward + side * transform.right).normalized;
            float inputMagnitude = Mathf.Clamp01(Mathf.Abs(forward) + Mathf.Abs(side));
            rb.velocity = (movementDir * animSpeed * inputMagnitude);
            rb.MovePosition(movementDir * inputMagnitude * Time.deltaTime * animSpeed + transform.position);
            anim.SetFloat("Forward", forward);
            anim.SetFloat("Side", side);
            if (forward == 0 && side == 0)
            {
                rb.velocity = Vector3.zero;
                InitializeIdle();
                
            }
        }
       
    }
    private void OnAnimatorIK(int layerIndex)
    {
        // anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ikWeight);
        // anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ikWeight);
        // anim.SetIKHintPosition(AvatarIKHint.LeftElbow, leftHint.position);
        // anim.SetIKHintPosition(AvatarIKHint.RightElbow, rightHint.position);
        //
        // anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, ikWeight);
        // anim.SetIKPositionWeight(AvatarIKGoal.RightHand, ikWeight);
        // anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, ikWeight);
        // anim.SetIKRotationWeight(AvatarIKGoal.RightHand, ikWeight);
        // anim.SetIKRotation(AvatarIKGoal.LeftHand, leftIKTarget.rotation);
        // anim.SetIKRotation(AvatarIKGoal.RightHand, rightIKTarget.rotation);
        // anim.SetIKPosition(AvatarIKGoal.LeftHand, leftIKTarget.position);
        // anim.SetIKPosition(AvatarIKGoal.RightHand, rightIKTarget.position);

        //leftFootWeight = anim.GetFloat("LeftFoot");
        //rightFootWeight = anim.GetFloat("RightFoot");

        // anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootWeight);
        // anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
        // anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftHint.position);
        // anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightHint.position);



        if (leftFeetGroundInfo != null)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFeetGroundInfo.GetRotation);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFeetGroundInfo.GetPosition + Vector3.up*offsets);
        }
        else 
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 0);
        }

        
        if (rightFeetGroundInfo != null)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFeetGroundInfo.GetRotation);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFeetGroundInfo.GetPosition + Vector3.up * offsets);
        }
        else 
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0);
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 0);
        }
        

    }

}
