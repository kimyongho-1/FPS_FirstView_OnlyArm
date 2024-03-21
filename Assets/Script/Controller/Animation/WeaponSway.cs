using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    public MyPlayerController pc;
   
    public AnimationCurve xPos, yPos;
    Vector3 input, velocity, destValue ,destRot;
    /// <summary>
    ///zeta : �����, ����� 0�̸� ������ϸ�,
    ///       ����� 0 : �����ϰ� �����ϴ� ����
    ///       0 ~ 1 ���� ������ : ��ǥ������ ���������� ������ ũ��� �ð��� ������ ���� ���ϱ޼������� ����
    ///       1 �Ӱ谨�� : �ý����� �������� ���� ������ ��ǥ������ �����Ǵ� ���� (��ǥ���� �����ϱ� ���� �ʿ��� �ּҽð����� �����ϴ� ��)
    ///       1 �ʰ� ������ : �Ӱ谨�躸�� ������ ��ǥ������ ����, ����� �����Կ� ���� ��ǥ������ �����ϴ� �ӵ��� ��������
    ///                     ������ ȿ�����ٴ� �巡��(����)ȿ���� �� �������� ������ ����.</summary>
    [Range(0, 1.5f)] public float zeta;
    public float Frequency = 1f;
    public float curveRatioX,curveRatioY = 0;
    Vector2 exMag;
    public void Update()
    {
        float deltaTime = Time.deltaTime;
        float Omega = Mathf.PI * Frequency;
        // 2���� �ֱ⸦ 1�ʿ� ���ļ� �ϳ��� ��ü�ֱ�
        // Frequency�� ���� �ֱ⸦ �÷� �ӵ��� ������Ű��

        Vector2 currDir = pc.myInput.mouseDir;
        float xDir = 1f;  float yDir = 1f;
        if (currDir.x > 0)  { xDir = -1; }
        if (currDir.y > 0) { yDir = -1; }

        float xMag = currDir.x - exMag.x;
        if (xMag < 1f) { curveRatioX = Mathf.Lerp(curveRatioX, 0, 0.25f);  } // ���� ����ϋ��� mag < 2~3���� ���� ��������
        else if (xMag < 3f) { curveRatioX = Mathf.Lerp(curveRatioX, 0.75f, 0.75f); } // ��������
        else { curveRatioX = Mathf.Lerp(curveRatioX, 1, 0.95f);  }


        float yMag = currDir.y - exMag.y;
        if (yMag < 1f) { curveRatioY = Mathf.Lerp(curveRatioY, 0, 0.25f);  } // ���� ����ϋ��� mag < 2~3���� ���� ��������
        else if (yMag < 3f) { curveRatioY = Mathf.Lerp(curveRatioY, 0.75f, 0.75f); } // ��������
        else { curveRatioY = Mathf.Lerp(curveRatioY, 1, 0.95f); }

        destValue = new Vector3(xDir * xPos.Evaluate(curveRatioX), yDir * yPos.Evaluate(curveRatioY), 0);
        exMag = currDir;
        SimpleSpring(Omega, deltaTime);
        //ImplictEulerSpring(Omega, deltaTime);

    }
    /// <summary>
    /// �ܼ��� ����� ��ġ ������
    /// �ܼ��� | �������� �����Ҽ��� (�㳪 ���� ��Ÿ���� ������ �Ѱ谡 �������־ ����ص� �����ҵ�)
    /// </summary>
    /// <param name="Omega"></param>
    public void SimpleSpring(float Omega, float deltaTime)
    {
        Vector3 preCalcul = -2.0f * deltaTime * zeta * Omega * velocity;

        velocity += preCalcul + deltaTime * Omega * Omega * (destValue - input);
        input += deltaTime * velocity;
        transform.localPosition = input;
        destRot += deltaTime * velocity;
        // ���콺�Է��� ������ ���� (�� ȸ���� �������̶��)
        // ũ�� �߰� ȸ���� �Է�
        // �ƹ��� �Է��� ������? => 0���� ���� �õ� (Lerp�Լ� ���)
        transform.localRotation = Quaternion.Euler(destRot.y * destRotMultiplyX,0, destRot.x * destRotMultiplyY);

    }
    public float destRotMultiplyX,destRotMultiplyY = 15f;
    /// <summary>
    /// �Ͻ������Ϸ� ��ġ ������
    /// ������ | ������ ��� + ��Ÿ���� ��ġ������
    /// </summary>
    /// <param name="Omega"></param>
    public void ImplictEulerSpring(float Omega, float deltaTime)
    {
        float f = (1 + 2 * deltaTime * zeta * Omega);
        float dDelta = deltaTime * deltaTime;
        float dOmega = (Omega * Omega);
        float Delta = 1.0f / (f + dDelta * dOmega);

        Vector3 DeltaVelo = velocity + deltaTime * dOmega * (destValue - input);
        velocity = DeltaVelo * Delta;
        Vector3 DeltaInput = f * input + deltaTime * velocity + dDelta * dOmega * destValue;
        input = DeltaInput * Delta;

        transform.localPosition = input;

        Vector3 DeltaRot = f * destRot + deltaTime * velocity + dDelta * dOmega * destValue;
        destRot = DeltaRot * Delta;
        transform.localRotation = Quaternion.Euler(destRot.y * destRotMultiplyX, 0, destRot.x * destRotMultiplyY);
    }
}
