using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    /// <summary>
    /// 상태변경 메소드
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IState newState)
    {
        // 널컨디션 사용 이유 : 현재 상태가 없을수도 있기에 대비
        currentState?.Exit();

        currentState = newState;

        // 위와 달리 newState참조를 진행후 실행이기에 널컨디션 실행할 필요 X
        currentState.Enter();
    }
    public IState GetCurrentState { get { return currentState; } }

    public void UserInput()
    { currentState?.UserInput(); }

    public void Update()
    { currentState?.Update(); }
    public void PhysicsUpdate()
    { currentState?.PhysicsUpdate(); }
}
