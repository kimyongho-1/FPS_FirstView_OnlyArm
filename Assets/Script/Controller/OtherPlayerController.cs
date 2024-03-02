using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MyBaseController
{
    SpineRotate0224 SR; 
    public MyCapsuleData CapsuleData;
    public Vector3 GetColliderCenterInWS()
    { return CapsuleData.collider.bounds.center; }
    public LayerMask GetGroundMask() { return CapsuleData.groundMask; }

    public override void Awake()
    {
        base.Awake();
        SR = transform.GetComponentInChildren<SpineRotate0224>();
        SR.Init(this, myInput);
        StateMachine = new SMB_Character(this);
    }

    private void OnValidate()
    {
        CapsuleData.ColliderSetting();
    }
    Vector3 curr;
    public float speeeeeed;
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
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            float groundDist = height - ((GetColliderCenterInWS().y) - hit.point.y) ;

            // 지면과의 차이거리값을 time.deltatime만큼
            // 계속 나누어 조금씩 캐릭터가 UpDown하도록 rb의 속력 조절
            // 이전 상하 속력을 뺴주어서 추가속력 생성막기
            Vector3 upwards= (Vector3.up * (groundDist / Time.fixedDeltaTime)) - new Vector3(0, RB.velocity.y,0);
      
            return upwards;
        }
        
        // 밑에 지면감지 못할경우, 땅에 착지 상태가 아님을 확인 및 중력 적용
        // 이전 속력을 유지하면서 새로운 중력을 더하여 점점 더 빠르게 낙하하도록 유도
        IsGround = false;
        return Vector3.up * ( myInput.gravityAmount * Time.fixedDeltaTime * myInput.gravityForce);
    }

    private void Update()
    {
       
        // 입력 실행
        StateMachine.CurrState.Update();

        // 입력처리에 따른 모델 애니메이션 업데이트
        SR.ModelUpdate();
    }
    private void FixedUpdate()
    {
        
        StateMachine.CurrState.FixedUpdate();
    }


}