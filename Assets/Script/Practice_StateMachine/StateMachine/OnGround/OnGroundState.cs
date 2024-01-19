using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundState : IStateEle
{
    protected bool shouldWalk;
    public float speedModifier = 1f;
    protected PlayerControlStateMachine GetStateMachine { get; set; }
    protected OnGroundState(PlayerControlStateMachine bsm)
    { GetStateMachine = bsm; }

    protected void ResetVelocity()
    {
        GetStateMachine.Player.RB.velocity = Vector3.zero;
    }
    protected virtual void AddInputCallBack()
    {
        GetStateMachine.Player.inputData.WalkToggle += OnWalkToggleStarted;
    }
    protected virtual void OnWalkToggleStarted()
    {
        shouldWalk = !shouldWalk;    
    }
    protected virtual void RemoveInputCallBack() 
    {
        GetStateMachine.Player.inputData.WalkToggle -= OnWalkToggleStarted;
    }
    public virtual void Enter()
    {
        Debug.Log($"현재 {GetType().Name} 상태");
        AddInputCallBack();
    }

    public virtual void Exit()
    {
        RemoveInputCallBack();
    }

    public virtual void PhysicsUpdate()
    {
        Floating();
        Rotate();
        Move();
    }
    void Floating()
    {
        Vector3 capsuleCenterInWS = GetStateMachine.Player.ColliderUtill.CapsuleColliderData.Collider.bounds.center;
        Ray down = new Ray(capsuleCenterInWS, Vector3.down);
        if (Physics.Raycast(down, out RaycastHit hit, GetStateMachine.Player.ColliderUtill.SlopeData.rayDist
            , GetStateMachine.Player.playerLayers.GroundLayer , QueryTriggerInteraction.Ignore))
        {
            float gorundAngle = Vector3.Angle(hit.normal, -down.direction);
            
            //SetSlopeSpeed(gorundAngle);
            //if (GetStateMachine.Player.inputData.slopeSpeed == 0) { return; }
            float distanceToFloat = GetStateMachine.Player.ColliderUtill.CapsuleColliderData.ColliderCenterInLocalSpace.y
                - hit.distance;
            Debug.Log(distanceToFloat);
            if (distanceToFloat == 0f) 
            { return; }

            float amountLift = distanceToFloat * GetStateMachine.Player.ColliderUtill.SlopeData.stepReachForce
                - GetStateMachine.Player.RB.velocity.y;

            GetStateMachine.Player.RB.AddForce(Vector3.up * (amountLift ), ForceMode.VelocityChange);
        }
    }

    private void SetSlopeSpeed(float gorundAngle)
    {
        GetStateMachine.Player.inputData.slopeSpeed = gorundAngle;
    }

    public virtual void Update()
    {
    }

    public virtual void UserInput()
    {
        GetStateMachine.Player.LeftShift = Input.GetKey(KeyCode.LeftShift);
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
        {
            return; 
        }

        Vector3 movementForce =
            GetStateMachine.Player.transform.right * GetStateMachine.Player.movementInput.x +
            GetStateMachine.Player.transform.forward * GetStateMachine.Player.movementInput.y  ;
        movementForce *= GetStateMachine.Player.inputData.moveSpeed * speedModifier * GetStateMachine.Player.inputData.slopeSpeed;

        Vector3 exMovement =  GetStateMachine.Player.RB.velocity ;
        exMovement.y = 0f;

        movementForce = (movementForce - exMovement);

        GetStateMachine.Player.RB.AddForce(movementForce, ForceMode.VelocityChange);
    }

}