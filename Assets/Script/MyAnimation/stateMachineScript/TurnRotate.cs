using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using static UnityEngine.UI.GridLayoutGroup;

public class TurnRotate : StateMachineBehaviour
{
    public SpineRotate SR;
    float rotateStartTime;
    public float finishedTime ;
    Quaternion startRot;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (SR == null)
        {
            SR = animator.GetComponent<SpineRotate>();
            finishedTime = animator.GetCurrentAnimatorClipInfo(layerIndex).FirstOrDefault(x=>x.clip.name == "RotatePose").clip.length
            * 0.75f;
        }
        SR.anim.SetLayerWeight(AnimLayer.LowerRotate, 1f);
        animator.SetBool("PelvisRotating", true);
        startRot = SR.transform.rotation;
        rotateStartTime = Time.time;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float processingTime = Time.time - rotateStartTime;
        Quaternion destRot = SR.YawRotater.localRotation;
        if (processingTime < finishedTime)
        {
            
            SR.transform.rotation = Quaternion.Slerp(startRot , destRot  , (processingTime / finishedTime) );
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("PelvisRotating",false);
        SR.anim.SetLayerWeight(AnimLayer.LowerRotate,0f);
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
