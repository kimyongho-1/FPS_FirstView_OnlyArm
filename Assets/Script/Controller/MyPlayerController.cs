using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MyBaseController
{
    public CamPivotSpring CamPivot;
    public MyCapsuleData CapsuleData;
    public Vector3 GetColliderCenterInWS()
    { return CapsuleData.collider.bounds.center; }
    public LayerMask GetGroundMask() { return CapsuleData.groundMask; }

    public override void Awake()
    {
        base.Awake();
       // SR = transform.GetComponentInChildren<SpineRotate>();
       // SR.Init(this, myInput);
        StateMachine = new SMB_Character(this);
    }

    private void OnValidate()
    {
        CapsuleData.ColliderSetting();
    }

    /// <summary>
    /// 지면 체크 함수
    /// 총 3가지의 결과물중 하나를 도출
    /// </summary>
    /// <returns>
    /// 1. 현재 지면착지상태이며, 현재 지면의 높이만큼 캐릭터의 높이를 올리거나 내리는 벡터 반환 (단 이전 속력을 뺴주어서 현재 적용해야될 속력 벡터를 반환)
    /// 2. 이동 불가능한 경사로에 있는 경우, 경사로의 노멀+아래 방향으로 향하는 벡터 반환 (강제로 미끄러지게, 1번과 마찬가지로 이전 속력을 뺴주어서 순간이동하듯 내려가지 않게 조정)
    /// 3. 지면 위가 아닌 공중에 있는 상태로서 중력값을 중복 적용하여 아래로 향하는 벡터 반환 (이전 속력을 뺴지않고 중복 적용하여 점점 빠르게 하강하도록 유도)
    /// </returns>
    public override Vector3 GroundCheck()
    {
        // 콜라이더의 절반 길이와캡슐콜라이더가 감싸지 못한 캐릭터의 다리길이
        float capsuleHalfLen = CapsuleData.collider.height * 0.5f;
        float uncorvedLen = 1.8f - CapsuleData.colHeight;
        // 캡슐중앙이 지면으로부터 떠있어야할 거리
        float height = uncorvedLen + capsuleHalfLen;
        Debug.DrawRay(GetColliderCenterInWS(), Vector3.down * (height + CapsuleData.rayLength), Color.blue);//RAYLEN + halfLen

        // 캡슐의 바운드 중앙에서 아래로 레이캐스팅 시작
        if (Physics.Raycast(GetColliderCenterInWS(), Vector3.down, out RaycastHit hit, height + CapsuleData.rayLength, GetGroundMask()))
        {
            IsGround = true;
            slopedSpeed = CapsuleData.slopeCurve.Evaluate(Vector3.Angle(Vector3.up, hit.normal));
            
            // 지면 가속도가 1이상이란건, 이동불가능한 경사로면 위에 있음을 의미
            if (slopedSpeed > 1)
            {
                Debug.DrawRay(hit.point, hit.normal * 10f, Color.red);
                Debug.DrawRay(hit.point, Vector3.up * 10f, Color.green);
                // 오른손 법칙을 이용해서 월드상 업벡터와 노말방향과 외적하여 현재 hit오브젝트의 right축 구하기
                Vector3 slopeRight = Vector3.Cross(Vector3.up, hit.normal);
                Debug.DrawRay(hit.point, slopeRight * 10f, Color.blue);
                // right축과 노멀방향을 외적하여 나머지 방향축을 구하기, 이떄 순서관계를 이용해서 아래 방향쪽의 벡터 구하기 (경사로 미끌어짐 유도 위해서)
                Vector3 slideDirection = Vector3.Cross(slopeRight, hit.normal).normalized;

                // 미끄러 떨어질 방향벡터
                return slideDirection;
            }

            // 지면에 맞게 캡슐이 떠있어야할 거리차 구하기
            float groundDist = height - ((GetColliderCenterInWS().y) - hit.point.y);

            // 지면과의 거리 차이값을 deltatime만큼
            // 계속 나누어 조금씩 캐릭터가 UpDown하도록 rb의 속력 조절 (이전 상하 속력을 뺴주어서 추가속력 생성막기)
            Vector3 upwards = (Vector3.up * (groundDist / Time.fixedDeltaTime)) - new Vector3(0, RB.velocity.y, 0);

            return upwards;
        }
        slopedSpeed = 1f;
        // 밑에 지면감지 못할경우, 땅에 착지 상태가 아님을 확인 및 중력 적용
        // 이전 속력을 유지하면서 새로운 중력을 더하여 점점 더 빠르게 낙하하도록 유도
        IsGround = false;
        return Vector3.up * (myInput.gravityAmount * Time.fixedDeltaTime * myInput.gravityForce);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) { CamPivot.handCamAnim.SetTrigger("RC"); }
        // 입력 실행
        StateMachine.CurrState.Update();

        // 카메라+무기루트 피봇 회전 실행
        CamPivot.Rotate(myInput.pitchVal);
        // 애니메티어 업데이트
        StateMachine.CurrState.AnimatorUpdate();

    }
    private void FixedUpdate()
    {

        StateMachine.CurrState.FixedUpdate();
    }

}