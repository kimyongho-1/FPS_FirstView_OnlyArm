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
        Owner.myInput.MovementInput(Owner.myInput.walkSpeed);
        AnimatorUpdate();

        if (Owner.myInput.dir == Vector2.zero)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandIdleState);
            return;
        }

        if (IsRunning)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandRunState);
            return;
        }
    }
    public override void AnimatorUpdate()
    {
        // 전신 캐릭터 애니메이터 업데이트
        base.AnimatorUpdate();
      
    }
}