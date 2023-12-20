using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnimationController))]
public class AnimationController : MonoBehaviour
{
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void SetBool(string animID, bool animBool)
    {
        anim.SetBool(animID,animBool);
    }
    public void SetTrigger(string animID)
    {
        anim.SetTrigger(animID);
    }
    public void EndFire()
    {
        WeaponManager.Instance.EndFire();
    }
    public void EndReload()
    {
        WeaponManager.Instance.EndReload();
    }
    public void WeaponDown()
    {
        WeaponManager.Instance.CloseWeapon();
    }

    public void SetAvailablity(int index)
    {
        WeaponManager.Instance.Availablity = index == 0 ? false : true;
    }
}
