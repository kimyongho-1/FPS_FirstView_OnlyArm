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
        // 회전해야할 방향 (LookTarget)
       Quaternion directionToLookTarget = Quaternion.Euler(0, Sr.myInput.YawRotator.rotation.eulerAngles.y, 0);

        // 트랜스폼 회전 (마치 하체가 상체를 따라가는것 같지만 사실 트랜스폼 회전)
        // 실제론 트랜스폼 자체가 회전중이며, 상체는 OnAnimatorIK함수내 LookAt영향으로 고정.
        Sr.transform.rotation = Quaternion.Slerp(Sr.transform.rotation, directionToLookTarget, Time.deltaTime / 0.1f);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 현재 회전중이 끝났음으로 변경
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
