using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStandState : ICState
{
    protected float standSpeed = 1f;

    public BaseStandState(OtherPlayerController owner)
    { Owner = owner; }

    public MyBaseController Owner;
    public virtual void Enter()
    {
        Debug.Log($"{GetType().Name} is Enter");
    }

    public virtual void Exit()
    {
        Debug.Log($"{GetType().Name} is Exit");
    }

    public virtual void FixedUpdate()
    {
        Vector3 ups = Owner.GroundCheck();

        // 공중속력 계산 (중력 또는 상승)
        if (Owner.slopedAngle > 0)
        {
            Vector3 exVelocity = new Vector3(Owner.RB.velocity.x, 0, Owner.RB.velocity.z);
            Vector3 moveForce = Owner.myInput.movement - exVelocity;
            moveForce += ups;
            Owner.RB.AddForce(moveForce, ForceMode.VelocityChange);
        }
        // 경사면으로 인해, 밑으로 미끄러져야하는 상황
        else
        {
            // 지면 착지 상태라면, 경사면을 타고 내려가도록 유도 (이전 속력은 현재에서 뺴주기 : 너무 빠른 낙하 방지)
            if (Owner.IsGround)
            { Owner.RB.AddForce((ups * -Owner.slopedAngle) - new Vector3(0, Owner.RB.velocity.y, 0), ForceMode.VelocityChange) ; }

            // 공중에 떠있으면, 중력을 중복 적용시키는 목적으로 이전 속력에서 더하기
            else
            { Owner.RB.AddForce(ups, ForceMode.VelocityChange);}
        }
    }

    public virtual void Update()
    {
        Owner.myInput.DirectionInput();
        Owner.myInput.MovementInput(standSpeed);

    }
    public virtual void ModelUpdate()
    { }
    public void Sliding()
    { 
        // 방향은 유지 + 단 slope값이 0보다 큰 수가 될떄까지 미끄러지기

    }
}