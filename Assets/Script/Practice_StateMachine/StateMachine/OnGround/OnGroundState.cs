using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundState : IStateEle
{
    protected PlayerControlStateMachine GetStateMachine { get; set; }
    protected OnGroundState(PlayerControlStateMachine bsm)
    { GetStateMachine = bsm; }
    public virtual void Enter()
    {
        Debug.Log($"현재 {GetType().Name} 상태");
    }

    public virtual void Exit()
    {

    }

    public virtual void PhysicsUpdate()
    {
        Rotate();
        Move();
    }

    public virtual void Update()
    {

    }

    public virtual void UserInput()
    {
        GetStateMachine.Player.movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        GetStateMachine.Player.directionInput += new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        GetStateMachine.Player.directionInput.x = Mathf.Clamp(GetStateMachine.Player.directionInput.x,-80f,80f);
    }
    protected void Rotate()
    {
        GetStateMachine.Player.RB.MoveRotation(Quaternion.Euler(0, GetStateMachine.Player.directionInput.y
            * GetStateMachine.Player.inputData.mouseSensiX, 0));
    }
    protected void Move()
    {
        if (GetStateMachine.Player.movementInput == Vector2.zero)
        { return; }

        Vector3 movementForce =
            GetStateMachine.Player.transform.right * GetStateMachine.Player.movementInput.x +
            GetStateMachine.Player.transform.forward * GetStateMachine.Player.movementInput.y  ;
        movementForce *= GetStateMachine.Player.inputData.moveSpeed ;

        Vector3 exMovement =  GetStateMachine.Player.RB.velocity ;
        exMovement.y = 0f;

        movementForce = (movementForce - exMovement);

        GetStateMachine.Player.RB.AddForce(movementForce, ForceMode.VelocityChange);
    }

}