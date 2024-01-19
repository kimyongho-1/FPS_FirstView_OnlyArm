using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : OnGroundState
{
    public WalkState(PlayerControlStateMachine playerControlStateMachine) : base(playerControlStateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        speedModifier = 0.225f;
    }
    public override void Update()
    {
        base.Update();

        if (GetStateMachine.Player.movementInput == Vector2.zero)
        {
            GetStateMachine.ChangeState(GetStateMachine.IdleState);
            return;
        }
        if (GetStateMachine.Player.LeftShift)
        {
            GetStateMachine.ChangeState(GetStateMachine.RunState);
        }
    }

    protected override void AddInputCallBack()
    {
        base.AddInputCallBack();
        GetStateMachine.Player.inputData.MovementCanceled += OnMovementCanceled;
    }
    protected override void RemoveInputCallBack()
    {
        base.RemoveInputCallBack();
        GetStateMachine.Player.inputData.MovementCanceled -= OnMovementCanceled;
    }
    protected void OnMovementCanceled()
    {
        GetStateMachine.ChangeState(GetStateMachine.IdleState);
    }

    protected override void OnWalkToggleStarted()
    {
        base.OnWalkToggleStarted();

        GetStateMachine.ChangeState(GetStateMachine.RunState);
    }
}