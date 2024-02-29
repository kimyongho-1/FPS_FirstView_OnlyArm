using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : MyBaseController
{
    SpineRotate0224 SR; 
    public MyCapsuleData CapsuleData;

    public override void Awake()
    {
        base.Awake();
        SR = transform.GetComponentInChildren<SpineRotate0224>();
        SR.Init(this, myInput);
        StateMachine = new SMB_Character(this);
    }

    private void Update()
    {
        StateMachine.CurrState.Update();

        // �Է�ó���� ���� �� �ִϸ��̼� ������Ʈ
        SR.ModelUpdate();
    }
    private void FixedUpdate()
    {
        StateMachine.CurrState.FixedUpdate();
    }


}