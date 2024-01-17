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
            // rb.UseGravity를 사용 안하려면 아래 코드 실행 또는
            // Drag = 15f 로 고정
            //  stateMachine.Player.rb.AddForce( -GetCurrentRigidbodyVelocity(), ForceMode.VelocityChange);
            return;
        }
        Vector3 movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
        
        float targetRotationYangle = Rotate(movementDirection.normalized);
        float movementSpeed = baseSpeed * speedModifier;
        Vector3 targetRotationDir = GetTargetRotationDirection(targetRotationYangle) * movementSpeed;


        #region Rigidbody.AddForce 리마인드
        // 강체(rigidbody)를 움직일떄 AddForce함수를 사용하거나 rb.Velocity속성을 직접 수정하는데
        // AddForce는 사용시 다음 물리업데이트에서 적용
        // Velocity는 현재 RB.Velocity를 덮어 띄우며 즉시 적용
        // 이후 AddForce의 4가지 옵션타입
        // mass 영향을 받을시 -> 같은 총알을 발사시 mass무게가 더 나가는것이 한번의 힘을 줄떄 더 빨리 땅에 떨어진다
        // time 영향을 받을시 -> 로켓을 발사한다 가정시, 로켓이 일정속도가 아닌 점진적으로 점점 속력을 내는 모션
        // ForceMode.Force : 시간과 질량 영향받음
        // ForceMode.Impulse : 질량에만 영향 받음
        // ForceMode.Acceleration : 시간에만 영향 받음
        // ForceMode.VelocityChange : 시간 질량 영향 X 
        #endregion

        // 유저의 현재 RB 수평속도를 가져온다
        Vector3 currentVelocity = GetCurrentRigidbodyVelocity();
        // 현재 이동할 방향*속도 벡터를 현재 속도를 뺸다 => 무한으로 속도가 증폭되는걸 방지
        

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