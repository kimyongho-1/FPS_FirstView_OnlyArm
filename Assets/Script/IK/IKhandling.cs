using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKhandling : MonoBehaviour
{
    Animator anim;

    public float leftFootWeight = 1;
    public float rightFootWeight = 1;

    Vector3 leftFootIKpos, rightFootIKpos;
    Quaternion leftFootIKrot, rightFootIKrot;
    bool leftFootOnGround, rightFootOnGround;
    public Transform leftFootRayOrigin, rightFootRayOrigin;
    public Transform leftIdleOrigin, rightIdleOrigin;
    Transform leftFoot,rightFoot;
    public float animSpeed = 1f;

    public float moveSpeed = 0.2f;
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
            leftFootOnGround = true;
            leftFootIKpos = hit.point;
            leftFootIKrot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else 
        {
            // 바닥이 없다면? 
            leftFootOnGround = false;
        }
    }
    public void CheckRightFeetUnderGround()
    {
        if (Physics.Raycast(rightFootRayOrigin.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
        {
            rightFootOnGround = true;
            rightFootIKpos = hit.point;
            rightFootIKrot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {
            // 바닥이 없다면? 
            rightFootOnGround = false;
        }
    }
    public void FeetsUpdate()
    {
        CheckLeftFeetUnderGround();
        CheckRightFeetUnderGround();
    }

    public void InitializeIdle()
    {
        if (leftFootOnGround == true && leftFootWeight < 1 )
        { CheckLeftFeetUnderGround(); leftFootWeight = 1;  }
        if (rightFootOnGround == true && rightFootWeight < 1)
        { CheckRightFeetUnderGround(); rightFootWeight = 1; }
        return;
    }
  
    #endregion
    private void FixedUpdate()
    {
      
     
    }
    private void Update()
    {
        #region DEBUG
        if (leftFootOnGround ==true)
        { 
            leftFootIKpositionReuslt.position = leftFootIKpos+Vector3.up*offsets;
            Debug.DrawRay(leftFootRayOrigin.position + Vector3.up, -Vector3.up * (1f + LeftFootRayLen), Color.black, 0.1f);
        }
        if (rightFootOnGround == true)
        {
            rightFootIKpositionReuslt.position = rightFootIKpos + Vector3.up * offsets;
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
            rb.MovePosition(movementDir * inputMagnitude * Time.deltaTime * moveSpeed + transform.position);
            anim.SetFloat("Forward", forward);
            anim.SetFloat("Side", side);
            if (forward == 0 && side == 0)
            {
                return;
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

        if (Physics.Raycast(leftFootRayOrigin.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
        {
            leftFootOnGround = true;
            leftFootIKpos = hit.point;
            leftFootIKrot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (leftFootOnGround == true)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIKrot);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootIKpos + Vector3.up*offsets);
        }

        if (rightFootOnGround == true)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIKrot);
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
            anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKpos + Vector3.up * offsets);
        }
        

    }

}
