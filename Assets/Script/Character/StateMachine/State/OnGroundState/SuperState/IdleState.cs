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
    }
    public override void HandleInput()
    {
        base.HandleInput();
        StateMachine.Player.SettingData.Run = Input.GetKey(KeyCode.LeftShift);
        StateMachine.Player.SettingData.Crouch = Input.GetKey(KeyCode.C);
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
            StateMachine.SwitchState(StateMachine.RunState);
            return;
        }
        StateMachine.SwitchState(StateMachine.WalkState);
    }
}