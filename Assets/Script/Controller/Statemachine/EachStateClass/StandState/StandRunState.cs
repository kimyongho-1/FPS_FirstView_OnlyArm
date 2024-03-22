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

        // 달리기 상태 조건 => 이동입력 존재 && 전방으로 향하는 이동입력 존재

        // 이동 입력이 없다면, Idle로 전환
        if (Owner.myInput.dir == Vector2.zero)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandIdleState);
            return;
        }

        // LeftShift 클릭이 없거나 전방이동 입력이 없으면 walk로 전환
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
