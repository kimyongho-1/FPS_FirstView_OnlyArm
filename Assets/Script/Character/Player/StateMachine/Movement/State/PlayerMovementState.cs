using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementState : IState
{
    public Vector2 movementInput;
    float baseSpeed = 5f;
    float speedModifier = 1f;


    public Vector2 directionInput;
    Vector3 currentTargetRotation;
    Vector3 timeToReachTargetRotation;
    Vector3 dampedTargetRotationCurrentVelocity;
    Vector3 dampedTargetRotationPassedTime;

    protected PlayerMovementStateMachine stateMachine;
    public PlayerMovementState(PlayerMovementStateMachine pmsm)
    {
        stateMachine = pmsm;
        InitializeData();
    }
    void InitializeData()
    {
        timeToReachTargetRotation.y = 0.14f;
    }
    #region IState Method
    public virtual void Enter()
    {
        Debug.Log($"State : {GetType().Name}");   
    }

    public virtual void Exit()
    {
    }

    public virtual void UserInput()
    {
        ReadDirectionInput();
        ReadMoveInput();
    }
    public virtual void PhysicsUpdate()
    {
        Move();
    }

    public virtual void Update()
    {
    }

    #endregion

    #region Func
    void ReadMoveInput()
    {
        movementInput.x = Input.GetAxis("Horizontal");
        movementInput.y = Input.GetAxis("Vertical");
    }
    void ReadDirectionInput()
    {
        directionInput.x -= Input.GetAxis("Mouse Y");
        directionInput.y = Input.GetAxis("Mouse X");
    }
    float Rotate(Vector3 dir)
    {
        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        if (angle < 0f)
        {
            angle += 360f;
        }

        float camAngle = stateMachine.Player.mainCamera.eulerAngles.y ;
        Debug.Log(camAngle);
        angle += camAngle; 

        if (angle > 360f)
        { angle -= 360f; }
        currentTargetRotation.y = angle;
        float currAngle = stateMachine.Player.rb.rotation.eulerAngles.y;
        if (currAngle == currentTargetRotation.y)
        { return 0; }

        float smoothedYAngle = Mathf.SmoothDampAngle(currAngle, currentTargetRotation.y, ref dampedTargetRotationCurrentVelocity.y,
           timeToReachTargetRotation.y - dampedTargetRotationCurrentVelocity.y);

        dampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

        stateMachine.Player.rb.MoveRotation(targetRotation);

        return smoothedYAngle;
        float Angle = UpdateTargetRotation(dir);
        RotateTowrdsTargetRotation();

        return Angle;
    }
    float UpdateTargetRotation(Vector3 dir)
    {
        float Angle = GetDirectionAngle(dir);
        Angle = AddCameraRotationToAngle(Angle);

        if (Angle != currentTargetRotation.y)
        {
            UpdateTargetRotationData(Angle);
        }
        return Angle;
    }
    void UpdateTargetRotationData(float angle)
    {
        currentTargetRotation.y = angle;
        dampedTargetRotationPassedTime.y = 0f;
    }
    float GetDirectionAngle(Vector3 dir)
    {
        float Angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        if (Angle < 0f)
        { 
            Angle += 360f;
        }
        
        return Angle;
    }
    float AddCameraRotationToAngle(float directionAngle)
    {
        directionAngle += stateMachine.Player.mainCamera.eulerAngles.y;
        if (directionAngle > 360f)
        { directionAngle -= 360f; }
        return directionAngle;
    }
    void RotateTowrdsTargetRotation()
    {
        float currAngle = stateMachine.Player.rb.rotation.eulerAngles.y;

        if (currAngle == currentTargetRotation.y)
        { return; }

        float smoothedYAngle = Mathf.SmoothDampAngle(currAngle, currentTargetRotation.y, ref dampedTargetRotationCurrentVelocity.y, 
            timeToReachTargetRotation.y - dampedTargetRotationCurrentVelocity.y);

        dampedTargetRotationPassedTime.y += Time.deltaTime;

        Quaternion targetRotation = Quaternion.Euler(0f,smoothedYAngle, 0f);

        stateMachine.Player.rb.MoveRotation(targetRotation);
    }
    void Move()
    {
        if (movementInput == Vector2.zero || speedModifier == 0)
        { 
            // rb.UseGravity�� ��� ���Ϸ��� �Ʒ� �ڵ� ���� �Ǵ�
            // Drag = 15f �� ����
            //  stateMachine.Player.rb.AddForce( -GetCurrentRigidbodyVelocity(), ForceMode.VelocityChange);
            return;
        }
        Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
        
        float targetRotationYangle = Rotate(movementDirection.normalized);
        float movementSpeed = baseSpeed * speedModifier;
        Vector3 targetRotationDir = GetTargetRotationDirection(targetRotationYangle) * movementSpeed;


        #region Rigidbody.AddForce �����ε�
        // ��ü(rigidbody)�� �����ϋ� AddForce�Լ��� ����ϰų� rb.Velocity�Ӽ��� ���� �����ϴµ�
        // AddForce�� ���� ���� ����������Ʈ���� ����
        // Velocity�� ���� RB.Velocity�� ���� ���� ��� ����
        // ���� AddForce�� 4���� �ɼ�Ÿ��
        // mass ������ ������ -> ���� �Ѿ��� �߻�� mass���԰� �� �����°��� �ѹ��� ���� �ً� �� ���� ���� ��������
        // time ������ ������ -> ������ �߻��Ѵ� ������, ������ �����ӵ��� �ƴ� ���������� ���� �ӷ��� ���� ���
        // ForceMode.Force : �ð��� ���� �������
        // ForceMode.Impulse : �������� ���� ����
        // ForceMode.Acceleration : �ð����� ���� ����
        // ForceMode.VelocityChange : �ð� ���� ���� X 
        #endregion

        // ������ ���� RB ����ӵ��� �����´�
        Vector3 currentVelocity = GetCurrentRigidbodyVelocity();
        // ���� �̵��� ����*�ӵ� ���͸� ���� �ӵ��� �A�� => �������� �ӵ��� �����Ǵ°� ����
        

        stateMachine.Player.rb.AddForce(targetRotationDir - currentVelocity, ForceMode.VelocityChange);
    }
    Vector3 GetTargetRotationDirection(float angle)
    {
        return Quaternion.Euler(0f, angle, 0f) * Vector3.forward;
    }
    protected Vector3 GetCurrentRigidbodyVelocity()
    {
        Vector3 result = stateMachine.Player.rb.velocity;
        result.y = 0f;
        return result;
    }
    #endregion

}