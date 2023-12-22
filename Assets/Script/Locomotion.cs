using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    public InputManager inputManager = default;
    private void Awake()
    {
        ForwardID = Animator.StringToHash("Forward");
        SideID = Animator.StringToHash("Side");
    }

    Vector3 movementDir;
    float inputAmount;

    private void FixedUpdate()
    {
        UpdateMovementInput();
        rb.velocity = (movementDir * movementSpeed * inputAmount + gravity);
        UpdatePhysics();
        UpdateAnim();
    }
    void UpdateMovementInput() 
    {
        movementDir = Vector3.zero;
        Vector3 forward = inputManager.Forward * transform.forward;
        Vector3 side = inputManager.Side * transform.right;

        movementDir = (forward + side).normalized;
        float inputMagnitude = Mathf.Abs(inputManager.Forward) + Mathf.Abs(inputManager.Side);
        inputAmount = Mathf.Clamp01(inputMagnitude);
    }

    [Header("Anim")]
    [SerializeField]Animator anim;
    int ForwardID;
    int SideID;
    void UpdateAnim()
    {
        anim.SetFloat(ForwardID, inputManager.Forward);
        anim.SetFloat(SideID, inputManager.Side);
    }

    [SerializeField] Rigidbody rb = default;
    [SerializeField] CapsuleCollider col = default;
    [SerializeField] float offsetFlootY = 0.4f;
    [SerializeField] float movementSpeed = 3f;

    Vector3 raycastFloorPos;
    Vector3 combinedRaycast;
    Vector3 gravity;
    Vector3 floorMovement;
    float groundRayLength;

    void UpdatePhysics()
    {
        // �ٴ�Ȯ���� ���̸� ����� ���� ���ϱ� : ĸ���ݶ��̴� �߾ӿ��� ��������, �ݶ��̴��������� + offset�� �߰�
        groundRayLength = col.height * 0.5f + offsetFlootY;

        // ���̸� �� ������, �ݶ��̴��� xz�� ���� ���߱⿡ 0���� offsetxz�� ����
        if (FloorRaycasts(0, 0, groundRayLength).transform == null)
        {
            // ���� �ٴ��� ���ٸ�, �߷� ����
            gravity += (Vector3.up * Physics.gravity.y * Time.fixedDeltaTime);
        }

        // �̵�����+�ӵ� ������ �ӷ°��
        rb.velocity = (movementDir * movementSpeed * inputAmount + gravity);
        floorMovement = new Vector3(rb.position.x, FindFloor().y, rb.position.z);
        // �ٴ�Ȯ���Ͽ��ٸ�
        if (FloorRaycasts(0, 0, groundRayLength).transform != null
            && floorMovement != rb.position)
        {
            rb.MovePosition(floorMovement);
            gravity.y = 0;
        }


    }
    /// <summary>
    /// Col�߾ӿ��� �ٴ����� ���̸� ���
    /// </summary>
    /// <param name="offsetX"></param>
    /// <param name="offsetZ"></param>
    /// <param name="raycastLen"></param>
    /// <returns></returns>
    RaycastHit FloorRaycasts(float offsetX, float offsetZ, float raycastLen)
    {
        RaycastHit hit;

        // ���� ������ ���ϱ�
        raycastFloorPos = transform.TransformPoint(0 + offsetX, col.center.y, offsetZ);
        Debug.DrawRay(raycastFloorPos, -Vector3.up * raycastLen, Color.red , 0.25f);
        Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, raycastLen);

        return hit;
    }
    Vector3 FindFloor()
    {
        float raycastWidth = 0.25f;
        int floorAvearge = 1;

        combinedRaycast = FloorRaycasts(0,0,groundRayLength).point;
        floorAvearge += (GetFloorAverage(raycastWidth, 0)) +
            GetFloorAverage(-raycastWidth, 0) + GetFloorAverage(0, raycastWidth)
             + GetFloorAverage(0, -raycastWidth);
        return combinedRaycast / floorAvearge;
    }
    int GetFloorAverage(float offsetX, float offsetZ)
    {
        if (FloorRaycasts(offsetX, offsetZ, groundRayLength).transform != null)
        {
            combinedRaycast += FloorRaycasts(offsetX, offsetZ, groundRayLength).point;
            return 1;
        }
        else return 0;
    }
}
