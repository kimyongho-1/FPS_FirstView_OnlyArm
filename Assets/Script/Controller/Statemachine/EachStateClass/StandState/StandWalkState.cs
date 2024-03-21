using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandWalkState : BaseStandState
{
    public StandWalkState(MyBaseController owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        Owner.GunModel.SetBool("HoldBreath",false);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Update()
    {
        base.Update();
        AnimatorUpdate();

        Owner.GunModel.SetBool("HoldBreath", false);
        if (Owner.myInput.dir == Vector2.zero)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandIdleState);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandRunState);
            return;
        }
    }
    public override void AnimatorUpdate()
    {
        // ���� ĳ���� �ִϸ����� ������Ʈ
        base.AnimatorUpdate();
      
    }
}