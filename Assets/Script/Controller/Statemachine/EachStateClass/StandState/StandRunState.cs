using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StandRunState : BaseStandState
{
    public StandRunState(OtherPlayerController owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        standSpeed = 1.5f;
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
    }

    public override void Update()
    {
    }
}