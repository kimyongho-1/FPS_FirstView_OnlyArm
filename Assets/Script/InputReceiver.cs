using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public Transform YawRotator, PitchRotator, LookTarget;
    public float power;
    public float yawAmount,pitchAmount;
    public Vector2 pitchAngle;
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            yawAmount -= Time.deltaTime * power;
        }

        if (Input.GetKey(KeyCode.D))
        { yawAmount += Time.deltaTime * power; }

        if (Input.GetKey(KeyCode.W))
        {
            pitchAmount -= Time.deltaTime * power;
        }
        if (Input.GetKey(KeyCode.S))
        {
            pitchAmount += Time.deltaTime * power;
        }

        yawAmount = Mathf.Repeat(yawAmount, 360f);
        pitchAmount = Mathf.Clamp(pitchAmount, pitchAngle.x, pitchAngle.y);

        PitchRotator.localRotation = Quaternion.Euler(Vector3.right * pitchAmount);
        YawRotator.localRotation = Quaternion.Euler(Vector3.up * yawAmount );
        
    }
}
