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
    /// ������
    /// ���°� ����ɋ����� ���ο� �ν��Ͻ��� ������ �ƴ�
    /// ������ ĳ���ϴ� ������ : ������ ���� ������ �ݵ�� ������⿡, ĳ���Ͽ� ����ϴ°���
    /// �ξ� ���� �����̶� �Ǵ�
    /// ��, AI && NPC���� ��ð� �� �ൿ���� ���Ѵٸ� �̷��� ĳ���� �ƴ�
    /// ���Ҹ��� �ֱ������� �ν��Ͻ� ���¸� ���� �����Ͽ� ����ϴ°��� �����Ұ����� �Ǵ�
    /// </summary>
    public PlayerMovementStateMachine(Player player)
    { 
        Player = player;
        IdleState = new PlayerIdleState(this);
        walkingState = new PlayerWalkingState(this);
        runingState = new PlayerRuningState(this);
    }
}
