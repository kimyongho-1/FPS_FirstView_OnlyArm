using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class FootIKData
{
    public float heightOffset;
    public float rayHeightOffset, rayLengeth;
    public Transform[] raycastPivot;
    
    public string propertyName, tag;
    Nullable<RaycastHit> hit;
    public float feetHeight
    {
        get
        { return anim.GetBoneTransform(bodyPart).position.y; }
    }
    public float GetWeight 
    { get { return anim.GetFloat(propertyName); } }
    public Nullable<RaycastHit> GetHit()
    {
        return hit;
    }
    public Vector3 GetNormal { get { return hit.Value.normal;  } }
    Animator anim;
    [HideInInspector]public AvatarIKGoal bodyPartGoal;
    public HumanBodyBones bodyPart;
    public void Init(Animator a)
    {
        anim = a;
        bodyPartGoal = (bodyPart == HumanBodyBones.RightFoot) ? AvatarIKGoal.RightFoot : AvatarIKGoal.LeftFoot;

    }
    

    public void CheckRay(LayerMask groundLayer, int idx = 0)
    {
        Vector3 start = raycastPivot[idx].position - raycastPivot[idx].up * rayHeightOffset;
        Vector3 end = start + raycastPivot[idx].up * (rayLengeth);
        Debug.DrawLine(start, end, Color.red);
        if ( Physics.Raycast(raycastPivot[idx].position - raycastPivot[idx].up * rayHeightOffset
            , raycastPivot[idx].up, out RaycastHit hits ,  rayLengeth, groundLayer))
        {
            hit = hits;   
            return ;
        }
        else
        {
            idx++;
            if (idx == raycastPivot.Length) 
            {
                hit = null;
                return ; 
            }

            CheckRay(groundLayer, idx);
        }
    }
}
