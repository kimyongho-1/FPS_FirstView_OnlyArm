using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IK_fORRecording : MonoBehaviour
{
    public Transform LT, RT, LE, RE;
    Animator anim;
    public bool TurnFingerIK;
    public Hand LeftHand ;
    public Hand RightHand;
    private void Awake()
    { 
        anim = GetComponent<Animator> ();
        LeftHand.anim = anim;
        RightHand.anim = anim;
    }
    private void OnAnimatorIK(int layerIndex)
    {  // Fingers
        if (TurnFingerIK == true)
        {
            LeftHand.Grap();
            RightHand.Grap();
        }

        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.RightHand, RT.position);
        anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
        anim.SetIKRotation(AvatarIKGoal.RightHand, RT.rotation);
        //anim.SetBoneLocalRotation(HumanBodyBones.RightHand, RT.localRotation);

        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1f);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, RE.position);

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, LT.position);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
        //anim.SetBoneLocalRotation(HumanBodyBones.LeftHand, LT.localRotation);
        anim.SetIKRotation(AvatarIKGoal.LeftHand, LT.rotation);

        anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1f);
        anim.SetIKHintPosition(AvatarIKHint.LeftElbow, LE.position);

      
    }

}
public enum handParts 
{
    Thumb, Index, Middle, Ring, Little
}

[System.Serializable]
public class Hand
{
    public Animator anim;
    public List<handPalm> list = new List<handPalm>();

    public void Grap()
    {
        foreach (var item in list)
        {
            foreach (var handPalm in item.eachFinger)
            {
                handPalm.SetRatio(anim);
            }
        }
    }
  
}
[System.Serializable]
public class handPalm
{
    public handParts idx;
    public fingerRatio[] eachFinger;

}
[System.Serializable]
public class fingerRatio
{
    public HumanBodyBones parts;
    public Quaternion rot;
    [Range(0,1f)]public float ratio;
    public void SetRatio( Animator a)
    {
        AvatarIKGoal goal = (parts.ToString().Contains("Left") == true) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
        a.SetBoneLocalRotation(
            parts ,
            Quaternion.Slerp(
                a.GetBoneTransform(parts).rotation ,
                rot, ratio)) ;
    }
}