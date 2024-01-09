using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform CamRoot,GunRoot;

    public Rigidbody rb;
    public CapsuleCollider cap;
    Animator anim;
    public float moveSpeed;

    public float groundDrag;
    public float gravityAmount = -9.81f;
    public float playerGravity = -1f;
    public float gravityForce = 4f;
    public float playerHeight;
    public Transform heightRay;
    public LayerMask groundMask;
    public bool isGrounded;

    public float jumpForce;
    public float jumpCoolDown;
    public bool readyToJump;

    public float xRotation;
    public float yRotation;
    float horizontalInput;
    float verticalInput;

    public float horSensi;
    public float vertSensi;
    Vector3 moveDir;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
    }

    public float mul;
    private void FixedUpdate()
    {
        MovementInput();

        mul = 100f;
        if (!isGrounded)
        { mul = 100f + playerGravity*2; }
        Vector3 move = moveDir * moveSpeed * Time.deltaTime * mul;

        Gravity();
        // ������ �߷� ���� (-y �ʱ�ȭ)
        move += (Vector3.up * playerGravity);
        rb.AddForce(move, ForceMode.Force);
    }
    void MovementInput()
    {
        // �Է°� �ִϸ��̼� ������Ƽ �ʱ�ȭ
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        anim.SetFloat("Side", horizontalInput);
        anim.SetFloat("Forward", verticalInput);

        // �ƹ� �Է¾����� �����̵� ���ϵ��� �ӷ��� 0���� ( y�� �߷����� ���ܵα�)
        if (horizontalInput == 0 && verticalInput == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

        }
        else 
        {
            // �Է�������, �ִ�ӵ��� �Ѿ���� Ȯ�� �� ����
            Vector3 moving = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (moving.sqrMagnitude > moveSpeed * moveSpeed)
            {
                Vector3 limited = moving.normalized * moveSpeed;

                rb.velocity = new Vector3(limited.x, rb.velocity.y, limited.z);
            }

        }

        // ���߿��� �������� �����̴� ��� �̵��ӷ��� ���߱�
        if (!isGrounded)
        { rb.velocity = new Vector3(rb.velocity.x * nerfVelocity, rb.velocity.y, rb.velocity.z * nerfVelocity); }
    }
    public float nerfVelocity = 1f;
    void Gravity()
    {

        // ���� ������, ������ �߷¸� ����
        if (isGrounded)
        {
            playerGravity = -1f;
        }
        // ���߿� ������ ���� �߷� ���� �ø���
        else
        {
            playerGravity += Time.deltaTime * gravityAmount * gravityForce;
        }
    }

    public float Angle; // ���ú����� �����ؾ��� ���� ��������� ����� ���� ����
    void Update()
    {
        // ���� üũ + ������ ��絵 Ȯ��
        if (Physics.Raycast(heightRay.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.2f, groundMask))
        {
            // ������ �븻(����)�� �ϴð��� �������� ���ϱ�
            Angle = Vector3.Angle(Vector3.up, hit.normal);

            // 50���� �Ѵ� ���δ� ���Ȱ� ������ ĸ���� ���̸� Ű�� �̵� ����
            if (Angle > 50f) 
            { 
                cap.height = 1.6f;
            }
            // �׿� �̵��� �ش� ������������ �̵������� ����
            else
            {
                moveDir = Vector3.ProjectOnPlane ((horizontalInput * transform.right + verticalInput * transform.forward).normalized, hit.normal);
                cap.height = 1.4f;
            }
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        // �浹ü ���� ���η� �������� �ʱ�ȭ + ���������� ��絵 ������ ����
        isGrounded = (hit.transform == null || Angle > 50) ? false : true;

        Debug.DrawRay(heightRay.position, Vector3.down * (playerHeight * 0.5f + 0.2f), Color.red);


        yRotation += Input.GetAxis("Mouse X") * horSensi * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation - Input.GetAxis("Mouse Y") * vertSensi * Time.deltaTime, -75f,75f);
        transform.rotation = Quaternion.Euler(0,yRotation,0);
        CamRoot.localRotation = Quaternion.Euler(xRotation,0,0);
        if (Input.GetKey(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump = false;
            jump();
            Invoke(nameof(ResetJump), jumpCoolDown);
        }


        // �������� �߰� (�����Ͻ�)
        if (isGrounded)
        { 
            rb.drag = groundDrag;
        }

        else 
        {
            rb.drag = 0;
        }
    }
    void ResetJump()
    {
        readyToJump = true;
    }
    
    public float footRayLength, offsets;
    public Transform LeftHandElbow, RightHandElbow, leftFoot, rightFoot,
        LeftHandIKTarget, RightHandIKTarget;
    Vector3 leftFootIKpos, rightFootIKpos;
    Quaternion leftFootIKrot, rightFootIKrot;
    void jump()
    {
        rb.velocity = new Vector3(rb.velocity.x,0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce , ForceMode.Impulse);
    }

    void Hand()
    {
        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftHandElbow.position);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, RightHandElbow.position);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        anim.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
    }
    private void OnAnimatorIK(int layerIndex)
    {
        //LookAt();
        Hand();

        if (isGrounded == false) { return; }
        float leftFootWeight = anim.GetFloat("LeftFootWeight");
        float rightFootWeight = anim.GetFloat("RightFootWeight");
        if (Physics.Raycast(leftFoot.position + Vector3.up, -Vector3.up, out RaycastHit hit, 1f + footRayLength))
        {
            leftFootIKpos = hit.point;
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit.normal);
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * leftFootWeight, rotAxis);
            leftFootIKrot = rot;

            
            // Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        //anim.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, leftFootWeight);
        //anim.SetIKHintPosition(AvatarIKHint.LeftKnee, leftHint.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIKrot * anim.GetIKRotation(AvatarIKGoal.LeftFoot));
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKpos + Vector3.up * offsets);

        if (Physics.Raycast(rightFoot.position + Vector3.up, -Vector3.up, out RaycastHit hit2, 1f + footRayLength))
        {
            rightFootIKpos = hit2.point;
            Vector3 rotAxis = Vector3.Cross(Vector3.up, hit2.normal);
            float angle = Vector3.Angle(Vector3.up, hit2.normal);
            Quaternion rot = Quaternion.AngleAxis(angle * rightFootWeight, rotAxis);
            rightFootIKrot = rot;
            //rightFootIKrot = Quaternion.FromToRotation(transform.up, hit2.normal) * transform.rotation;
      
        }

        //anim.SetIKHintPositionWeight(AvatarIKHint.RightKnee, rightFootWeight);
        //anim.SetIKHintPosition(AvatarIKHint.RightKnee, rightHint.position);

        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIKrot * anim.GetIKRotation(AvatarIKGoal.RightFoot));
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKpos + Vector3.up * offsets);

    }
  
}
