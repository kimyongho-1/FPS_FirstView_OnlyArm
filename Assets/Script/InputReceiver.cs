using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public UserConfigureData userInputData; // ����ڰ� ���������� �����Ϳ���

    
    public float pitchVal, yawVal;
    public Vector2 pitchAngle;
    public Transform Look, YawRotator, PitchRotator;
    public float gravityAmount = 4f; // ������ ����
    public float gravityForce = 1f; // ���� ����
    
    
    public float mouseX { get { return userInputData.mouseX; } }
    public float mouseY { get { return userInputData.mouseY; } }
    public Vector3 movement;
    public void MovementInput(float speed)
    {
        // �ӷ� �ʱ�ȭ
        movement = Vector3.zero;
        // Ű�Է�
        float forwward = Input.GetAxis("Vertical");
        float side = Input.GetAxis("Horizontal");
        movement += (YawRotator.forward * forwward + YawRotator.right * side).normalized * speed ;
    }
    public void DirectionInput()
    {
        yawVal += Input.GetAxis("Mouse X") * mouseX * Time.deltaTime;
        pitchVal = Mathf.Clamp(pitchVal - Input.GetAxis("Mouse Y") * mouseY * Time.deltaTime, -85f, 85f);

        yawVal = Mathf.Repeat(yawVal, 360f);
        pitchVal = Mathf.Clamp(pitchVal, pitchAngle.x, pitchAngle.y);

        YawRotator.localRotation = Quaternion.Euler(Vector3.up * yawVal);
        PitchRotator.localRotation = Quaternion.Euler(Vector3.right * pitchVal);
    }
}
