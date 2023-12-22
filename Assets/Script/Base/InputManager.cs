using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Movement Axis")]
    [SerializeField] private string forwardAxis = "Vertical";
    [SerializeField] private string sideAxis = "Horizontal";

    /// <summary>
    /// ������Ƽ�� ���
    /// </summary>
    float forward; public float Forward { get { return forward; } }
    /// <summary>
    /// ������Ƽ�� ���
    /// </summary>
    float side; public float Side { get { return side; } }

    private void Update()
    {
        HandleInput();
    }
    void HandleInput()
    { 
        forward = Input.GetAxis(forwardAxis);
        side = Input.GetAxis(sideAxis); 
    }
}
