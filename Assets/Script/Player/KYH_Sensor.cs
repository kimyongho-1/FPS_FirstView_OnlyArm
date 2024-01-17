using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable] // ����ȭ���� (�������̹� X)
public class KYH_Sensor
{
    #region Debug
   
    public void DrawDebug()
    {
        if (hasDetectedHit && isInDebugMode)
        {
            //Debug.DrawRay(hitPosition, hitNormal, Color.red, Time.deltaTime);
            float _markerSize = 0.2f;
            
           // Debug.DrawRay(GetRayOrigin(),Vector3.down * hitDistance, Color.green, Time.deltaTime); // ������� �Ÿ� ����
            Debug.DrawRay(GetRayOrigin(), Vector3.down * castLength, Color.red, Time.deltaTime); // �������� ����
            //Debug.DrawLine(GetRayOrigin(), hitPosition - Vector3.up * _markerSize, Color.green, Time.deltaTime);
            //Debug.DrawLine(hitPosition + Vector3.right * _markerSize, hitPosition - Vector3.right * _markerSize, Color.green, Time.deltaTime);
            //Debug.DrawLine(hitPosition + Vector3.forward * _markerSize, hitPosition - Vector3.forward * _markerSize, Color.green, Time.deltaTime);
        }
    }
    #endregion

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="owner">�� ������ ������ Mover������Ʈ ��ü</param>
    /// <param name="ownersCol">Mover������Ʈ ��ü�� �ݶ��̴�</param>
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

    // ���� Sensor�� ���� ������Ʈ ����
    private Transform tr;
    private Collider col;

    // ���̹߻� ��Ÿ�� : �Ϲ� Ray, ��ü����ĳ����
    [SerializeField]
    public enum CastType
    { Raycast, Spherecast }
    public CastType castType = CastType.Raycast;
    public LayerMask layermask = 255; // ��ü ���̾��ũ
    int ignoreRaycastLayer; // ����Ƽ �⺻���̾���, IgnoreLayer����


    public float castLength = 1f; // ���� ����
    
    Vector3 rayStartingPosisiton = Vector3.zero; // ���� ������ġ 
    public Vector3 RayStartPosition { get { return rayStartingPosisiton; } set { rayStartingPosisiton = value; } }

    // ���ù����� ����ĳ���� �������� ����ϱ�����, Enum���� �з�
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

    #region Ray�浹������, �浹ü(�ٴ�,���� ��)�� ����
    private bool hasDetectedHit; // ��������
    private Vector3 hitPosition; // �浹ü ��ġ
    private Vector3 hitNormal; // �浹ü���� ���溤��
    private float hitDistance; // �Ÿ�
    #endregion


    // ����ĳ���ý�, ������ �浹ü ��� ( ex : �÷��̾� �ڽ��� �浹ü)
    private Collider[] ignoreColliderList;
    // ����ĳ���ý�, ������ ���̾� ��� (ex : ����Ƽ ���� �⺻ Ignore RaycastLayer )
    private int[] ignoreLayersList;

    // ����� ��� Ȱ��ȭ �Ұ�����
    public bool isInDebugMode = true;

    #region ��üĳ����
    public float sphereCastRadius = 0.2f; // ��ü ĳ���ý�, �ѷ�
    //Cast an additional ray to get the true surface normal;
    public bool calculateRealSurfaceNormal = false;
    //Cast an additional ray to get the true distance to the ground;
    public bool calculateRealDistance = false;
    //Backup normal used for specific edge cases when using spherecasts;                                          
    private Vector3 backupNormal;

    #endregion

    // ���̹߻� : ���� �������� ����
    public void Cast()
    {
        ResetFlags();

        // ������ǥ������ ���̽�����ġ�� ���� �ʱ�ȭ
	    Vector3 _worldDirection = GetCastDirection();
        Vector3 _worldOrigin = GetRayOrigin();


        // ���� ����ĳ���� ���� ���̾�� ����ĳ���� ���� �ݶ��̴����� �ٸ���
        if (ignoreLayersList.Length != ignoreColliderList.Length)
        {
            // ����ĳ���� ���÷��̾� ���� ���� �ݶ��̴����� �����ϰ� ����
            ignoreLayersList = new int[ignoreColliderList.Length];
        }

        // ���� ����ĳ������ �����ؾ��ϴ� �浹ü���� � ���̾ ������� �𸣴� ����
        // �׷��� ����ĳ���� ��������, ����ĳ���� ���� List�� �� �浹ü���� ���� ���̾ �־ ����صα�
        // ���� �浹ü���� ���̾ ����Ƽ ���� IgnoreRaycast ���̾�� ��ȯ (����ĳ���� ������ ����)
        for (int i = 0; i < ignoreColliderList.Length; i++)
        {
            // �ٽ� ������ ���̾�� ���ư�������, ���� ���̾ ����
            ignoreLayersList[i] = ignoreColliderList[i].gameObject.layer;
            // ���� ���̾ �����صξ��⿡, ����ĳ������ ���ϱ����� IgnoreLayer�� �ʱ�ȭ
            ignoreColliderList[i].gameObject.layer = ignoreRaycastLayer;
        }

        #region Ray �߻�
        hasDetectedHit = Physics.Raycast(_worldOrigin, _worldDirection, out RaycastHit _hit, castLength, layermask, 
            QueryTriggerInteraction.Ignore);  // Collider�� isTrigger�� ����ϴ� �浹ü ����

        // �浹ü ������
        if (hasDetectedHit) 
        {
            hitPosition = _hit.point;
            hitNormal = _hit.normal;
            hitDistance = _hit.distance;
        }
        #endregion

        // ����ĳ������ ��������, �����ؾ��� �浹ü���� ���̾� ���󺹱�
        for (int i = 0; i < ignoreColliderList.Length; i++)
        {
            ignoreColliderList[i].gameObject.layer = ignoreLayersList[i];
        }
    }


    /// <summary>
    ///���� ĳ�����ϱ���, �� ������� ������ ������� ���� ĳ���� ������ϵ� �ʱ�ȭ 
    /// </summary>
    void ResetFlags()
    {
        hasDetectedHit = false;
        hitPosition = Vector3.zero;
        hitNormal = Vector3.zero;
        hitDistance  = 0f;

    }

    /// <summary>
    /// ���̹߻������ ĳ������ ���� ������� ��ȯ�� ���⺤�� ( ex : ����ϰ� �������� Down�����̸� Tr.down)
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
    /// ����ĳ���� ������ġ��, ���� Mover��ü ��ġ�� �߽������� ������ǥ�� ������ �������� (���� : WorldPositon)
    /// </summary>
    /// <returns></returns>
    Vector3 GetRayOrigin()
    { return tr.TransformPoint(rayStartingPosisiton);  }

    // �浹ü ���� ���� ��ȯ
    public bool HasDetectedHit() { return hasDetectedHit; }
    public float GetDistance() { return hitDistance; }
}
