using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyBaseController : MonoBehaviour
{
    [HideInInspector] public SpineRotate SR;
    public SMB_Character StateMachine;
    [HideInInspector] public Rigidbody RB;
    public Model FullBodyModel, GunModel, ArmModel;
    
    [HideInInspector] public InputReceiver myInput;
    public bool IsGround;
    public float slopedSpeed;
    public virtual void Awake()
    {
        RB = GetComponent<Rigidbody>();
        myInput = GetComponent<InputReceiver>();
    }
    public virtual Vector3 GroundCheck() { return default; }
    public Transform Look { get { return myInput.Look; } }
    public Transform YawRotator { get { return myInput.YawRotator; } }
    public Transform PitchRotator { get { return myInput.PitchRotator; } }

}
