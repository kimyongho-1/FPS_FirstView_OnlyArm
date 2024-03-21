using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : Model
{
    public Transform Pivot;
    public float aimingSpeed = 2f;
    public Camera handCam;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public Vector3 AimingPos; // fpsCamÀÇ ÀÚ¸®
    public Quaternion AimingRot;
    public Vector3 HipFirePivotPos, AimingPivotPos;

    public float aimingRatio;
    public override void Lerp(bool isAiming)
    {
        // isAiming = true; anim.SetBool("HoldBreath",true);
        aimingRatio = (aimingRatio + ((isAiming) ? Time.deltaTime*aimingSpeed : -Time.deltaTime * aimingSpeed));
        aimingRatio = Mathf.Clamp01(aimingRatio);
        Pivot.transform.localPosition = Vector3.Lerp(HipFirePivotPos, AimingPivotPos, aimingRatio);
        handCam.transform.localPosition = Vector3.Lerp(Vector3.zero, AimingPos ,aimingRatio);
        handCam.transform.localRotation = Quaternion.Slerp(Quaternion.identity, AimingRot, aimingRatio);
    }
}
