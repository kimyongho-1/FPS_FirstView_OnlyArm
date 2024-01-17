using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable] // 직렬화위해 (모노비헤이버 X)
public class KYH_Sensor
{
    #region Debug
   
    public void DrawDebug()
    {
        if (hasDetectedHit && isInDebugMode)
        {
            //Debug.DrawRay(hitPosition, hitNormal, Color.red, Time.deltaTime);
            float _markerSize = 0.2f;
            
           // Debug.DrawRay(GetRayOrigin(),Vector3.down * hitDistance, Color.green, Time.deltaTime); // 지면과의 거리 레이
            Debug.DrawRay(GetRayOrigin(), Vector3.down * castLength, Color.red, Time.deltaTime); // 센서길이 레이
            //Debug.DrawLine(GetRayOrigin(), hitPosition - Vector3.up * _markerSize, Color.green, Time.deltaTime);
            //Debug.DrawLine(hitPosition + Vector3.right * _markerSize, hitPosition - Vector3.right * _markerSize, Color.green, Time.deltaTime);
            //Debug.DrawLine(hitPosition + Vector3.forward * _markerSize, hitPosition - Vector3.forward * _markerSize, Color.green, Time.deltaTime);
        }
    }
    #endregion

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="owner">이 센서의 소유자 Mover컴포넌트 객체</param>
    /// <param name="ownersCol">Mover컴포넌트 객체의 콜라이더</param>
    public KYH_Sensor(Transform owner, Collider ownersCol)
    {
        tr = owner;
        col = ownersCol;
        ignoreColliderList = new Collider[1];
        ignoreColliderList[0] = ownersCol;
        ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
        ignoreLayersList = new int[1];
        ignoreLayersList[0] = ignoreRaycastLayer;
    }

    // 현재 Sensor의 주인 오브젝트 정보
    private Transform tr;
    private Collider col;

    // 레이발사 스타일 : 일반 Ray, 구체레이캐스팅
    [SerializeField]
    public enum CastType
    { Raycast, Spherecast }
    public CastType castType = CastType.Raycast;
    public LayerMask layermask = 255; // 전체 레이어마스크
    int ignoreRaycastLayer; // 유니티 기본레이어중, IgnoreLayer정보


    public float castLength = 1f; // 레이 길이
    
    Vector3 rayStartingPosisiton = Vector3.zero; // 레이 시작위치 
    public Vector3 RayStartPosition { get { return rayStartingPosisiton; } set { rayStartingPosisiton = value; } }

    // 로컬방향을 레이캐스팅 방향으로 사용하기위해, Enum으로 분류
    public enum CastDirection
    {
        Forward,
        Right,
        Up,
        Backward,
        Left,
        Down
    }
    private CastDirection castDirection;
    public CastDirection SetCastDir { set { castDirection = value; } }

    #region Ray충돌감지시, 충돌체(바닥,경사면 등)의 정보
    private bool hasDetectedHit; // 감지여부
    private Vector3 hitPosition; // 충돌체 위치
    private Vector3 hitNormal; // 충돌체면의 전방벡터
    private float hitDistance; // 거리
    #endregion


    // 레이캐스팅시, 제외할 충돌체 목록 ( ex : 플레이어 자신의 충돌체)
    private Collider[] ignoreColliderList;
    // 레이캐스팅시, 제외할 레이어 목록 (ex : 유니티 내장 기본 Ignore RaycastLayer )
    private int[] ignoreLayersList;

    // 디버깅 모드 활성화 할것인지
    public bool isInDebugMode = true;

    #region 구체캐스팅
    public float sphereCastRadius = 0.2f; // 구체 캐스팅시, 둘레
    //Cast an additional ray to get the true surface normal;
    public bool calculateRealSurfaceNormal = false;
    //Cast an additional ray to get the true distance to the ground;
    public bool calculateRealDistance = false;
    //Backup normal used for specific edge cases when using spherecasts;                                          
    private Vector3 backupNormal;

    #endregion

    // 레이발사 : 지면 포착등을 위해
    public void Cast()
    {
        ResetFlags();

        // 월드좌표에서의 레이시작위치와 방향 초기화
	    Vector3 _worldDirection = GetCastDirection();
        Vector3 _worldOrigin = GetRayOrigin();


        // 현재 레이캐스팅 무시 레이어와 레이캐스팅 무시 콜라이더수가 다를시
        if (ignoreLayersList.Length != ignoreColliderList.Length)
        {
            // 레이캐스팅 무시레이어 수를 무시 콜라이더수와 동일하게 수정
            ignoreLayersList = new int[ignoreColliderList.Length];
        }

        // 현재 레이캐스팅을 무시해야하는 충돌체들이 어떤 레이어를 사용할지 모르는 상태
        // 그래서 레이캐스팅 시작전에, 레이캐스팅 무시 List에 각 충돌체들의 원본 레이어를 넣어서 기억해두기
        // 그후 충돌체들의 레이어를 유니티 내장 IgnoreRaycast 레이어로 변환 (레이캐스팅 끝날시 원복)
        for (int i = 0; i < ignoreColliderList.Length; i++)
        {
            // 다시 본래의 레이어로 돌아가기위해, 현재 레이어를 저장
            ignoreLayersList[i] = ignoreColliderList[i].gameObject.layer;
            // 원본 레이어를 저장해두었기에, 레이캐스팅을 피하기위해 IgnoreLayer로 초기화
            ignoreColliderList[i].gameObject.layer = ignoreRaycastLayer;
        }

        #region Ray 발사
        hasDetectedHit = Physics.Raycast(_worldOrigin, _worldDirection, out RaycastHit _hit, castLength, layermask, 
            QueryTriggerInteraction.Ignore);  // Collider중 isTrigger를 사용하는 충돌체 무시

        // 충돌체 감지시
        if (hasDetectedHit) 
        {
            hitPosition = _hit.point;
            hitNormal = _hit.normal;
            hitDistance = _hit.distance;
        }
        #endregion

        // 레이캐스팅이 끝났으니, 무시해야할 충돌체들의 레이어 원상복구
        for (int i = 0; i < ignoreColliderList.Length; i++)
        {
            ignoreColliderList[i].gameObject.layer = ignoreLayersList[i];
        }
    }


    /// <summary>
    ///새로 캐스팅하기전, 새 지면등의 정보를 담기위해 기존 캐싱한 변수기록들 초기화 
    /// </summary>
    void ResetFlags()
    {
        hasDetectedHit = false;
        hitPosition = Vector3.zero;
        hitNormal = Vector3.zero;
        hitDistance  = 0f;

    }

    /// <summary>
    /// 레이발사방향을 캐릭터의 현재 모습으로 변환시 방향벡터 ( ex : 평범하게 서있을시 Down방향이면 Tr.down)
    /// </summary>
    /// <returns></returns>
    Vector3 GetCastDirection()
    {
        switch (castDirection)
        {
            case CastDirection.Forward:
                return tr.forward;

            case CastDirection.Right:
                return tr.right;

            case CastDirection.Up:
                return tr.up;

            case CastDirection.Backward:
                return -tr.forward;

            case CastDirection.Left:
                return -tr.right;

            case CastDirection.Down:
                return -tr.up;
            default:
                return Vector3.one;
        }
    }
    /// <summary>
    /// 레이캐스팅 시작위치를, 현재 Mover객체 위치를 중심으로한 로컬좌표로 수정후 가져오기 (최종 : WorldPositon)
    /// </summary>
    /// <returns></returns>
    Vector3 GetRayOrigin()
    { return tr.TransformPoint(rayStartingPosisiton);  }

    // 충돌체 감지 여부 반환
    public bool HasDetectedHit() { return hasDetectedHit; }
    public float GetDistance() { return hitDistance; }
}
