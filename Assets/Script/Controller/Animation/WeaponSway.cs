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
    [Header("�¿� ��鸲")]
    public AnimationCurve xPos, yRot, zRot;
    [Header("���� ��鸲")]
    public AnimationCurve yPos, xRot;
    float tickX, tickY, tick;
    public Vector2 exDir;
    public float speedFactor;
    float deltaInput;
    public int Operator, exOperator;

    Quaternion startRot, EndRot;
    private void Awake()
    {
        //Debug.Log($"���� : { Vector2.Dot(input, exDir)}");
        curr = process.Idle;
    }
    private void Update()
    {
        Vector2 input = pc.myInput.mouseDir;
        float inputMagnitude = input.sqrMagnitude; // ���콺 �Է� ũ��

        // deltaInput = Mathf.Abs(input.x) - Mathf.Abs(exDir.x);
        float swayStartPoint = (exDir == default) ? 1f * speedFactor : (requierdSwayAmount * requierdSwayAmount);

        Debug.Log($"ù ȸ�� ���� : {(exDir == default)}");
        // ���콺 �Է��� ���� �� (�ּ� �䱸 �޼�)
        if (inputMagnitude > swayStartPoint)//swayStartPoint
        {
            // ���� �����Ӱ� ���� �����Ӱ� �Է� ���̸� ��� : ��ȭ���� ���� tick �� ����
            deltaInput = Mathf.Abs(input.x) - Mathf.Abs(exDir.x);
            float speed = speedChangeSection.Evaluate(deltaInput);

            Operator = (input.x < 0) ? 1 : -1;
            if (Operator != exOperator) // �ݴ� �����̶��
            {
                  // tick ����
                tick = tick - Time.deltaTime * speedFactor;
            }
            else
            {
                // tick ����
                tick = tick + Time.deltaTime * speed;
            }
            
            exOperator = Operator;
        }
        else
        {
            // ���콺 �Է��� ���� �� tick�� ����
            tick -= Time.deltaTime *speedFactor ;
        }
        tick = Mathf.Clamp01(tick);
        // Lerp�� �����Ͽ� �ε巴�� ���̵��� �����
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

        exDir = Vector2.Lerp(exDir, input, Time.deltaTime * speedFactor); // ���� �������� �����Ͽ� ����
        if (tick == 0 && input.x == 0)
        {
            exDir = default;
        }
    }
}
