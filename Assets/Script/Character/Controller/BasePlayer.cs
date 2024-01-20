using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    PlayerStateMachine StateMachine { get; set; }
    public Rigidbody RB { get; private set; }
    [field:SerializeField]public Animator Anim { get; private set; }
    [field: SerializeField] public FixedPlayerSettingData SettingData;

    public Vector2 movement;
    public Vector2 direction;

    private void Awake()
    {
        StateMachine = new PlayerStateMachine(this);
        RB = GetComponent<Rigidbody>();
        SettingData.Init(Anim);
    }

    private void Update()
    {
        StateMachine.CurrentState.HandleInput();
        StateMachine.CurrentState.Update();
    }
    private void FixedUpdate()
    {
        StateMachine.CurrentState.FixedUpdate();
    }

}
