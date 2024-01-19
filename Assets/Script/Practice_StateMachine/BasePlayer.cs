using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInputData))]
public class BasePlayer : MonoBehaviour
{
    [field: SerializeField] public CapsuleColliderUtility ColliderUtill { get; private set; } = new CapsuleColliderUtility();
    [field: SerializeField] public Layers playerLayers { get; private set; } = new Layers();
    public bool LeftShift;
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
        ColliderUtill.Initialize(this.gameObject);
        ColliderUtill.CalculateCapsuleColliderDimension();
    }
    private void OnValidate()
    {
        ColliderUtill.Initialize(this.gameObject);
        ColliderUtill.CalculateCapsuleColliderDimension();
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
