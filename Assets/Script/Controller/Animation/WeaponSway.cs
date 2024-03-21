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
    ///zeta : 감쇠비, 감쇠비가 0이면 감쇠안하며,
    ///       감쇠비가 0 : 무한하게 진동하는 상태
    ///       0 ~ 1 사이 저감쇠 : 목표값으로 수렴하지만 진동의 크기는 시간이 지남에 따라 기하급수적으로 감소
    ///       1 임계감쇠 : 시스템이 진동없이 가장 빠르게 목표값으로 고정되는 현상 (목표값에 도달하기 위해 필요한 최소시간으로 수렴하는 비)
    ///       1 초과 과감쇠 : 임계감쇠보다 느리게 목표값으로 수렴, 감쇠비가 증가함에 따라 목표값으로 도달하는 속도가 느려진다
    ///                     스프링 효과보다는 드래그(저항)효과가 더 강해지는 성질을 지님.</summary>
    [Range(0, 1.5f)] public float zeta;
    public float Frequency = 1f;
    public float curveRatioX,curveRatioY = 0;
    Vector2 exMag;
    public void Update()
    {
        float deltaTime = Time.deltaTime;
        float Omega = Mathf.PI * Frequency;
        // 2파이 주기를 1초에 걸쳐서 하나의 전체주기
        // Frequency를 통해 주기를 올려 속도를 증감시키기

        Vector2 currDir = pc.myInput.mouseDir;
        float xDir = 1f;  float yDir = 1f;
        if (currDir.x > 0)  { xDir = -1; }
        if (currDir.y > 0) { yDir = -1; }

        float xMag = currDir.x - exMag.x;
        if (xMag < 1f) { curveRatioX = Mathf.Lerp(curveRatioX, 0, 0.25f);  } // 조준 모드일떄는 mag < 2~3으로 증가 시켜주자
        else if (xMag < 3f) { curveRatioX = Mathf.Lerp(curveRatioX, 0.75f, 0.75f); } // 마찬가지
        else { curveRatioX = Mathf.Lerp(curveRatioX, 1, 0.95f);  }


        float yMag = currDir.y - exMag.y;
        if (yMag < 1f) { curveRatioY = Mathf.Lerp(curveRatioY, 0, 0.25f);  } // 조준 모드일떄는 mag < 2~3으로 증가 시켜주자
        else if (yMag < 3f) { curveRatioY = Mathf.Lerp(curveRatioY, 0.75f, 0.75f); } // 마찬가지
        else { curveRatioY = Mathf.Lerp(curveRatioY, 1, 0.95f); }

        destValue = new Vector3(xDir * xPos.Evaluate(curveRatioX), yDir * yPos.Evaluate(curveRatioY), 0);
        exMag = currDir;
        SimpleSpring(Omega, deltaTime);
        //ImplictEulerSpring(Omega, deltaTime);

    }
    /// <summary>
    /// 단순한 계산의 수치 스프링
    /// 단순함 | 오류수정 존재할수도 (허나 지금 제타같은 변수가 한계가 정해져있어서 사용해도 무방할듯)
    /// </summary>
    /// <param name="Omega"></param>
    public void SimpleSpring(float Omega, float deltaTime)
    {
        Vector3 preCalcul = -2.0f * deltaTime * zeta * Omega * velocity;

        velocity += preCalcul + deltaTime * Omega * Omega * (destValue - input);
        input += deltaTime * velocity;
        transform.localPosition = input;
        destRot += deltaTime * velocity;
        // 마우스입력이 작으면 무시 (단 회전이 진행중이라면)
        // 크면 추가 회전량 입력
        // 아무런 입력이 없으면? => 0도로 원복 시도 (Lerp함수 사용)
        transform.localRotation = Quaternion.Euler(destRot.y * destRotMultiplyX,0, destRot.x * destRotMultiplyY);

    }
    public float destRotMultiplyX,destRotMultiplyY = 15f;
    /// <summary>
    /// 암시적오일러 수치 스프링
    /// 복잡함 | 정밀한 결과 + 제타같은 수치안정성
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
