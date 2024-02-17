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
    private Quaternion targetRotation; // 상체가 바라보는 방향회전축 : 이후 하체(전신이 강제로 바라볼 방향)
    public Quaternion GetTargetDir { get { return targetRotation; } set { targetRotation = value; } }
    public float rotationDelay = 0.5f; // 하체가 따라오는데 걸리는 시간

    public Transform Tr;
    public float offset = 90f;
    private void Update()
    {
        Vector3 dir = myInput.LookTarget.position - WeaponPivot.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir ); 
        Quaternion offsetRotation = Quaternion.Euler(0, offset, 0);

        // lookRotation에 offsetRotation을 곱합니다.
        lookRotation = lookRotation * offsetRotation;
        WeaponPivot.rotation = Quaternion.Slerp(WeaponPivot.rotation, lookRotation, Time.deltaTime / 0.1f);
        
        //Debug.Log(upperBody.rotation.eulerAngles.y);
        Vector3 move = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            move = transform.forward * Time.deltaTime;
            
        }

        #region 이동없이 회전만 있는 상태라면
        // 이동입력이 없는 상태라면 아래 실행하고 끝내기
        // 유니티 생명주기상에선, 업데이트가 애니메이션프로세스보다 먼저 실행
        if (move == Vector3.zero)
        {
            
            float rotateFinishedTime = rotationDelay; // 제자리회전은 더 빨리 끝나게 설정

            if (GetAnim.GetBool("CurrentTurning") == false)
            {
                // 전신과 상체의 회전차이량(y축 기준)
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

 
    private void LateUpdate()
    {    
       // upperBody.rotation = Quaternion.Euler(0, myInput.yawAmount, 0);
   
    }
}
