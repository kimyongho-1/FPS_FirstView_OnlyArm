using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachLegsRotate : StateMachineBehaviour
{
    public RootSpin RS; Quaternion dir;
    float rotateAmount;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (RS == null) { RS = animator.GetComponentInParent<RootSpin>(); }
        dir = RS.YawRotator.localRotation;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Quaternion ex = RS.transform.rotation;
        Quaternion rot = Quaternion.Lerp(RS.transform.rotation, dir , Time.deltaTime * 5f);

        RS.transform.rotation = rot;
        Quaternion amount = Quaternion.Inverse(ex) * rot;
    

        RS.YawRotator.localRotation  = Quaternion.Inverse(amount) * RS.YawRotator.localRotation ;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("PelvisRotating", false);
    }

}
