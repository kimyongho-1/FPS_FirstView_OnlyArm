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
        // �ݶ��̴��� ���� ���̿�ĸ���ݶ��̴��� ������ ���� ĳ������ �ٸ�����
        float capsuleHalfLen = CapsuleData.collider.height * 0.5f;
        float uncorvedLen = 1.8f - CapsuleData.colHeight;
        // ĸ���߾��� �������κ��� ���־���� �Ÿ�
        float height = uncorvedLen + capsuleHalfLen;
        Debug.DrawRay(GetColliderCenterInWS(), Vector3.down * (height + CapsuleData.rayLength), Color.blue);//RAYLEN + halfLen

        // ĸ���� �ٿ�� �߾ӿ��� �Ʒ��� ����ĳ���� ����
        if (Physics.Raycast(GetColliderCenterInWS(), Vector3.down, out RaycastHit hit, height + CapsuleData.rayLength, GetGroundMask()))
        {
            IsGround = true;
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            float groundDist = height - ((GetColliderCenterInWS().y) - hit.point.y) ;

            // ������� ���̰Ÿ����� time.deltatime��ŭ
            // ��� ������ ���ݾ� ĳ���Ͱ� UpDown�ϵ��� rb�� �ӷ� ����
            // ���� ���� �ӷ��� ���־ �߰��ӷ� ��������
            Vector3 upwards= (Vector3.up * (groundDist / Time.fixedDeltaTime)) - new Vector3(0, RB.velocity.y,0);
      
            return upwards;
        }
        
        // �ؿ� ���鰨�� ���Ұ��, ���� ���� ���°� �ƴ��� Ȯ�� �� �߷� ����
        // ���� �ӷ��� �����ϸ鼭 ���ο� �߷��� ���Ͽ� ���� �� ������ �����ϵ��� ����
        IsGround = false;
        return Vector3.up * ( myInput.gravityAmount * Time.fixedDeltaTime * myInput.gravityForce);
    }

    private void Update()
    {
       
        // �Է� ����
        StateMachine.CurrState.Update();

        // �Է�ó���� ���� �� �ִϸ��̼� ������Ʈ
        SR.ModelUpdate();
    }
    private void FixedUpdate()
    {
        
        StateMachine.CurrState.FixedUpdate();
    }


}