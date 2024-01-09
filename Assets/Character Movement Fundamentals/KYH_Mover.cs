using CMF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class KYH_Mover : MonoBehaviour
{
    public Transform CameraTr;
    public Animator anim;
    public SkinnedMeshRenderer Model;
    private float modelLength;
    KYH_input input;
    public float pitch = 0f;
    public float yaw = 0f;
    Rigidbody rb; 
    CapsuleCollider col;
    KYH_Sensor sensor;
    int currentLayer;
    public float RayLenthOffset;
    public float stepLength;
    [Range(0f, 1f)][SerializeField] float stepHeightRatio = 0.25f;
    //[SerializeField] float colliderHeight = 2f;
    //[SerializeField] float colliderThickness = 1f;
    //Vector3 colliderOffset = Vector3.zero;
    public Vector3 currentGroundVelocity = Vector3.zero;
    public bool isGrounded;
    private void Awake()
    {
        input = GetComponent<KYH_input>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        sensor = new KYH_Sensor(this.transform, col) ;

        rb.freezeRotation = true; // 회전고정하여 불필요한 계산과 이동중에 쓰러지는 현상 방지
        rb.useGravity = false; // 중력은 코드로 구현


        InitCapsuleCollider();
    }
    
    void InitSensor()
    {
        // 레이시작위치는 항상 무버의 충돌체 센터에서 시작하도록 (월드포지션을 로컬좌표로)
        sensor.RayStartPosition = transform.InverseTransformPoint(col.bounds.center);
        
        // 레이 발사방향
        sensor.SetCastDir = KYH_Sensor.CastDirection.Down;

        RecalculateSensorLayerMask();

        // 사용할 3d모델의 순수 y길이
        modelLength = Model.bounds.size.y;
        // 콜라이더가 감싸지 않는 캐릭터 범위 : 모델캐릭터의 길이 - 충돌체 길이
        stepLength = modelLength - col.height;

        // 지면체크 레이캐스팅의 길이는
        // 충돌체 외의 모델길이 + 충돌체 절반 (레이가 충돌체 센터에서 시작) + 추가 Offset
        sensor.castLength = stepLength + (col.height * 0.5f) + RayLenthOffset;
    }

    void InitCapsuleCollider()
    {
        // TO DO : 스탠딩, 크런칭, 엎드린 상태등에 따라 충돌체 모형 바꾸기

        // 계단 격차 비율만큼 캡슐콜라이더의 센터와 높이 초기화(언제나 캐릭터의 정수리에서 콜라이더가 시작되도록)
        col.center = new Vector3(0, Mathf.Lerp(0.8f, 1.2f, stepHeightRatio) ,0);
        col.height = Mathf.Lerp(1.6f , 0.8f , stepHeightRatio);

        // 충돌체의 높이등이 변경시 센서도 재변경 요함
        InitSensor();
    }
    private void OnValidate()
    {
        if (transform.gameObject.activeInHierarchy)
        {
            InitCapsuleCollider();
        }   
    }

    private void Update()
    {
        sensor.DrawDebug();
        pitch -= input.GetMouseY() * 50f * Time.deltaTime;
        pitch = Mathf.Clamp(pitch ,-80f,80f);
        CameraTr.localRotation = Quaternion.Euler(pitch, 0, 0);
        yaw += input.GetMouseX() * 50f * Time.deltaTime;   
        transform.rotation = Quaternion.Euler(0, yaw, 0);

    }
 
    private void LateUpdate()
    {
    }
    public void CheckGround()
    {  
        if (currentLayer != this.gameObject.layer)
            RecalculateSensorLayerMask();

        Check();
    }
    void RecalculateSensorLayerMask()
    {
        #region 비트연산 리마인드
        // AND : A & B => A와B 비트요소가 1인것만 반환한 결과
        // OR  : A | B => A와B 비트요소가 하나라도 1이면 1 반환
        // XOR : A ^ B => A와B 비트요 소가 서로 다를때만 1 반환
        // NOT : ~A   = > A비트를 반전 (개별수행)
        #endregion

        int _layerMask = 0; // 감지해야할 레이어 리스트 변수
        int _objectLayer = this.gameObject.layer; // 현재 캐릭터 레이어

        // 유니티 기본 레이어에서 IgnoreRaycast 찾기
        int ignores = LayerMask.GetMask("Ignore Raycast");

        // 전체 레이어 32개를 탐색
        for (int i = 0; i < 32; i++)
        {
            // 해당 레이어가 IgnoreRaycast인 경우 건너뛰기
            if ( (1 << i) == ignores)  // 1을 i만큼 왼쪽 쉬프트연산한 레이어값이 ignores와 같은지 (2 ^ i)
            { continue;  }

            // 현재 레이어 i가 무시하면 안되는것이면
            if (!Physics.GetIgnoreLayerCollision(_objectLayer, i))
            // 감지범위에 해당 레이어 포함시키기
            { _layerMask = _layerMask | (1 << i); }
        }

        // 이후 지면등을 감지할떄 레이의 감지 기준레이어를 현재 충돌레이어 목록으로 교체
        sensor.layermask = _layerMask;

        // 우리 캐릭터의 레이어 재설정
        currentLayer = _objectLayer;
    }
    void Check()
    {
        // 지면상에서의 속도 초기화
        currentGroundVelocity = Vector3.zero;

        // 센서통해서 레이 발사 (지면체크)
        sensor.Cast();

        // 레이 결과, 충돌체 존재여부 확인
        if (!sensor.HasDetectedHit())
        {
            // 땅에 착지된 상태가 아니면, 결과 반환후 끝내기 (Controller.cs가 나머지 계산)
            isGrounded = false;
            return;
        }

        #region 땅에 차지된 상태라면
        isGrounded = true;

        // 레이시작점에서 충돌체까지의 거리
        float _distance = sensor.GetDistance();

        // 캡슐이 땅으로부터 떠있을 거리 구하기 (지면 체크 레이를 길게해도 지면에 맞게 캐릭터 높이를 내리기 가능)
        float _distanceToGo = (col.height * 0.5f + stepLength)  - _distance;
        #region (col.height * 0.5f + stepLength) 설명
        // Ground 체크시 레이캐스팅은 언제나 충돌체 센터에서 시작 (capsuleCollider.center, 이하 col)
        // col.height * 0.5f는 레이시작위치에서 충돌체 끝까지 길이를 의미하고
        // stepLength는 충돌체범위를 벗어난 모델의 길이를 의미 (stepHeightRatio가 높을수록 콜라이더 축소)
        // col.height * 0.5f + stepLength는 결국 레이시작점에서 모델발(끝)까지의 최종길이를 의미
        // 다른 의미로는, 현재 캐릭터의 콜라이더가 지면으로 부터 떠 있어야할 거리량을 의미
        // 지면체크 성공시, 지면까지의 거리 _distance를 현재( ) 와 뺴줄시
        // 충돌체가 얼마나 위로 아래로 가야하는지 수직속도 구하기
        #endregion

        // 다음 프레임에 해당 높이지점으로 도달위한 수직속력 계산 (컨트롤러에서 최종 속력을 구한뒤에 더해줄 예정)
        currentGroundVelocity = transform.up * (_distanceToGo / Time.fixedDeltaTime);
        #endregion
    }
    

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity + currentGroundVelocity;
    }
}
