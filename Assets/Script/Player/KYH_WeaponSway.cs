using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_WeaponSway : MonoBehaviour
{
    public KYH_Mover mover; // mover + 무버에서는 당장 마우스 입력값을 변수로 빼서 캐싱
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

        #region Lerp가 아닌 SmoothDamp를 사용한 이유
        // Mathf.Lerp함수의 경우 선형보간 -> 목적지까지 일정속도로 이동
        // 일정속도만을 사용하기에 총기의 흔들림 sway를 구현하기엔 부적합
        // 이유 : 마우스입력이 점점 없어질수록 총기의 흔들림도 점차 줄어드는걸 원하기에 
        // 이떄 현재의 이동속도 ref를 사용하는 SmoothDamp의 함수가 현재 무기흔들림에 맞다고 판단
        // SmoothDamp 커브드 방식으로 보간, 목적지와의 거리에 따라 속도가 변하며
        // 현재 마우스로 회전시, 마우스 회전값이 점점 없어질수록 본연의 시작위치값으로 천천히 변하도록 유도
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