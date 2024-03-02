using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandWalkState : BaseStandState
{
    public StandWalkState(OtherPlayerController owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        Owner.Model.CrossFadeInFixedTime("Walk Forward", 0.1f, 0);
    }

    public override void Exit()
    {
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override void Update()
    {
        base.Update();
        ModelUpdate();

        if (Owner.myInput.movement == Vector3.zero)
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandIdleState);
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandRunState);
            return;
        }
    }
    public override void ModelUpdate()
    {
        Owner.Model.transform.rotation = Owner.myInput.YawRotator.localRotation;
    }
}