using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotTest : MonoBehaviour
{
    public Transform gunPivot, dest;
    public Vector3 p;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        gunPivot.localPosition = gunPivot.localPosition  + p;
    }
}
