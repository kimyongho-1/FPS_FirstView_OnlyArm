using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStandState : ICState
{
    protected float standSpeed = 1f;

    public BaseStandState(OtherPlayerController owner)
    { Owner = owner; }

    public MyBaseController Owner;
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

        // ���߼ӷ� ��� (�߷� �Ǵ� ���)
        if (Owner.slopedAngle > 0)
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
            { Owner.RB.AddForce((ups * -Owner.slopedAngle) - new Vector3(0, Owner.RB.velocity.y, 0), ForceMode.VelocityChange) ; }

            // ���߿� ��������, �߷��� �ߺ� �����Ű�� �������� ���� �ӷ¿��� ���ϱ�
            else
            { Owner.RB.AddForce(ups, ForceMode.VelocityChange);}
        }
    }

    public virtual void Update()
    {
        Owner.myInput.DirectionInput();
        Owner.myInput.MovementInput(standSpeed);

    }
    public virtual void ModelUpdate()
    { }
    public void Sliding()
    { 
        // ������ ���� + �� slope���� 0���� ū ���� �ɋ����� �̲�������

    }
}