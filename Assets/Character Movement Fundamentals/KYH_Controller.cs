using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_Controller : MonoBehaviour
{
    Animator anim;
    KYH_input input;
    KYH_Mover mover;
    
    public float moveSpeed;
    public float jumpForce;
    public float gravityForce;
    public bool isGrounded; 
    Transform tr;

    float currentVerticalSpeed; // 현재 적용할 수직속력 (위아래)
    Vector3 lastVelocity; // 지난 프레임에 적용된 속력

    void Start()
    {
        tr = transform;
        mover = GetComponent<KYH_Mover>();
        anim = mover.anim;
        input = GetComponent<KYH_input>();
    }
    void FixedUpdate()
    {
        #region 지면착지 상태 확인
        // 현재 캐릭터가 땅에 착지 상태인지 확인
        mover.CheckGround();

        // 이전 프레임에 컨트롤러상에선 땅위가 아니었지만, 현재 캐릭터가 땅에 착지상태라면
        if (isGrounded == false && mover.isGrounded == true)
        { OnGroundContact(lastVelocity); }

        // 현재 캐릭터가 땅에 있는지 상태를 초기화
        isGrounded = mover.isGrounded;

        #endregion

        #region 중력 작용
        if (!isGrounded)
        {
            // 공중에 있을시, 중력강도만큼 수직 속력을 내려주기
            currentVerticalSpeed -= gravityForce * Time.deltaTime;
        }
        else
        {
            // 땅에 착지상태면, 중력 추가 X
            if (currentVerticalSpeed <= 0f)
                currentVerticalSpeed = 0f;
        }
        #endregion

        #region 점프
        // 현재 지면 착지상태에서 Space 입력시 점프 시작
        if (isGrounded && input.IsJumpKeyPressed())
        {
            OnJumpStart();
            currentVerticalSpeed = jumpForce;
            isGrounded = false;
        }
        #endregion

        #region 이동속도 조정
        // 캐릭터 이동속도
        Vector3 _velocity = Vector3.zero;
        // 이동키 입력값으로 WASD방향 속력 결정
        // 지면인지 공중인지 확인으로 수직 속력 결정
        _velocity += CalculateMovementDirection() * moveSpeed + tr.up * currentVerticalSpeed;
        #endregion

        // 다음프레임떄 사용위해 현재 속력 저장
        lastVelocity = _velocity;

        // 실제 이동
        mover.SetVelocity(_velocity);
    }

    /// <summary>
    /// 이동속도 계산
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateMovementDirection()
    {
        if (input == null)
        { Debug.Log("Character Input Is NULL!!"); return Vector3.zero; }

        // 현재 캐릭터의 바라보는 방향에 맞게 입력값 초기화
        Vector3 _direction = Vector3.zero;
        float side = input.GetHorizontalMovementInput();
        float forward = input.GetVerticalMovementInput();
        _direction += tr.right * side;
        _direction += tr.forward *forward;
        anim.SetFloat("Forward", forward); anim.SetFloat("Side", side);
        // 대각선 키입력일시 1+1 = 2로 속력차이가 생기기에, 평균화하기
        if (_direction.magnitude > 1f)
        { _direction.Normalize(); }

        return _direction;
    }

    /// <summary>
    /// 캐릭터가 공중에서 땅에 착지하자마자 실행할 이벤트 모두 호출
    /// </summary>
    /// <param name="_collisionVelocity"></param>
    void OnGroundContact(Vector3 _collisionVelocity)
    {
        // 이벤트 함수 호출
    }
    
    /// <summary>
    /// 점프시작시 이벤트 존재시 호출
    /// </summary>
    void OnJumpStart()
    {
       // JUMP시 이벤트함수
    }
}
