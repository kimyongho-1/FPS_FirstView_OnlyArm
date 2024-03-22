using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public UserConfigureData userInputData; // ����ڰ� ���������� �����Ϳ���

    public float walkSpeed, runSpeed;
    public float pitchVal, yawVal;
    public Vector2 pitchAngle;
    public Transform Look, YawRotator, PitchRotator;
    public float gravityAmount = 4f; // ������ ����
    public float gravityForce = 1f; // ���� ����
    public float animationSmoothTime = 0.1f;
    public float mouseX { get { return userInputData.mouseX; } }
    public float mouseY { get { return userInputData.mouseY; } }
    public Vector3 movement;
    public Vector2 dir;
    public Vector2 mouseDir;
    public float rotSpeedLimit;
    public void MovementInput(float speed)
    {
        // Ű�Է�
        dir.y = Input.GetAxis("Vertical");
        dir.x = Input.GetAxis("Horizontal");
        // �ӷ� �ʱ�ȭ
        movement = (YawRotator.forward * dir.y + YawRotator.right * dir.x).normalized * speed;
    }
    public void DirectionInput()
    {
        mouseDir.x = Input.GetAxis("Mouse X");// Mathf.Clamp(Input.GetAxis("Mouse X") * mouseX, -rotSpeedLimit, rotSpeedLimit);
        mouseDir.y = Input.GetAxis("Mouse Y");// Mathf.Clamp(Input.GetAxis("Mouse Y") * mouseY ,-rotSpeedLimit, rotSpeedLimit);
        yawVal += mouseDir.x * mouseX * Time.deltaTime;
        yawVal = Mathf.Repeat(yawVal, 360f);
        pitchVal = Mathf.Clamp(pitchVal - (mouseDir.y * mouseY * Time.deltaTime), pitchAngle.x, pitchAngle.y);

        YawRotator.localRotation = Quaternion.AngleAxis(yawVal, Vector3.up);
        PitchRotator.localRotation = Quaternion.AngleAxis( pitchVal, Vector3.right);
    }

}
