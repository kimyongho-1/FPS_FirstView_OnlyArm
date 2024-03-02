using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public UserConfigureData userInputData; // 사용자가 수정가능한 데이터영역

    
    public float pitchVal, yawVal;
    public Vector2 pitchAngle;
    public Transform Look, YawRotator, PitchRotator;
    public float gravityAmount = 4f; // 고정을 유지
    public float gravityForce = 1f; // 변경 가능
    
    
    public float mouseX { get { return userInputData.mouseX; } }
    public float mouseY { get { return userInputData.mouseY; } }
    public Vector3 movement;
    public void MovementInput(float speed)
    {
        movement = Vector3.zero;
        float forwward = Input.GetAxis("Vertical") ;
        float side = Input.GetAxis("Horizontal") ;
        
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
