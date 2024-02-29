using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyBaseController : MonoBehaviour
{
    public SMB_Character StateMachine;
    [HideInInspector] public Rigidbody RB;
    [HideInInspector] public Animator Model;
    [HideInInspector] public InputReceiver myInput;
    public virtual void Awake()
    {
        RB = GetComponent<Rigidbody>();
        myInput = GetComponent<InputReceiver>();
        Model = GetComponent<Animator>();
    }
    public Transform Look { get { return myInput.Look; } }
    public Transform YawRotator { get { return myInput.YawRotator; } }
    public Transform PitchRotator { get { return myInput.PitchRotator; } }

}
