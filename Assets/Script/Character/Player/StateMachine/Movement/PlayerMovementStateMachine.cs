using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementStateMachine : StateMachine
{
    public Player Player { get; private set; }
    public PlayerIdleState IdleState { get;}
    public PlayerWalkingState walkingState { get;}
    public PlayerRuningState runingState { get;}

    /// <summary>
    /// 생성자
    /// 상태가 변경될떄마다 새로운 인스턴스로 생성이 아닌
    /// 변수로 캐싱하는 이유는 : 유저의 잦은 변경이 반드시 따라오기에, 캐싱하여 사용하는것이
    /// 훨씬 나은 선택이라 판단
    /// 단, AI && NPC같이 장시간 한 행동만을 취한다면 이러한 캐싱이 아닌
    /// 긴텀마다 주기적으로 인스턴스 상태를 새로 생성하여 사용하는것이 유리할것으로 판단
    /// </summary>
    public PlayerMovementStateMachine(Player player)
    { 
        Player = player;
        IdleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        runingState = new PlayerRuningState(this);
    }
}
