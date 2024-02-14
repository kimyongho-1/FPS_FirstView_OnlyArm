using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_WeaponSway : MonoBehaviour
{
    public KYH_Mover mover; // mover + ���������� ���� ���콺 �Է°��� ������ ���� ĳ��
    Vector3 startPos;

    private void Awake()
    {
        swayPos = startPos = transform.localPosition;
    }

    public bool usePOSITION = true;
    public bool useROTATION = true;
    Vector3 currPosVelo;
    private void Update()
    {
        if (useROTATION) SwayRot();
        if (usePOSITION) SwayPos();
    }

    Vector3 swayPos;
    public float swayPosForce = 0.015f;
    public float limitDistance = 0.06f;
    public float smoothPosTime = 5;
    void SwayPos()
    {
        swayPos.x = Mathf.Clamp(-swayPosForce * mover.input.GetMouseX(), -limitDistance, limitDistance);
        swayPos.y = Mathf.Clamp(-swayPosForce * mover.input.GetMouseY(), -limitDistance, limitDistance);
        swayPos += new Vector3(startPos.x, startPos.y,0);

        #region Lerp�� �ƴ� SmoothDamp�� ����� ����
        // Mathf.Lerp�Լ��� ��� �������� -> ���������� �����ӵ��� �̵�
        // �����ӵ����� ����ϱ⿡ �ѱ��� ��鸲 sway�� �����ϱ⿣ ������
        // ���� : ���콺�Է��� ���� ���������� �ѱ��� ��鸲�� ���� �پ��°� ���ϱ⿡ 
        // �̋� ������ �̵��ӵ� ref�� ����ϴ� SmoothDamp�� �Լ��� ���� ������鸲�� �´ٰ� �Ǵ�
        // SmoothDamp Ŀ��� ������� ����, ���������� �Ÿ��� ���� �ӵ��� ���ϸ�
        // ���� ���콺�� ȸ����, ���콺 ȸ������ ���� ���������� ������ ������ġ������ õõ�� ���ϵ��� ����
        #endregion
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, swayPos, ref currPosVelo, Time.deltaTime * smoothPosTime);
    }



    public float smoothRotTime = 3f;
    public float swayRotForce = 8f;
    public float limitRotation = 25f;
    void SwayRot()
    {
        Vector3 swayRot = Vector3.zero;
        swayRot.x = Mathf.Clamp(-swayRotForce * mover.input.GetMouseY(), -limitRotation, limitRotation);
        swayRot.y = Mathf.Clamp(-swayRotForce * mover.input.GetMouseX(), -limitRotation, limitRotation);
        swayRot.z = swayRot.y;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(swayRot), Time.deltaTime * smoothRotTime);

    }


}