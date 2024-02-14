using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : OnGroundState
{
    public IdleState(PlayerStateMachine fsm) : base(fsm)
    { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
    public override void HandleInput()
    {
        base.HandleInput();
        StateMachine.Player.SettingData.Run = Input.GetKey(KeyCode.LeftShift);
        if(Input.GetKeyDown(KeyCode.C))
        {
            StateMachine.Player.SettingData.Crouch = !StateMachine.Player.SettingData.Crouch;
        }
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Update()
    {
        if (StateMachine.Player.movement == Vector2.zero)
        { return; }

        if (StateMachine.Player.SettingData.Run)
        {
            StateMachine.Player.Anim.CrossFadeInFixedTime("Run", StateMachine.Player.TSspeed);
            StateMachine.SwitchState(StateMachine.RunState);
            return;
        }

        StateMachine.Player.Anim.CrossFadeInFixedTime("WalkEnter", StateMachine.Player.TSspeed);
        StateMachine.SwitchState(StateMachine.WalkState);
    }
}