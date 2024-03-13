using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.ProBuilder.MeshOperations;
using static UnityEngine.UI.GridLayoutGroup;

public class TurnRotate : StateMachineBehaviour
{
    public MyBaseController BC;
    float rotateStartTime;
    public float finishedTime ;
    float r;
    Quaternion startRot;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (BC == null)
        {
            string clipName =stateInfo.IsName("RightRotatePose") ? "TurnRight" : "TurnLeft";// "RotatePose";//
            BC = animator.GetComponentInParent<MyBaseController>();
            var clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
            finishedTime = clipInfo.FirstOrDefault(x=>x.clip.name.Contains(clipName)).clip.length / stateInfo.speed;
        }
        animator.SetLayerWeight(AnimLayer.LowerRotate, 1f);
        animator.SetBool("PelvisRotating", true);
        startRot = animator.transform.rotation;
        r = animator.transform.rotation.eulerAngles.y;
        rotateStartTime = Time.time;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float processingTime = Time.time - rotateStartTime;
        Quaternion destRot = BC.YawRotator.localRotation;

        
        if (processingTime < finishedTime)
        {
            animator.transform.rotation =  Quaternion.Slerp(startRot , destRot  , (processingTime / finishedTime) );
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("PelvisRotating",false);
        animator.SetLayerWeight(AnimLayer.LowerRotate,0f);
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
