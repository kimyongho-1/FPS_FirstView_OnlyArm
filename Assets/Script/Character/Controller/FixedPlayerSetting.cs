using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FixedPlayerSettingData
{
    Animator anim;
    int RunID, CrouchID;
    public FixedPlayerSettingData() { }
    public void Init(Animator A) 
    {
        RunID = Animator.StringToHash("Run");
        CrouchID = Animator.StringToHash("Crouch");
        anim = A; 
    }

    [field:SerializeField] public float moveSpeed = 2f;

    [field: SerializeField] [Range(0f, -90f)] public float minXrot;
    [field: SerializeField] [Range(0f, 90f)] public float maxXrot;
    [field: SerializeField] public float mouseSensiX { get; set; }
    [field: SerializeField] public float mouseSensiY { get; set; }

    public void InvertRun() { Run = !Run; }
    public void InvertCrouch() { Crouch = !Crouch; }
    public bool Run { get { return anim.GetBool(RunID); }  set { anim.SetBool(RunID, value); } }
    public bool Crouch { get { return anim.GetBool(CrouchID); } set { anim.SetBool(CrouchID, value); } }
}
