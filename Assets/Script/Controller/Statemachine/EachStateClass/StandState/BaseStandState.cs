using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStandState : ICState
{
    protected Action StateEvt = null;

    public BaseStandState(MyBaseController owner)
    { Owner = owner; }

    public MyBaseController Owner;

    public bool IsRunning 
    { get { return (Owner.myInput.dir.y > 0  && Input.GetKey(KeyCode.LeftShift)) ; } }
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
        Vector3 ups = Owner.GroundCheck();

        // 경사로 속도보정이 1이란건, 경사로 문제가 없는경우 => 중력 또는 이동속도 적용
        if (Owner.slopedSpeed == 1 )
        {
            Vector3 exVelocity = new Vector3(Owner.RB.velocity.x, 0, Owner.RB.velocity.z);
            Vector3 moveForce = Owner.myInput.movement - exVelocity;
            moveForce += ups;
            Owner.RB.AddForce(moveForce, ForceMode.VelocityChange);
        }
        // 경사면으로 인해, 밑으로 미끄러져야하는 상황
        else
        {
            // 지면 착지 상태라면, 경사면을 타고 내려가도록 유도 (이전 속력은 현재에서 뺴주기 : 너무 빠른 낙하 방지)
            if (Owner.IsGround)
            {
                Owner.RB.AddForce((ups * Owner.slopedSpeed) - new Vector3(0, Owner.RB.velocity.y, 0), ForceMode.VelocityChange) ; 
            }

            // 공중에 떠있으면, 중력을 중복 적용시키는 목적으로 이전 속력에서 더하기 (점점 빠르게 떨어지도록 유도)
            else
            { Owner.RB.AddForce(ups, ForceMode.VelocityChange);}
        }
    }

    public virtual void Update()
    {
        Owner.myInput.DirectionInput();
    }
    public virtual void AnimatorUpdate()
    {
        // 애니메이션 파라미터 업데이트를 위한 스무딩 적용
        float currentX = Owner.FullBodyModel.GetFloat("X");
        float newX = Mathf.Lerp(currentX, Owner.myInput.dir.x, Owner.myInput.animationSmoothTime);

        float currentZ = Owner.FullBodyModel.GetFloat("Z");
        
        float newZ = Mathf.Lerp(currentZ, Owner.myInput.dir.y, Owner.myInput.animationSmoothTime);
       
        Owner.FullBodyModel.SetFloat("X", newX);
        Owner.FullBodyModel.SetFloat("Z", newZ);
        // 총기 업데이트
        Owner.GunModel.SetFloat("X", newX);
        Owner.GunModel.SetFloat("Z", newZ);

        // 방향 동기화
        Owner.FullBodyModel.transform.rotation = Owner.myInput.YawRotator.localRotation;
    }

}