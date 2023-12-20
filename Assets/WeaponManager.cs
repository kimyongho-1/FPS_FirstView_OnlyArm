using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public bool Availablity;
    [SerializeField] bool Fire;
    [SerializeField] bool Reload;

    [SerializeField] AnimationController AC;
    [SerializeField] string Fire1_ID;
    [SerializeField] string Fire2_ID;
    [SerializeField] string Reload1_ID;
    [SerializeField] string WeaponDown_ID;

    [SerializeField] Transform WeaponTransform;

    [SerializeField] int CurrentAmmo;
    [SerializeField] int MaxAmmo;


    [SerializeField] float fireFraq;
    float FireCounter;
    RaycastHit FireRayCast;
    [SerializeField] float fireRange;
    private void Update()
    {
        Inputs();
    }
    void Inputs()
    {
        WeaponTransform.localRotation = MouseLook.Instance.cameraParent.localRotation;
        if (Input.GetMouseButton(0) && !Reload && CurrentAmmo > 0 && Time.time > FireCounter && Availablity )
        {
            StartFire();
        }
        if (Fire == false && Input.GetKeyDown(KeyCode.R))
        { StartReload(); }
    }
    public void StartFire()
    {
        Fire = true;
        AC.SetBool(Fire1_ID, Fire);
        CurrentAmmo--;
        FireCounter = Time.time + fireFraq;

        if (Physics.Raycast(CameraController.Instance.Camera.localPosition , CameraController.Instance.Camera.forward 
            , out FireRayCast, fireRange))
        {// ���� ī�޶� ���̰��� �ȸ���
            print(FireRayCast.transform.name);
        }
    }
    public void EndFire()
    {
        Fire = false;
        AC.SetBool(Fire1_ID,Fire);
    }
    public void StartReload()
    { 
        Reload = true;
        AC.SetBool(Reload1_ID,Reload);
    }
    public void EndReload()
    {
        Reload = false;
        AC.SetBool(Reload1_ID, Reload);
    }
    public void CloseWeapon()
    { }
}
