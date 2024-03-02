using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStandState : ICState
{
    protected float standSpeed = 1f;

    public BaseStandState(OtherPlayerController owner) 
    { Owner = owner;  }

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
        Vector3 exVelocity = new Vector3(Owner.RB.velocity.x, 0, Owner.RB.velocity.z);
        Vector3 moveForce = Owner.myInput.movement - exVelocity ;

        // 공중속력 계산 (중력 또는 상승)
        moveForce += Owner.GroundCheck();
        Owner.RB.AddForce(moveForce, ForceMode.VelocityChange);
    }

    public virtual void Update()
    {
        Owner.myInput.DirectionInput();
        Owner.myInput.MovementInput(standSpeed);
    }
    public virtual void ModelUpdate()
    { }
}