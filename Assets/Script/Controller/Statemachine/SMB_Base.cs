using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMB_Base
{
    public StandIdleState StandIdleState;
    public StandWalkState StandWalkState;
    public StandRunState StandRunState;

    public ICState CurrState;// ������ ����

    /// <summary>
    /// ���� ������ Exit�� �����ϰ�
    /// ���� ���� nextState�� Enter�� ����
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(ICState nextState)
    {
        CurrState?.Exit();

        CurrState = nextState;

        CurrState.Enter();
    }
}
