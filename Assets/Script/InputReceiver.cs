using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    public Transform YawRotator, PitchRotator, LookTarget;
    public float power;
    public float yawAmount;
    private void Awake()
    {
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            yawAmount -= Time.deltaTime * power;
        }

        if (Input.GetKey(KeyCode.D))
        { yawAmount += Time.deltaTime * power; }

        if (yawAmount < 0f) { yawAmount += 360f; }
        if (yawAmount > 360f) { yawAmount = yawAmount % 360f; }

        YawRotator.localRotation = Quaternion.Euler(Vector3.up * yawAmount );
        
    }
}
