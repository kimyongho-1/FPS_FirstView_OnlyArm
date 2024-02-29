using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMB_Base
{
    public StandIdleState StandIdleState;
    public StandWalkState StandWalkState;
    public StandRunState StandRunState;

    public ICState CurrState;// 현재의 상태

    public void ChangeState(ICState nextState)
    {
        CurrState?.Exit();

        CurrState = nextState;

        CurrState.Enter();
    }
}
