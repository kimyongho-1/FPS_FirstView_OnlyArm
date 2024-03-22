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

        // ���� �ӵ������� 1�̶���, ���� ������ ���°�� => �߷� �Ǵ� �̵��ӵ� ����
        if (Owner.slopedSpeed == 1 )
        {
            Vector3 exVelocity = new Vector3(Owner.RB.velocity.x, 0, Owner.RB.velocity.z);
            Vector3 moveForce = Owner.myInput.movement - exVelocity;
            moveForce += ups;
            Owner.RB.AddForce(moveForce, ForceMode.VelocityChange);
        }
        // �������� ����, ������ �̲��������ϴ� ��Ȳ
        else
        {
            // ���� ���� ���¶��, ������ Ÿ�� ���������� ���� (���� �ӷ��� ���翡�� ���ֱ� : �ʹ� ���� ���� ����)
            if (Owner.IsGround)
            {
                Owner.RB.AddForce((ups * Owner.slopedSpeed) - new Vector3(0, Owner.RB.velocity.y, 0), ForceMode.VelocityChange) ; 
            }

            // ���߿� ��������, �߷��� �ߺ� �����Ű�� �������� ���� �ӷ¿��� ���ϱ� (���� ������ ���������� ����)
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
        // �ִϸ��̼� �Ķ���� ������Ʈ�� ���� ������ ����
        float currentX = Owner.FullBodyModel.GetFloat("X");
        float newX = Mathf.Lerp(currentX, Owner.myInput.dir.x, Owner.myInput.animationSmoothTime);

        float currentZ = Owner.FullBodyModel.GetFloat("Z");
        
        float newZ = Mathf.Lerp(currentZ, Owner.myInput.dir.y, Owner.myInput.animationSmoothTime);
       
        Owner.FullBodyModel.SetFloat("X", newX);
        Owner.FullBodyModel.SetFloat("Z", newZ);
        // �ѱ� ������Ʈ
        Owner.GunModel.SetFloat("X", newX);
        Owner.GunModel.SetFloat("Z", newZ);

        // ���� ����ȭ
        Owner.FullBodyModel.transform.rotation = Owner.myInput.YawRotator.localRotation;
    }

}