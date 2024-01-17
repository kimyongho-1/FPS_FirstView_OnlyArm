using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachine
{
    protected IState currentState;

    /// <summary>
    /// ���º��� �޼ҵ�
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(IState newState)
    {
        // ������� ��� ���� : ���� ���°� �������� �ֱ⿡ ���
        currentState?.Exit();

        currentState = newState;

        // ���� �޸� newState������ ������ �����̱⿡ ������� ������ �ʿ� X
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
