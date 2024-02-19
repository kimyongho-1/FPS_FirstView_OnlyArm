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

    public AnimationCurve bodyWeightPerPitchRatio;
    [Range(0, 1f)] public float clampweight;
    [Range(0, 1f)] public float body;
    [Range(0, 1f)] public float head;
    public Transform LeftHandTarget, RightHandTarget, LeftElbow, RightElbow;
    private void OnAnimatorIK(int layerIndex)
    {

        switch (layerIndex)
        {
            case 0: LookAt(); return;

            case 1: HandIK(); return;

            case 2: FeetIK(); return;

        }

        void LookAt() // ��ü�� LookTarget�� �ٶ󺸱�
        {
            return;
            //body = bodyWeightPerPitchRatio.Evaluate(myInput.pitchAmount);
            GetAnim.SetLookAtWeight(1f, body, head, 1f, clampweight);
            GetAnim.SetLookAtPosition(myInput.LookTarget.position);
        }
        void HandIK() // �ѱ⿡ �ڵ� ����
                      // LookTarget���� �̹� ��ü�� ȸ���ϸ�,
                      // �ڽ��� �ڵ���� ȸ��+�̵��ϰ� �Ǿ� LookAt�� ���� ����� HandIK��
                      // ������ ��찡 �߻� => ���� ���̾� �������� �����Ͽ� �ذ�
        {
            GetAnim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            GetAnim.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
            GetAnim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            GetAnim.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);
            GetAnim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
            GetAnim.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbow.position);

            GetAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            GetAnim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
            GetAnim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            GetAnim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);
            GetAnim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow,1);
            GetAnim.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbow.position);
        }

        void FeetIK() 
        { }
    }

    public Transform gunForward;
    public void AimIk()
    {
        Vector3 aimDirection = gunForward.forward;
        Vector3 targetDirection = ( myInput.LookTarget.position - gunForward.position);
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        
        //upperBody2.rotation = aimTowards * upperBody2.rotation;// Quaternion.Euler(myInput.pitchAmount, upperBody2.rotation.y, upperBody2.rotation.z); // ����ϸ� �㸮���� ������ ������..
        
        upperBody.rotation = aimTowards * upperBody.rotation; // �̰͸� ����� ��ü ���κи� ������ ����
    }


    public Transform upperBody, upperBody2, lowerBody;
    private Quaternion targetRotation; // ��ü�� �ٶ󺸴� ����ȸ���� : ���� ��ü(������ ������ �ٶ� ����)
    public Quaternion GetTargetDir { get { return targetRotation; } set { targetRotation = value; } }
    public float rotationDelay = 0.5f; // ��ü�� ������µ� �ɸ��� �ð�

    public Transform Tr; // �ְ�θ� : �̵� 
    public float offsetGunRot = 90f;
    public Transform WeaponPivot;
    private void Update()
    {
        
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move = transform.forward * Time.deltaTime;
            
        }

        #region �̵����� ȸ���� �ִ� ���¶��
        // �̵��Է��� ���� ���¶��  (move == default)
        // ��ü�� ��ü ȸ������ ũ�� ���ڸ� ȸ�� ������ ������
        if (move == Vector3.zero)
        {   
            if (GetAnim.GetBool("CurrentTurning") == false)
            {
                // ���Ű� ��ü�� ȸ�����̷�(y�� ����)
                float angle = Vector3.SignedAngle(transform.forward, myInput.YawRotator.forward, Vector3.up);
                Debug.Log(angle);

                if (angle > 60f)
                {
                    GetAnim.SetBool("CurrentTurning", true);
                    GetAnim.PlayInFixedTime("RightRotatePose", 1);
                }

                else if (angle < -50f)
                {
                    GetAnim.SetBool("CurrentTurning", true);
                    GetAnim.PlayInFixedTime("LeftRotatePose", 1);
                }
            }
            // ȸ���� �ִϸ��̼� Ŭ���� StateMachineBehaviour.cs�� �����ϴ°� 
            // �ϰ����ִ� �Ǵ��Ͽ� Ʈ������ ��ü�� ȸ���� Update���� ����(�Ʒ� �ּ�)
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / 0.1f);
        }
        #endregion
        return;
        // �ѱ� ȸ�� ����
        GunRotateToLookTarget();
        void GunRotateToLookTarget()
        {
            Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Quaternion offsetRotation = Quaternion.Euler(0, offsetGunRot, 0);
            // offset��ŭ ����(�ѱ� �𵨸� ��ü�� �ѱ������� �ƴ� �������� z�����̶�)
            lookRotation = lookRotation * offsetRotation;
            WeaponPivot.rotation = lookRotation;
        }

       
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


    public float offsetsUpperBody= 40f;
    public float Y,X;
    private void LateUpdate()
    {
        AimIk();
        return;
        // ��ü ȸ��
        upperBody.rotation = Quaternion.Euler(myInput.pitchAmount, myInput.yawAmount , 0);
        upperBody2.rotation = Quaternion.Euler(myInput.pitchAmount, (myInput.yawAmount + offsetsUpperBody) , 0);
        //GunRotateToLookTarget();
        void GunRotateToLookTarget()
        {
            Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Quaternion offsetRotation = Quaternion.Euler(0, offsetGunRot, 0);
            // offset��ŭ ����(�ѱ� �𵨸� ��ü�� �ѱ������� �ƴ� �������� z�����̶�)
            lookRotation = lookRotation * offsetRotation;
            WeaponPivot.rotation = lookRotation;
        }
        
    }
}
