using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class StandIdleState : BaseStandState
{
    public StandIdleState(MyBaseController owner) : base(owner) { }
   
    public override void Enter()
    {
        base.Enter();
        Owner.RB.velocity = Vector3.zero;
        StateEvt -= TransitionToIdleQuickly;
        StateEvt += TransitionToIdleQuickly;
    }

    // Idle로 전환시, 빠르게 파라미터값 수정 (현재 2배로)
    public void TransitionToIdleQuickly() 
    {
        // 애니메이션 파라미터 업데이트를 위한 스무딩 적용
        float currentX = Owner.FullBodyModel.GetFloat("X");
        float newX = Mathf.Lerp(currentX, 0, Owner.myInput.animationSmoothTime * 2f);
        float currentZ = Owner.FullBodyModel.GetFloat("Z");
        float newZ = Mathf.Lerp(currentZ, 0, Owner.myInput.animationSmoothTime * 2f);

        if (Mathf.Abs(newX) < 0.1f && Mathf.Abs(newZ) < 0.1f)
        {
            newX = newZ = 0;
            StateEvt -= TransitionToIdleQuickly;
        }
        Owner.FullBodyModel.SetFloat("X", newX);
        Owner.FullBodyModel.SetFloat("Z", newZ);
        // 총기 업데이트
        Owner.GunModel.SetFloat("X", newX);
        Owner.GunModel.SetFloat("Z", newZ);

    }

    public override void Exit()
    {
        StateEvt -= TransitionToIdleQuickly;
        #region 제자리 회전 애니메이션 강제중지
        // 현재 제자리에서 타겟방향을 바라보도록 회전하는 애니메이션 실행중일경우
        // 빈 클립을 재생시켜
        // 회전 애니메이션과 애니메이션TurnRotate스크립트 실행중지 시키기
        Owner.FullBodyModel.Play("Null",1);
        #endregion
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }
    public override void Update()
    {
        base.Update();

        // 입력처리에 따른 모델 애니메이션 업데이트
        AnimatorUpdate();

        if (Owner.myInput.dir != Vector2.zero) 
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandWalkState);
            return;
        }
    }
    public override void AnimatorUpdate()
    {
        StateEvt?.Invoke();

        if (Owner.FullBodyModel.GetBool("PelvisRotating") == false)
        {
            // 회전 애니메이션 레이어의 가중치는
            // 내부 스테이트머신비헤이버스크립트 Enter에서 1로 상승시키는 구조
            float angle = Vector3.SignedAngle(Owner.FullBodyModel.transform.forward, Owner.myInput.YawRotator.forward, Vector3.up);
       
            if (angle > 45f)
            { Owner.FullBodyModel.PlayInFixedTime("RightRotatePose", 1); }
            else if (angle < -45f)
            { Owner.FullBodyModel.PlayInFixedTime("LeftRotatePose", 1); }
        }
    }
}