using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandRunState : BaseStandState
{
    public StandRunState(MyBaseController owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        Owner.FullBodyModel.SetBool("Sprint", true);
    }

    public override void Exit()
    {
        Owner.FullBodyModel.SetBool("Sprint", false);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        base.Update();
        Owner.myInput.MovementInput(Owner.myInput.runSpeed);
        AnimatorUpdate();

        // �޸��� ���� ���� => �̵��Է� ���� && �������� ���ϴ� �̵��Է� ����

        // �̵� �Է��� ���ٸ�, Idle�� ��ȯ
        if (Owner.myInput.dir == Vector2.zero)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandIdleState);
            return;
        }

        // LeftShift Ŭ���� ���ų� �����̵� �Է��� ������ walk�� ��ȯ
        if (!Input.GetKey(KeyCode.LeftShift) || Owner.myInput.dir.y <= 0)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandWalkState);
            return;
        }
    }

    public override void AnimatorUpdate()
    {
        base.AnimatorUpdate();
    }
}
