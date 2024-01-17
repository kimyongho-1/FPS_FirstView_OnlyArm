using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlStateMachine : BaseStateMachine
{
    public BasePlayer Player { get; private set; }
    public IdleState IdleState { get; private set; }
    public WalkState WalkState { get; private set; }
    public RunState RunState { get; private set; }

    public PlayerControlStateMachine(BasePlayer player)
    {
        Player = player;
        IdleState = new IdleState(this);
        WalkState = new WalkState(this);
        RunState = new RunState(this);
    }
}