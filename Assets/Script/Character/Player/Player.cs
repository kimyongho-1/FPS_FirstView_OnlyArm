using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(PlayerInputs))]
public class Player : MonoBehaviour
{
    public bool useLerpCamRotate = false;
    public Transform mainCamera { get; private set; }
    PlayerMovementStateMachine movementStateMachine;
    public Rigidbody rb { get; private set; }
    public PlayerInputs Inputs { get; set; }
    private void Awake()
    {
        mainCamera = Camera.main.transform;
        Inputs = GetComponent<PlayerInputs>();
        rb = GetComponent<Rigidbody>();
        movementStateMachine = new PlayerMovementStateMachine(this);

    }
    private void Start()
    {
        movementStateMachine.ChangeState(movementStateMachine.IdleState);
    }
    private void Update()
    {
        movementStateMachine.UserInput();
        movementStateMachine.Update();
    }
    private void FixedUpdate()
    {
        movementStateMachine.PhysicsUpdate();
    }
}
