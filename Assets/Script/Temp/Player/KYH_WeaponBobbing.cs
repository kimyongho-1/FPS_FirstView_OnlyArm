using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_WeaponBobbing : MonoBehaviour
{
    Animator anim;
    public AnimationCurve x,y,z;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    
    public void WeaponBobbing()
    {
        #region ���� Bob�� ���� �Լ��� ���� ����
        // ������ �ִϸ��̼� ������
        // ���¿� ���� �ִϸ��̼� Ŭ���� ������Ƽ�� * ���°����� ���� ���� ��ġ���
        // �㳪 �ִϸ��̼� ����� LateUpdate�� ����� ��(IK)�� ���⸦ ����������ؼ�
        // ���̾��Ű @FPSarm�� �ڵ�ik�� ����Ǵ� OnAniamtorIK()�Լ� ���ο��� ���� ����
        // ���� �ڵ�IK�����Ͽ� ��ũ ���߱� �Ϸ�
        #endregion
        // TO DO : ���º�ȭ�� ���� TEST�� ��ȭ�� ��ģ���� | iDLE���µ� ǥ��
        // ĳ������ ������ ���� Ratio => anim.speed��
        // ĳ������ ���ƴ��� ���¿� ����
        // ���� enum PlayerState { DEAD, NORMAL , INJUERED ...}�� ǥ���ϱ�
        //  multiply
        // Idle �����϶� 0
        transform.localPosition *= anim.GetFloat("Amplify");
    }
   
}
