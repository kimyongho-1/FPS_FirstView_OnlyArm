using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class IKhandling : MonoBehaviour
{   
    public MainPlayer player;
    public Animator anim;

    Vector3 leftFootIKpos, rightFootIKpos;
    Quaternion leftFootIKrot, rightFootIKrot;
    public Transform leftFootRayOrigin, rightFootRayOrigin;
    Transform leftFoot, rightFoot;
    public float animSpeed = 1f;

    public Transform leftIKTarget;
    public Transform rightIKTarget;
    public Transform leftHint;
    public Transform rightHint;
    public float GetLeftFootWeight{get{return anim.GetFloat("LeftFootWeight");}}
    public float offsets;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
    }
    

    #region Test
    public float LeftFootRayLen = 0.35f;

    
    #endregion
    private void FixedUpdate()
    {
       
    }
    public float gravity ;
    private void Update()
    {
        anim.speed = animSpeed;   
    }

    public Transform LeftHandElbow, RightHandElbow;
    public Transform LeftHandIKTarget, RightHandIKTarget;
    void Hand()
    { 
         anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
         anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
         anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftHandElbow.position);
         anim.SetIKHintPosition(AvatarIKHint.RightElbow, RightHandElbow.position);
    
         anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
         anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
         anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
         anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
         anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
         anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
         anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
         anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        LookAt();
        Hand();

        gravity = 0;
        bool isAir = false;
        float leftFootWeight = anim.GetFloat("LeftFootWeight");
        float rightFootWeight = anim.GetFloat("RightFootWeight");
        if (Physics.Raycast(leftFoot.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + LeftFootRayLen))
        {
            leftFootIKpos = hit.point;
            gravity = hit.point.y * 0.5f;
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit.normal);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * leftFootWeight, rotAxis);


            leftFootIKrot = rot;
            // Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else 
        {
            isAir = true;
        }
        
        //anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootWeight);
        //anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftHint.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIKrot * anim.GetIKRotation(AvatarIKGoal.LeftFoot)); 
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKpos + Vector3.up * offsets);

        if (Physics.Raycast(rightFoot.position + Vector3.up, -Vector3.up, out RaycastHit hit2, 1f + LeftFootRayLen))
        {
            rightFootIKpos = hit2.point;
            gravity += hit2.point.y * 0.5f;
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit2.normal);
            float angle = Vector3.Angle(Vector3.up, hit2.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * rightFootWeight, rotAxis);
            rightFootIKrot = rot;
            //rightFootIKrot = Quaternion.FromToRotation(transform.up, hit2.normal) * transform.rotation;
        }
        else
        {
            if (isAir == true)
            {
                gravity -= Physics.gravity.y * -Time.deltaTime;
            }
            else { gravity *= 2f; }
        }
        //anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
        //anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightHint.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIKrot * anim.GetIKRotation(AvatarIKGoal.RightFoot));
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKpos + Vector3.up * offsets);

        
    }

    private void LateUpdate()
    {
       
    }

}
