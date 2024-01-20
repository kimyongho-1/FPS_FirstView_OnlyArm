using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine
{
    public BasePlayer Player { get; private set; }
    public IState CurrentState { get; private set; }
    public BaseStateMachine(BasePlayer p) { Player = p; }
    public void SwitchState(IState nextState)
    {
        CurrentState?.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }
    public void HandleInput() => CurrentState.HandleInput();
    public void Update() => CurrentState.Update();
    public void FixedUpdate() => CurrentState.FixedUpdate();

}
