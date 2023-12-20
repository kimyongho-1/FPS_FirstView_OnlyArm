using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{

    public static CharacterMovement instance;
    private void Awake()
    {
        instance = this;
    }
    [SerializeField]CharacterController CC;
    [SerializeField] Transform CharacterBody;
    [SerializeField] Transform GroundCheck;

    [Header("")]

    public float X;
    public float Y;

    [Header("")]
    [SerializeField] float WalkSpeed;
    [SerializeField] float RunSpeed;
    [SerializeField] float JumpForce;

    [Header("Gravity")]
    Vector3 GravityVector;
    [SerializeField] float GravityAc = -9.81f;
    [SerializeField] bool IsGrounded;
    [SerializeField] LayerMask GroundLayer;

    private void FixedUpdate()
    {
        Movement();
        Gravity();
    }
    private void Update()
    {
        Jump();
    }

    void Movement()
    {
        X = Input.GetAxis("Horizontal");
        Y = Input.GetAxis("Vertical");

        Vector3 move = CharacterBody.right * X + CharacterBody.forward * Y;
        CC.Move(move*TotalSpeed()*Time.deltaTime);
    }

    void Gravity()
    {
        IsGrounded = Physics.CheckSphere(GroundCheck.position,0.2f, GroundLayer);

        if (!IsGrounded)
        {
            GravityVector.y += GravityAc * Time.deltaTime*Time.deltaTime;
        }
        else if (GravityVector.y < 0)
        { GravityVector.y = -0.15f; }
        
        CC.Move(GravityVector);
    }

    void Jump()
    {
        if (IsGrounded && Input.GetButtonDown("Jump"))
        {
            GravityVector.y = Mathf.Sqrt(JumpForce * -2f * GravityAc / 1000f) ;
        }
    }

    float TotalSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { return RunSpeed; }
        else return WalkSpeed;
    }
}
