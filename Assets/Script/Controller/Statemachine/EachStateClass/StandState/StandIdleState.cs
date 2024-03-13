using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StandIdleState : BaseStandState
{
    public StandIdleState(MyBaseController owner) : base(owner) { }
   
    public override void Enter()
    {
        base.Enter();
        Owner.RB.velocity = Vector3.zero;
        StateEvt -= TransitionToIdleQuickly;
        StateEvt += TransitionToIdleQuickly;
    }

    // Idle�� ��ȯ��, ������ �Ķ���Ͱ� ���� (���� 2���)
    public void TransitionToIdleQuickly() 
    {
        // �ִϸ��̼� �Ķ���� ������Ʈ�� ���� ������ ����
        float currentX = Owner.FullBodyModel.GetFloat("X");
        float newX = Mathf.Lerp(currentX, 0, Owner.myInput.animationSmoothTime * 2f);
        float currentZ = Owner.FullBodyModel.GetFloat("Z");
        float newZ = Mathf.Lerp(currentZ, 0, Owner.myInput.animationSmoothTime * 2f);

        if (Mathf.Abs(newX) < 0.1f && Mathf.Abs(newZ) < 0.1f)
        {
            newX = newZ = 0;
            StateEvt -= TransitionToIdleQuickly;
        }
        Owner.FullBodyModel.SetFloat("X", newX);
        Owner.FullBodyModel.SetFloat("Z", newZ);
        // �ѱ� ������Ʈ
        Owner.GunModel.SetFloat("X", newX);
        Owner.GunModel.SetFloat("Z", newZ);

    }

    public override void Exit()
    {
        StateEvt -= TransitionToIdleQuickly;
        #region ���ڸ� ȸ�� �ִϸ��̼� ��������
        // ���� ���ڸ����� Ÿ�ٹ����� �ٶ󺸵��� ȸ���ϴ� �ִϸ��̼� �������ϰ��
        // �� Ŭ���� �������
        // ȸ�� �ִϸ��̼ǰ� �ִϸ��̼�TurnRotate��ũ��Ʈ �������� ��Ű��
        Owner.FullBodyModel.Play("Null",1);
        #endregion
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Update()
    {
        base.Update();

        // �Է�ó���� ���� �� �ִϸ��̼� ������Ʈ
        AnimatorUpdate();

        if (Owner.myInput.dir != Vector2.zero) 
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandWalkState);
            return;
        }
    }
    public override void AnimatorUpdate()
    {
        StateEvt?.Invoke();

        if (Owner.FullBodyModel.GetBool("PelvisRotating") == false)
        {
            // ȸ�� �ִϸ��̼� ���̾��� ����ġ��
            // ���� ������Ʈ�ӽź����̹���ũ��Ʈ Enter���� 1�� ��½�Ű�� ����
            float angle = Vector3.SignedAngle(Owner.FullBodyModel.transform.forward, Owner.myInput.YawRotator.forward, Vector3.up);
       
            if (angle > 45f)
            { Owner.FullBodyModel.PlayInFixedTime("RightRotatePose", 1); }
            else if (angle < -45f)
            { Owner.FullBodyModel.PlayInFixedTime("LeftRotatePose", 1); }
        }
    }
}