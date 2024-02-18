using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningState : StateMachineBehaviour
{
    public SpineRotate Sr;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Sr == null) { Sr = animator.GetComponent<SpineRotate>(); }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ȸ���ؾ��� ���� (LookTarget)
       Quaternion directionToLookTarget = Quaternion.Euler(0, Sr.myInput.YawRotator.rotation.eulerAngles.y, 0);

        // Ʈ������ ȸ�� (��ġ ��ü�� ��ü�� ���󰡴°� ������ ��� Ʈ������ ȸ��)
        // ������ Ʈ������ ��ü�� ȸ�����̸�, ��ü�� OnAnimatorIK�Լ��� LookAt�������� ����.
        Sr.transform.rotation = Quaternion.Slerp(Sr.transform.rotation, directionToLookTarget, Time.deltaTime / 0.1f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // ���� ȸ������ ���������� ����
        animator.SetBool("CurrentTurning", false);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
