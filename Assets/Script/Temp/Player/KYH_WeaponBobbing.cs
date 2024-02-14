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
        #region 무기 Bob을 따로 함수로 뺴둔 이유
        // 무기의 애니메이션 진행후
        // 상태에 따라 애니메이션 클립의 프로퍼티값 * 상태값으로 최종 무기 위치계산
        // 허나 애니메이션 재생후 LateUpdate에 실행시 손(IK)이 무기를 따라오지못해서
        // 하이어아키 @FPSarm의 핸드ik가 실행되는 OnAniamtorIK()함수 내부에서 먼저 실행
        // 그후 핸드IK실행하여 싱크 맞추기 완료
        #endregion
        // TO DO : 상태변화에 따라 TEST값 변화로 다친상태 | iDLE상태등 표현
        // 캐릭터의 숨차는 상태 Ratio => anim.speed로
        // 캐릭터의 다쳤는지 상태에 따라
        // 추후 enum PlayerState { DEAD, NORMAL , INJUERED ...}로 표현하기
        //  multiply
        // Idle 상태일때 0
        transform.localPosition *= anim.GetFloat("Amplify");
    }
   
}
