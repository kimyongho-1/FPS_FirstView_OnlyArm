using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : BaseStateMachine
{
    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public RunState RunState { get; private set; }

    public PlayerStateMachine(BasePlayer p) : base(p)
    {
        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        RunState = new RunState(this);
        SwitchState(IdleState);
    }
}
