using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayerController : MyBaseController
{
    SpineRotate0224 SR;
    public MyCapsuleData capsuleData;
    private void Awake()
    {
        SR = transform.GetComponentInChildren<SpineRotate0224>();
        Model = SR.gameObject.GetComponent<Animator>();
    }
}