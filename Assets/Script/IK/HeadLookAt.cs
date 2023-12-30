using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class IKhandling : MonoBehaviour
{
    public Transform LookTarget;

    public float lookWeight = 1;
    public float bodyWeight=1;
    public float headWeight=1;
    public float clampWeight;
    void LookAt()
    {
        if (LookTarget == null)
        { return; }

        anim.SetLookAtWeight(lookWeight, bodyWeight, headWeight,0,clampWeight);
        anim.SetLookAtPosition(LookTarget.position);
    }

}