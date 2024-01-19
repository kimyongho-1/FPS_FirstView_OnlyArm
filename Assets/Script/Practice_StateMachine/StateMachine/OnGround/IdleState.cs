using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IdleState : OnGroundState
{

    public IdleState(PlayerControlStateMachine playerControlStateMachine) : base(playerControlStateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();
        speedModifier = 0;
        ResetVelocity();
    }
    public override void Update()
    {
        base.Update();
        if (GetStateMachine.Player.movementInput == Vector2.zero)
        {
            return;
        }

        if (!GetStateMachine.Player.LeftShift)
        { GetStateMachine.ChangeState(GetStateMachine.WalkState); return; }

        GetStateMachine.ChangeState(GetStateMachine.RunState);
    }
}
