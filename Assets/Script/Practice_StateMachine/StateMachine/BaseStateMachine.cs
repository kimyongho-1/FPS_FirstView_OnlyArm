using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseStateMachine 
{
    public IStateEle currentState { get; private set; }
    public void ChangeState(IStateEle newState)
    {
        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }
    public void UserInput()=> currentState?.UserInput(); 
    public void Update()=> currentState?.Update(); 
    public void PhysicsUpdate() => currentState?.PhysicsUpdate(); 
}
