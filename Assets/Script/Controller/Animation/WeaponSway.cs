using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum process
{ Idle, StartSway, Swaying, StartRewind, EndRewind }

public class WeaponSway : MonoBehaviour
{
    public MyPlayerController pc;
    public process curr;
    public float requierdSwayAmount;
    public AnimationCurve speedChangeSection;
    [Header("좌우 흔들림")]
    public AnimationCurve xPos, yRot, zRot;
    [Header("상하 흔들림")]
    public AnimationCurve yPos, xRot;
    float tickX, tickY, tick;
    public Vector2 exDir;
    public float speedFactor;
    float deltaInput;
    public int Operator, exOperator;

    Quaternion startRot, EndRot;
    private void Awake()
    {
        //Debug.Log($"내적 : { Vector2.Dot(input, exDir)}");
        curr = process.Idle;
    }
    private void Update()
    {
        Vector2 input = pc.myInput.mouseDir;
        float inputMagnitude = input.sqrMagnitude; // 마우스 입력 크기

        // deltaInput = Mathf.Abs(input.x) - Mathf.Abs(exDir.x);
        float swayStartPoint = (exDir == default) ? 1f * speedFactor : (requierdSwayAmount * requierdSwayAmount);

        Debug.Log($"첫 회전 시작 : {(exDir == default)}");
        // 마우스 입력이 있을 때 (최소 요구 달성)
        if (inputMagnitude > swayStartPoint)//swayStartPoint
        {
            // 이전 프레임과 현재 프레임간 입력 차이를 계산 : 변화량에 따라 tick 값 조절
            deltaInput = Mathf.Abs(input.x) - Mathf.Abs(exDir.x);
            float speed = speedChangeSection.Evaluate(deltaInput);

            Operator = (input.x < 0) ? 1 : -1;
            if (Operator != exOperator) // 반대 방향이라면
            {
                  // tick 증감
                tick = tick - Time.deltaTime * speedFactor;
            }
            else
            {
                // tick 증감
                tick = tick + Time.deltaTime * speed;
            }
            
            exOperator = Operator;
        }
        else
        {
            // 마우스 입력이 없을 때 tick을 감소
            tick -= Time.deltaTime *speedFactor ;
        }
        tick = Mathf.Clamp01(tick);
        // Lerp로 보간하여 부드럽게 보이도록 만들기
        float lerpRatio = (tick > 1f) ? 1f : 0.85f;
        transform.localPosition =
            Vector3.Lerp(
                transform.localPosition
                , new Vector3(Operator*xPos.Evaluate(tick),0, 0)
                , lerpRatio
                );
        transform.localRotation =
            Quaternion.Slerp(
                transform.localRotation
                , Quaternion.Euler(0, Operator * yRot.Evaluate(tick), Operator * zRot.Evaluate(tick))
                , lerpRatio
                );

        exDir = Vector2.Lerp(exDir, input, Time.deltaTime * speedFactor); // 현재 프레임을 보간하여 저장
        if (tick == 0 && input.x == 0)
        {
            exDir = default;
        }
    }
}
