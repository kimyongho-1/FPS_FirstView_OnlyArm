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

    [SerializeField] AnimationController AC;
    [SerializeField] string Fire1_ID;
    [SerializeField] string Fire2_ID;
    [SerializeField] string Reload1_ID;
    [SerializeField] string WeaponDown_ID;

    [SerializeField] Transform WeaponTransform;

    [SerializeField] bool Fire;
    [SerializeField] int CurrentAmmo;


    [SerializeField] float fireFraq;
    float FireCounter;
    RaycastHit FireRayCast;
    [SerializeField] float fireRange;

    [Header("reload Variable")]
    [SerializeField] bool Reload;
    [SerializeField] int MaxAmmo;
    [SerializeField] int TotalAmmo;
    [SerializeField] AmmoType Type;
    public enum AmmoType
    { 
        _5_56,
        _7_62,
        _9mm
    }
    [SerializeField] int _5_56;
    [SerializeField] int _9mm; 
    [SerializeField] int _7_62;
    void SetTotalAmmo()
    { 
        
    }

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
        if (CurrentAmmo <= 1)
        { AC.SetBool(Fire1_ID, Fire); }
        else
        {
            AC.SetBool(Fire1_ID, Fire); 
        }

        CurrentAmmo--;
        FireCounter = Time.time + fireFraq;

        if (Physics.Raycast(CameraController.Instance.Camera.localPosition , CameraController.Instance.Camera.forward 
            , out FireRayCast, fireRange))
        {// 현재 카메라 높이값이 안맞음
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
