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
    }

    public virtual void Update()
    {
        Owner.myInput.DirectionInput();
        Owner.myInput.MovementInput(standSpeed);
    }
    public virtual void ModelUpdate()
    { }
}