using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : OnGroundState
{
    public RunState(PlayerStateMachine fsm) : base(fsm)
    { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
    }
}
