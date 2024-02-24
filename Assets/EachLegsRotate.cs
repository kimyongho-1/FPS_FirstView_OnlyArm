using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class EachLegsRotate : StateMachineBehaviour
{
    public RootSpin RS;
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (RS == null) { RS = animator.GetComponentInParent<RootSpin>(); }
        animator.GetComponent<CharRot>().animationStartTime = Time.time;
        animator.GetComponent<CharRot>().dirY = animator.GetFloat("y");
        RS.addRotate = 0;
        RS.sideRotating = 0;
    }
    
    private float animationStartTime;
    private float animationDuration = 0.5f; // 30프레임 동안 진행되는 시간 (초)
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("PelvisRotating", false);
        
        RS.sideRotating =RS.addRotate;
    }

}
