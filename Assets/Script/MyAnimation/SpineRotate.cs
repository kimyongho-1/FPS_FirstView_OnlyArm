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

        void LookAt() // 상체가 LookTarget을 바라보기
        {
            return;
            //body = bodyWeightPerPitchRatio.Evaluate(myInput.pitchAmount);
            GetAnim.SetLookAtWeight(1f, body, head, 1f, clampweight);
            GetAnim.SetLookAtPosition(myInput.LookTarget.position);
        }
        void HandIK() // 총기에 핸드 고정
                      // LookTarget에서 이미 상체가 회전하면,
                      // 자식인 핸드까지 회전+이동하게 되어 LookAt과 같이 실행시 HandIK가
                      // 묻히는 경우가 발생 => 따로 레이어 구분지어 실행하여 해결
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
        
        //upperBody2.rotation = aimTowards * upperBody2.rotation;// Quaternion.Euler(myInput.pitchAmount, upperBody2.rotation.y, upperBody2.rotation.z); // 어떻게하면 허리축을 완전히 굽힐까..
        
        upperBody.rotation = aimTowards * upperBody.rotation; // 이것만 실행시 상체 윗부분만 접히는 현상
    }


    public Transform upperBody, upperBody2, lowerBody;
    private Quaternion targetRotation; // 상체가 바라보는 방향회전축 : 이후 하체(전신이 강제로 바라볼 방향)
    public Quaternion GetTargetDir { get { return targetRotation; } set { targetRotation = value; } }
    public float rotationDelay = 0.5f; // 하체가 따라오는데 걸리는 시간

    public Transform Tr; // 최고부모 : 이동 
    public float offsetGunRot = 90f;
    public Transform WeaponPivot;
    private void Update()
    {
        
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move = transform.forward * Time.deltaTime;
            
        }

        #region 이동없이 회전만 있는 상태라면
        // 이동입력이 없는 상태라면  (move == default)
        // 상체와 하체 회전차가 크면 제자리 회전 실행후 끝내기
        if (move == Vector3.zero)
        {   
            if (GetAnim.GetBool("CurrentTurning") == false)
            {
                // 전신과 상체의 회전차이량(y축 기준)
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
            // 회전을 애니메이션 클립의 StateMachineBehaviour.cs에 진행하는게 
            // 일관성있다 판단하여 트랜스폼 자체의 회전을 Update에서 제거(아래 주석)
            //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / 0.1f);
        }
        #endregion
        return;
        // 총기 회전 진행
        GunRotateToLookTarget();
        void GunRotateToLookTarget()
        {
            Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Quaternion offsetRotation = Quaternion.Euler(0, offsetGunRot, 0);
            // offset만큼 조정(총기 모델링 자체가 총구방향이 아닌 옆구리가 z방향이라서)
            lookRotation = lookRotation * offsetRotation;
            WeaponPivot.rotation = lookRotation;
        }

       
        // 처음 이동 실행시, 이벤트함수로 트랜스폼 회전 강제로 맞추기(하체회전 떄문)
        // Enum으로 Idle, Walk등 상태 변수 생성 + exState변수로 이전 상태와 현재 상태 if체크하여 한번만 실행하는 함수 실행

        #region 이동시 강제로 방향으로 맞추기
        // 상체의 회전을 타겟으로 설정
        targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, myInput.YawRotator.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

        // 하체가 상체 회전을 따라 잡도록 지연 시간을 가지고 회전
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
        // 상체 회전
        upperBody.rotation = Quaternion.Euler(myInput.pitchAmount, myInput.yawAmount , 0);
        upperBody2.rotation = Quaternion.Euler(myInput.pitchAmount, (myInput.yawAmount + offsetsUpperBody) , 0);
        //GunRotateToLookTarget();
        void GunRotateToLookTarget()
        {
            Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Quaternion offsetRotation = Quaternion.Euler(0, offsetGunRot, 0);
            // offset만큼 조정(총기 모델링 자체가 총구방향이 아닌 옆구리가 z방향이라서)
            lookRotation = lookRotation * offsetRotation;
            WeaponPivot.rotation = lookRotation;
        }
        
    }
}
