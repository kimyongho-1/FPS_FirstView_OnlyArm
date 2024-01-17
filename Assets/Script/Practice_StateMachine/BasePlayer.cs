using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputData))]
public class BasePlayer : MonoBehaviour
{
    public Vector2 movementInput;
    public Vector2 directionInput;
    public Rigidbody RB { get; private set; }
    public PlayerInputData inputData { get; private set; }
    PlayerControlStateMachine stateMachine;
    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        stateMachine = new PlayerControlStateMachine(this);
        inputData = GetComponent<PlayerInputData>();
    }
    void Start()
    {
        stateMachine.ChangeState(stateMachine.IdleState);
    }

    void Update()
    {
        stateMachine.UserInput();
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }
}
