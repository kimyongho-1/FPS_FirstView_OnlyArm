using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SMB_Base
{
    public StandIdleState StandIdleState;
    public StandWalkState StandWalkState;
    public StandRunState StandRunState;

    public ICState CurrState;// 현재의 상태

    /// <summary>
    /// 현재 상태의 Exit를 실행하고
    /// 이후 인자 nextState의 Enter가 실행
    /// </summary>
    /// <param name="nextState"></param>
    public void ChangeState(ICState nextState)
    {
        CurrState?.Exit();

        CurrState = nextState;

        CurrState.Enter();
    }
}
