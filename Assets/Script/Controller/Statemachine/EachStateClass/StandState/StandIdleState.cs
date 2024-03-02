using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandIdleState : BaseStandState
{
    public StandIdleState(OtherPlayerController owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        Owner.RB.velocity = Vector3.zero;

        Owner.Model.CrossFadeInFixedTime("Rifle Aiming Idle", 0.1f, 0);
    }

    public override void Exit()
    {
        #region 제자리 회전 애니메이션 강제중지
        // 현재 제자리에서 타겟방향을 바라보도록 회전하는 애니메이션 실행중일경우
        // 빈 클립을 재생시켜
        // 회전 애니메이션과 애니메이션TurnRotate스크립트 실행중지 시키기
        Owner.Model.Play("Null",1);
        #endregion
    }

    public override void FixedUpdate()
    {
        Owner.RB.AddForce(Owner.GroundCheck(), ForceMode.VelocityChange);
    }
    public override void Update()
    {
        base.Update();

        // 입력처리에 따른 모델 애니메이션 업데이트
        ModelUpdate();

        if (Owner.myInput.movement != Vector3.zero) 
        {
            Owner.StateMachine.ChangeState(Owner.StateMachine.StandWalkState);
            return;
        }
    }
    public override void ModelUpdate()
    {
        base.ModelUpdate();  
        if (Owner.Model.GetBool("PelvisRotating") == false)
        {
            // 회전 애니메이션 레이어의 가중치는
            // 내부 스테이트머신비헤이버스크립트 Enter에서 1로 상승시키는 구조
            float angle = Vector3.SignedAngle(Owner.Model.transform.forward, Owner.myInput.YawRotator.forward, Vector3.up);
            //Debug.Log(angle);
            if (angle > 45f)
            {
                Owner.Model.PlayInFixedTime("RightRotate", 1);
            }
            else if (angle < -35f)
            {
                Owner.Model.PlayInFixedTime("LeftRotate", 1);
            }
        }
    }
}