using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpineRotate : MonoBehaviour
{
#if UNITY_EDITOR
    public bool drawBone; public float boneSize = 0.1f;
    public bool drawNormal; public float arrowSize = 0.1f;
    public bool drawTarget; public float targetSize = 0.1f;

#endif
    public InputReceiver myInput; public InputReceiver GetReceiver { get { if (myInput == null) { myInput = GetComponent<InputReceiver>(); } return myInput; } }
    Animator anim; public Animator GetAnim
    {
        get { if (anim == null) { anim = GetComponent<Animator>(); }
            return anim; }
    }
    Transform Spine, Chest, UpperChest;
    public Transform[] GetUpperBody { get { return new Transform[] {
        GetAnim.GetBoneTransform(HumanBodyBones.Spine) ,
        GetAnim.GetBoneTransform(HumanBodyBones.Chest) ,
        GetAnim.GetBoneTransform(HumanBodyBones.UpperChest)
    }; } }
    private void Awake()
    {
        myInput = GetComponent<InputReceiver>();

        Spine = GetAnim.GetBoneTransform(HumanBodyBones.Spine);
        Chest = GetAnim.GetBoneTransform(HumanBodyBones.Chest);
        UpperChest = GetAnim.GetBoneTransform(HumanBodyBones.UpperChest);
    }

    [Range(0, 1f)] public float clampweight;
    [Range(0, 1f)] public float body;
    [Range(0, 1f)] public float head;
    public Transform LeftHandTarget, RightHandTarget, LeftElbow, RightElbow;
    public Transform WeaponPivot;
    [Range(0, 1f)] public float weights;
    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetLookAtWeight(1, body, head, 0, clampweight);
        anim.SetLookAtPosition(myInput.LookTarget.position);

        GetAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        GetAnim.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
        GetAnim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
        GetAnim.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbow.position);

        GetAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
        GetAnim.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);


        GetAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, weights);
        GetAnim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
        GetAnim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, weights);
        GetAnim.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbow.position);

        GetAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, weights);
        GetAnim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);

    }

    public Transform upperBody, lowerBody;
    private Quaternion targetRotation; // ��ü�� �ٶ󺸴� ����ȸ���� : ���� ��ü(������ ������ �ٶ� ����)
    public Quaternion GetTargetDir { get { return targetRotation; } set { targetRotation = value; } }
    public float rotationDelay = 0.5f; // ��ü�� ������µ� �ɸ��� �ð�

    public Transform Tr;
    public float offset = 90f;
    private void Update()
    {
        Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir ); 
        Quaternion offsetRotation = Quaternion.Euler(0, offset, 0);

        // lookRotation�� offsetRotation�� ���մϴ�.
        lookRotation = lookRotation * offsetRotation;
        WeaponPivot.rotation = Quaternion.Slerp(WeaponPivot.rotation, lookRotation, Time.deltaTime / 0.1f);
        
        //Debug.Log(upperBody.rotation.eulerAngles.y);
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move = transform.forward * Time.deltaTime;
            
        }

        #region �̵����� ȸ���� �ִ� ���¶��
        // �̵��Է��� ���� ���¶�� �Ʒ� �����ϰ� ������
        // ����Ƽ �����ֱ�󿡼�, ������Ʈ�� �ִϸ��̼����μ������� ���� ����
        if (move == Vector3.zero)
        {
            
            float rotateFinishedTime = rotationDelay; // ���ڸ�ȸ���� �� ���� ������ ����

            if (GetAnim.GetBool("CurrentTurning") == false)
            {
                // ���Ű� ��ü�� ȸ�����̷�(y�� ����)
                float angle = Vector3.SignedAngle(transform.forward, myInput.YawRotator.forward, Vector3.up);
                Debug.Log(angle);

                if (angle > 60f)
                {
                    rotateFinishedTime = 0.25f;
                    GetAnim.SetBool("CurrentTurning", true);
                    GetAnim.PlayInFixedTime("RightRotatePose", 1);
                }

                else if (angle < -50f)
                {
                    rotateFinishedTime = 0.25f;
                    GetAnim.SetBool("CurrentTurning", true);
                    GetAnim.PlayInFixedTime("LeftRotatePose", 1);
                }
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / 0.1f);
            return;
        }

        #endregion

        
        // ó�� �̵� �����, �̺�Ʈ�Լ��� Ʈ������ ȸ�� ������ ���߱�(��üȸ�� ����)
        // Enum���� Idle, Walk�� ���� ���� ���� + exState������ ���� ���¿� ���� ���� ifüũ�Ͽ� �ѹ��� �����ϴ� �Լ� ����

        #region �̵��� ������ �������� ���߱�
        // ��ü�� ȸ���� Ÿ������ ����
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, myInput.YawRotator.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // ��ü�� ��ü ȸ���� ���� �⵵�� ���� �ð��� ������ ȸ��
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                               Time.deltaTime / rotationDelay);

        Tr.position += transform.forward * Time.deltaTime;
        #endregion

    }

 
    private void LateUpdate()
    {    
       // upperBody.rotation = Quaternion.Euler(0, myInput.yawAmount, 0);
   
    }
}
