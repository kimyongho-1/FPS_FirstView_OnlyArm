using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MyBaseController
{

    public MyCapsuleData CapsuleData;
    public Vector3 GetColliderCenterInWS()
    { return CapsuleData.collider.bounds.center; }
    public LayerMask GetGroundMask() { return CapsuleData.groundMask; }

    public override void Awake()
    {
        base.Awake();
        SR = transform.GetComponentInChildren<SpineRotate>();
        SR.Init(this, myInput);
        StateMachine = new SMB_Character(this);
    }

    private void OnValidate()
    {
        CapsuleData.ColliderSetting();
    }

    /// <summary>
    /// ���� üũ �Լ�
    /// �� 3������ ������� �ϳ��� ����
    /// </summary>
    /// <returns>
    /// 1. ���� �������������̸�, ���� ������ ���̸�ŭ ĳ������ ���̸� �ø��ų� ������ ���� ��ȯ (�� ���� �ӷ��� ���־ ���� �����ؾߵ� �ӷ� ���͸� ��ȯ)
    /// 2. �̵� �Ұ����� ���ο� �ִ� ���, ������ ���+�Ʒ� �������� ���ϴ� ���� ��ȯ (������ �̲�������, 1���� ���������� ���� �ӷ��� ���־ �����̵��ϵ� �������� �ʰ� ����)
    /// 3. ���� ���� �ƴ� ���߿� �ִ� ���·μ� �߷°��� �ߺ� �����Ͽ� �Ʒ��� ���ϴ� ���� ��ȯ (���� �ӷ��� �����ʰ� �ߺ� �����Ͽ� ���� ������ �ϰ��ϵ��� ����)
    /// </returns>
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
            slopedAngle = CapsuleData.slopeCurve.Evaluate(Vector3.Angle(Vector3.up, hit.normal));
            Debug.Log(slopedAngle);
            if (slopedAngle < 0)
            {
                // ������ ��Ģ�� �̿��ؼ� ����� �����Ϳ� �븻����� �����Ͽ� ���� hit������Ʈ�� right�� ���ϱ�
                Vector3 slopeRight = Vector3.Cross(Vector3.up, hit.normal);

                // right��� ��ֹ����� �����Ͽ� ������ �������� ���ϱ�, �̋� �������踦 �̿��ؼ� �Ʒ� �������� ���� ���ϱ� (���� �̲����� ���� ���ؼ�)
                Vector3 slideDirection = Vector3.Cross(slopeRight, hit.normal).normalized;

                // �̲��� ������ ���⺤��
                return slideDirection;
            }

            float groundDist = height - ((GetColliderCenterInWS().y) - hit.point.y);

            // ������� �Ÿ� ���̰��� deltatime��ŭ
            // ��� ������ ���ݾ� ĳ���Ͱ� UpDown�ϵ��� rb�� �ӷ� ���� (���� ���� �ӷ��� ���־ �߰��ӷ� ��������)
            Vector3 upwards = (Vector3.up * (groundDist / Time.fixedDeltaTime)) - new Vector3(0, RB.velocity.y, 0);

            return upwards;
        }
        slopedAngle = 1f;
        // �ؿ� ���鰨�� ���Ұ��, ���� ���� ���°� �ƴ��� Ȯ�� �� �߷� ����
        // ���� �ӷ��� �����ϸ鼭 ���ο� �߷��� ���Ͽ� ���� �� ������ �����ϵ��� ����
        IsGround = false;
        return Vector3.up * (myInput.gravityAmount * Time.fixedDeltaTime * myInput.gravityForce);
    }

    private void Update()
    {

        // �Է� ����
        StateMachine.CurrState.Update();
    }
    private void FixedUpdate()
    {

        StateMachine.CurrState.FixedUpdate();
    }


}