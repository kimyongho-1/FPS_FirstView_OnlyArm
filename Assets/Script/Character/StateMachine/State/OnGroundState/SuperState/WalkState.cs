using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : OnGroundState
{
    public WalkState(PlayerStateMachine fsm) : base(fsm)
    {  }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        if (StateMachine.Player.movement == Vector2.zero)
        { StateMachine.SwitchState(StateMachine.IdleState); }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}