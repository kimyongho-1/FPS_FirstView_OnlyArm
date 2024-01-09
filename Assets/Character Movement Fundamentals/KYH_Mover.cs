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

        rb.freezeRotation = true; // ȸ�������Ͽ� ���ʿ��� ���� �̵��߿� �������� ���� ����
        rb.useGravity = false; // �߷��� �ڵ�� ����


        InitCapsuleCollider();
    }
    
    void InitSensor()
    {
        // ���̽�����ġ�� �׻� ������ �浹ü ���Ϳ��� �����ϵ��� (������������ ������ǥ��)
        sensor.RayStartPosition = transform.InverseTransformPoint(col.bounds.center);
        
        // ���� �߻����
        sensor.SetCastDir = KYH_Sensor.CastDirection.Down;

        RecalculateSensorLayerMask();

        // ����� 3d���� ���� y����
        modelLength = Model.bounds.size.y;
        // �ݶ��̴��� ������ �ʴ� ĳ���� ���� : ��ĳ������ ���� - �浹ü ����
        stepLength = modelLength - col.height;

        // ����üũ ����ĳ������ ���̴�
        // �浹ü ���� �𵨱��� + �浹ü ���� (���̰� �浹ü ���Ϳ��� ����) + �߰� Offset
        sensor.castLength = stepLength + (col.height * 0.5f) + RayLenthOffset;
    }

    void InitCapsuleCollider()
    {
        // TO DO : ���ĵ�, ũ��Ī, ���帰 ���µ ���� �浹ü ���� �ٲٱ�

        // ��� ���� ������ŭ ĸ���ݶ��̴��� ���Ϳ� ���� �ʱ�ȭ(������ ĳ������ ���������� �ݶ��̴��� ���۵ǵ���)
        col.center = new Vector3(0, Mathf.Lerp(0.8f, 1.2f, stepHeightRatio) ,0);
        col.height = Mathf.Lerp(1.6f , 0.8f , stepHeightRatio);

        // �浹ü�� ���̵��� ����� ������ �纯�� ����
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
        #region ��Ʈ���� �����ε�
        // AND : A & B => A��B ��Ʈ��Ұ� 1�ΰ͸� ��ȯ�� ���
        // OR  : A | B => A��B ��Ʈ��Ұ� �ϳ��� 1�̸� 1 ��ȯ
        // XOR : A ^ B => A��B ��Ʈ�� �Ұ� ���� �ٸ����� 1 ��ȯ
        // NOT : ~A   = > A��Ʈ�� ���� (��������)
        #endregion

        int _layerMask = 0; // �����ؾ��� ���̾� ����Ʈ ����
        int _objectLayer = this.gameObject.layer; // ���� ĳ���� ���̾�

        // ����Ƽ �⺻ ���̾�� IgnoreRaycast ã��
        int ignores = LayerMask.GetMask("Ignore Raycast");

        // ��ü ���̾� 32���� Ž��
        for (int i = 0; i < 32; i++)
        {
            // �ش� ���̾ IgnoreRaycast�� ��� �ǳʶٱ�
            if ( (1 << i) == ignores)  // 1�� i��ŭ ���� ����Ʈ������ ���̾�� ignores�� ������ (2 ^ i)
            { continue;  }

            // ���� ���̾� i�� �����ϸ� �ȵǴ°��̸�
            if (!Physics.GetIgnoreLayerCollision(_objectLayer, i))
            // ���������� �ش� ���̾� ���Խ�Ű��
            { _layerMask = _layerMask | (1 << i); }
        }

        // ���� ������� �����ҋ� ������ ���� ���ط��̾ ���� �浹���̾� ������� ��ü
        sensor.layermask = _layerMask;

        // �츮 ĳ������ ���̾� �缳��
        currentLayer = _objectLayer;
    }
    void Check()
    {
        // ����󿡼��� �ӵ� �ʱ�ȭ
        currentGroundVelocity = Vector3.zero;

        // �������ؼ� ���� �߻� (����üũ)
        sensor.Cast();

        // ���� ���, �浹ü ���翩�� Ȯ��
        if (!sensor.HasDetectedHit())
        {
            // ���� ������ ���°� �ƴϸ�, ��� ��ȯ�� ������ (Controller.cs�� ������ ���)
            isGrounded = false;
            return;
        }

        #region ���� ������ ���¶��
        isGrounded = true;

        // ���̽��������� �浹ü������ �Ÿ�
        float _distance = sensor.GetDistance();

        // ĸ���� �����κ��� ������ �Ÿ� ���ϱ� (���� üũ ���̸� ����ص� ���鿡 �°� ĳ���� ���̸� ������ ����)
        float _distanceToGo = (col.height * 0.5f + stepLength)  - _distance;
        #region (col.height * 0.5f + stepLength) ����
        // Ground üũ�� ����ĳ������ ������ �浹ü ���Ϳ��� ���� (capsuleCollider.center, ���� col)
        // col.height * 0.5f�� ���̽�����ġ���� �浹ü ������ ���̸� �ǹ��ϰ�
        // stepLength�� �浹ü������ ��� ���� ���̸� �ǹ� (stepHeightRatio�� �������� �ݶ��̴� ���)
        // col.height * 0.5f + stepLength�� �ᱹ ���̽��������� �𵨹�(��)������ �������̸� �ǹ�
        // �ٸ� �ǹ̷δ�, ���� ĳ������ �ݶ��̴��� �������� ���� �� �־���� �Ÿ����� �ǹ�
        // ����üũ ������, ��������� �Ÿ� _distance�� ����( ) �� ���ٽ�
        // �浹ü�� �󸶳� ���� �Ʒ��� �����ϴ��� �����ӵ� ���ϱ�
        #endregion

        // ���� �����ӿ� �ش� ������������ �������� �����ӷ� ��� (��Ʈ�ѷ����� ���� �ӷ��� ���ѵڿ� ������ ����)
        currentGroundVelocity = transform.up * (_distanceToGo / Time.fixedDeltaTime);
        #endregion
    }
    

    public void SetVelocity(Vector3 velocity)
    {
        rb.velocity = velocity + currentGroundVelocity;
    }
}
