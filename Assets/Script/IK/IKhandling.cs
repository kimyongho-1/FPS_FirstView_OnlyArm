using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKhandling : MonoBehaviour
{
    Animator anim;

    public float leftFootWeight = 1;
    public float rightFootWeight = 1;
    Vector3 leftFootPos, rightFootPos;
    Quaternion leftFootRot, rightFootRot;
    Transform leftFoot,rightFoot;



    public float ikWeight = 1;
    public Transform leftIKTarget;
    public Transform rightIKTarget;
    public Transform leftHint;
    public Transform rightHint;

    public Vector3 offsets;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        leftFootRot = leftFoot.rotation;
        rightFootRot = rightFoot.rotation;  
    }
    private void Update()
    {
        RaycastHit lefthit,righthit;

        Vector3 lpos = leftFoot.position;// ������ǥ�� ��ȯ���� ��ġ�� ���ϱ� (���̽�����ġ�� ����)
        Vector3 rpos = rightFoot.position;
        Debug.DrawRay(lpos, -Vector3.up * 0.2f, Color.magenta);
        if (Physics.Raycast(lpos, -Vector3.up * 0.2f, out lefthit, 1))
        {
            leftFootPos = lefthit.point + offsets;
            Debug.Log(lefthit.transform.name);
            // ���̿� �ɸ�, �ٴ� ������Ʈ�� ǥ����� �븻���͹�������
            // ĳ���� ��ü ���� �����͹������� ȸ������ ���ϱ�
            leftFootRot = Quaternion.FromToRotation(transform.up, lefthit.normal)
                //���� ȸ������ŭ �߸�ȸ�� �ʱ�ȭ 
                * transform.rotation; // ��üȸ���� �߽�����(��ü�� ȸ���� �ٲ�°�� ���)

        }
        if (Physics.Raycast(rpos, -Vector3.up * 0.2f, out righthit, 1))
        {
            rightFootPos = righthit.point + offsets;
            rightFootRot = Quaternion.FromToRotation(transform.up, righthit.normal) * transform.rotation;
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

        leftFootWeight = anim.GetFloat("LeftFoot");
        rightFootWeight = anim.GetFloat("RightFoot");

       // anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootWeight);
       // anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
       // anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftHint.position);
       // anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightHint.position);
        
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootRot);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootRot);
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPos);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPos);

    }

}
