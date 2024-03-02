using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandIdleState : BaseStandState
{
    public StandIdleState(OtherPlayerController owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        Owner.RB.velocity = Vector3.zero;

        Owner.Model.CrossFadeInFixedTime("Rifle Aiming Idle", 0.1f, 0);
    }

    public override void Exit()
    {
        #region ���ڸ� ȸ�� �ִϸ��̼� ��������
        // ���� ���ڸ����� Ÿ�ٹ����� �ٶ󺸵��� ȸ���ϴ� �ִϸ��̼� �������ϰ��
        // �� Ŭ���� �������
        // ȸ�� �ִϸ��̼ǰ� �ִϸ��̼�TurnRotate��ũ��Ʈ �������� ��Ű��
        Owner.Model.Play("Null",1);
        #endregion
    }

    public override void FixedUpdate()
    {
        Owner.RB.AddForce(Owner.GroundCheck(), ForceMode.VelocityChange);
    }
    public override void Update()
    {
        base.Update();

        // �Է�ó���� ���� �� �ִϸ��̼� ������Ʈ
        ModelUpdate();

        if (Owner.myInput.movement != Vector3.zero) 
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandWalkState);
            return;
        }
    }
    public override void ModelUpdate()
    {
        base.ModelUpdate();  
        if (Owner.Model.GetBool("PelvisRotating") == false)
        {
            // ȸ�� �ִϸ��̼� ���̾��� ����ġ��
            // ���� ������Ʈ�ӽź����̹���ũ��Ʈ Enter���� 1�� ��½�Ű�� ����
            float angle = Vector3.SignedAngle(Owner.Model.transform.forward, Owner.myInput.YawRotator.forward, Vector3.up);
            //Debug.Log(angle);
            if (angle > 45f)
            {
                Owner.Model.PlayInFixedTime("RightRotate", 1);
            }
            else if (angle < -35f)
            {
                Owner.Model.PlayInFixedTime("LeftRotate", 1);
            }
        }
    }
}