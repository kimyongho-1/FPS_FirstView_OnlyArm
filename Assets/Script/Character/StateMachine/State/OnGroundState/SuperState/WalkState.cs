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
        base.Exit();
    }

    public override void Update()
    {
        if (StateMachine.Player.movement == Vector2.zero)
        {
            StateMachine.Player.Anim.CrossFadeInFixedTime("IdleEnter", StateMachine.Player.TSspeed); 
            StateMachine.SwitchState(StateMachine.IdleState); 
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}