using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullBodyModel : Model
{
    public override void Lerp(bool b)
    {
        throw new System.NotImplementedException();
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();    
    }
}