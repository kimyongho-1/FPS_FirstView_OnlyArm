using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public abstract class SpineRotate : MonoBehaviour
{
    [HideInInspector] public MyBaseController PC;
    [HideInInspector] public InputReceiver myInput;
    public Transform Look { get { return PC.Look; } }
    public Transform YawRotater { get { return PC.YawRotator; } }
    public Transform PitchRotator { get { return PC.PitchRotator; } }
    [HideInInspector] public Animator anim;


   

    public abstract void Init(MyBaseController p, InputReceiver pi);

    public float aimRatio = 0;
    public abstract void ModelUpdate();

}
