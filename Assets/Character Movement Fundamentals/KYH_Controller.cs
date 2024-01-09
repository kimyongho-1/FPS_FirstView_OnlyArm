using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_Controller : MonoBehaviour
{
    Animator anim;
    KYH_input input;
    KYH_Mover mover;
    
    public float moveSpeed;
    public float jumpForce;
    public float gravityForce;
    public bool isGrounded; 
    Transform tr;

    float currentVerticalSpeed; // ���� ������ �����ӷ� (���Ʒ�)
    Vector3 lastVelocity; // ���� �����ӿ� ����� �ӷ�

    void Start()
    {
        tr = transform;
        mover = GetComponent<KYH_Mover>();
        anim = mover.anim;
        input = GetComponent<KYH_input>();
    }
    void FixedUpdate()
    {
        #region �������� ���� Ȯ��
        // ���� ĳ���Ͱ� ���� ���� �������� Ȯ��
        mover.CheckGround();

        // ���� �����ӿ� ��Ʈ�ѷ��󿡼� ������ �ƴϾ�����, ���� ĳ���Ͱ� ���� �������¶��
        if (isGrounded == false && mover.isGrounded == true)
        { OnGroundContact(lastVelocity); }

        // ���� ĳ���Ͱ� ���� �ִ��� ���¸� �ʱ�ȭ
        isGrounded = mover.isGrounded;

        #endregion

        #region �߷� �ۿ�
        if (!isGrounded)
        {
            // ���߿� ������, �߷°�����ŭ ���� �ӷ��� �����ֱ�
            currentVerticalSpeed -= gravityForce * Time.deltaTime;
        }
        else
        {
            // ���� �������¸�, �߷� �߰� X
            if (currentVerticalSpeed <= 0f)
                currentVerticalSpeed = 0f;
        }
        #endregion

        #region ����
        // ���� ���� �������¿��� Space �Է½� ���� ����
        if (isGrounded && input.IsJumpKeyPressed())
        {
            OnJumpStart();
            currentVerticalSpeed = jumpForce;
            isGrounded = false;
        }
        #endregion

        #region �̵��ӵ� ����
        // ĳ���� �̵��ӵ�
        Vector3 _velocity = Vector3.zero;
        // �̵�Ű �Է°����� WASD���� �ӷ� ����
        // �������� �������� Ȯ������ ���� �ӷ� ����
        _velocity += CalculateMovementDirection() * moveSpeed + tr.up * currentVerticalSpeed;
        #endregion

        // ���������Ӌ� ������� ���� �ӷ� ����
        lastVelocity = _velocity;

        // ���� �̵�
        mover.SetVelocity(_velocity);
    }

    /// <summary>
    /// �̵��ӵ� ���
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateMovementDirection()
    {
        if (input == null)
        { Debug.Log("Character Input Is NULL!!"); return Vector3.zero; }

        // ���� ĳ������ �ٶ󺸴� ���⿡ �°� �Է°� �ʱ�ȭ
        Vector3 _direction = Vector3.zero;
        float side = input.GetHorizontalMovementInput();
        float forward = input.GetVerticalMovementInput();
        _direction += tr.right * side;
        _direction += tr.forward *forward;
        anim.SetFloat("Forward", forward); anim.SetFloat("Side", side);
        // �밢�� Ű�Է��Ͻ� 1+1 = 2�� �ӷ����̰� ����⿡, ���ȭ�ϱ�
        if (_direction.magnitude > 1f)
        { _direction.Normalize(); }

        return _direction;
    }

    /// <summary>
    /// ĳ���Ͱ� ���߿��� ���� �������ڸ��� ������ �̺�Ʈ ��� ȣ��
    /// </summary>
    /// <param name="_collisionVelocity"></param>
    void OnGroundContact(Vector3 _collisionVelocity)
    {
        // �̺�Ʈ �Լ� ȣ��
    }
    
    /// <summary>
    /// �������۽� �̺�Ʈ ����� ȣ��
    /// </summary>
    void OnJumpStart()
    {
       // JUMP�� �̺�Ʈ�Լ�
    }
}
