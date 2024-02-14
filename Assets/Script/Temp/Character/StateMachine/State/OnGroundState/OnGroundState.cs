using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundState : IState
{
    protected PlayerStateMachine StateMachine { get; private set; }
    public OnGroundState(PlayerStateMachine fsm) { StateMachine = fsm; }

    public virtual void HandleInput()
    {
        float limitedX = Mathf.Clamp(StateMachine.Player.direction.x - Input.GetAxis("Mouse Y") * StateMachine.Player.SettingData.mouseSensiY
                                    , StateMachine.Player.SettingData.minXrot
                                    , StateMachine.Player.SettingData.maxXrot );

        float Y = StateMachine.Player.direction.y + Input.GetAxis("Mouse X") * StateMachine.Player.SettingData.mouseSensiX;

        StateMachine.Player.direction = new Vector2(limitedX, Y);
        StateMachine.Player.movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        
        StateMachine.Player.Anim.SetFloat("Forward", StateMachine.Player.movement.y);
        StateMachine.Player.Anim.SetFloat("Side", StateMachine.Player.movement.x);
    }
    public virtual void Enter()
    {
        Debug.Log($"Enter {GetType().Name}");
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
        Rotate();
        Move();
    }

    public virtual void Update()
    {
        
    }

    public virtual void Rotate()
    {
        //float angle = Mathf.Atan2(StateMachine.Player.direction.y , StateMachine.Player.direction.x) * Mathf.Rad2Deg;
        StateMachine.Player.RB.MoveRotation(Quaternion.Euler(0, StateMachine.Player.direction.y, 0));
    }

    public virtual void Move()
    {
        Vector3 moveForce = StateMachine.Player.transform.forward * StateMachine.Player.movement.y
            + StateMachine.Player.transform.right * StateMachine.Player.movement.x;

        moveForce = moveForce .normalized * StateMachine.Player.SettingData.moveSpeed;

        Vector3 exForce = GetHorizontalVelocity();
        StateMachine.Player.RB.AddForce( (moveForce - exForce) , ForceMode.VelocityChange );
    }


    public Vector3 GetHorizontalVelocity() 
    {
        Vector3 velocity = StateMachine.Player.RB.velocity;
        velocity.y = 0f;
        return velocity;
    }
    public void ResetVelocity() 
    {
        StateMachine.Player.RB.velocity = (StateMachine.Player.RB.velocity - GetHorizontalVelocity()) ;
    }
}
