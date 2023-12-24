using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FARBIK : MonoBehaviour
{
    public Transform Target;
    public Transform[] Bones;
    public Vector3[] Positions;
    public float[] BonesLength;
    int ChainLen = 3; // 관절사이의 간격 갯수 (4개의 본이라면 3개의 연결선)
    float totalLen;
    public int Iterations=5;
    void Init()
    {
        Bones = new Transform[ChainLen + 1];
        Positions = new Vector3[ChainLen+1];
        BonesLength = new float[ChainLen];
        Transform current = transform;
         totalLen=0;
        for (int i = Bones.Length-1; i >= 0; i--)
        {
            Bones[i] = current;
            if (i == Bones.Length - 1)
            { }
            else
            {
                BonesLength[i] = (Bones[i + 1].position - current.position).magnitude;
                totalLen += BonesLength[i];
            }
            current = current.parent; // 아마츄어 구조상, 부모관계로 초기화
        }
    }

    private void Update()
    {
        ResolveIK();
    }
    void ResolveIK()
    {
        if (Target == null) { return; }

        if (BonesLength.Length != ChainLen) { Init(); }

        // GetPosition
        for (int i = 0; i < Bones.Length; i++)
        {
            Positions[i] = Bones[i].position;
        }

        // Calculate
        // Bones[0] 최고 Leaf오브젝트
        // sqrMag 사용이유는 루트 계산 피하기위해 (추가로 totalLen을 제곱하기)
        // 각 관절마다의 길이를 구하여 총합한 totalLen을 init함수에서 구한 상태
        if ((Target.position - Bones[0].position).sqrMagnitude >= totalLen * totalLen)
        {
            Vector3 dir = (Target.position - Positions[0]).normalized;

            for (int i = 1; i < Positions.Length; i++)
            {
                Positions[i] = Positions[i - 1] + dir * BonesLength[i - 1];
            }
        }
        else 
        {
            for (int iteration = 0; iteration < Iterations; iteration++)
            {
                #region Back
                // 본의 가장 꼬리부분부터 시작
                for (int i = Positions.Length-1; i >0; i--)
                {
                    // 최하단 꼬리 본이라면
                    if (i == Positions.Length - 1)
                    {
                        // 타겟이, 총 본의 길이보다 짧은 상태이고
                        // 현재 가장 꼬리 본이라면 타겟위치로 강제 고정
                        Positions[i] = Target.position;
                    }
                    else 
                    {
                        // 제일 꼬리가 아닌 이외 파트는
                        // 자신의 자식에서 자신으로 향하는 방향벡터 곱하기 자신의 본 길이
                        // 한 새 위치값 할당
                        Positions[i] = (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }
                #endregion

                #region Forward
                for (int i = 1; i < Positions.Length; i++)
                {
                    // 자신의 부모에서 자신으로 향하는 방향벡터구하기
                    Positions[i] = Positions[i - 1] +
                        (Positions[i] - Positions[i - 1]).normalized * BonesLength[i-1];
                 
                }
                #endregion
                // 제일 촉수부분이 타겟이랑 너무 가까우면 강제 종료
                if ((Positions[Positions.Length - 1] - Target.position).sqrMagnitude
                    < 0.01f * 0.01f)
                { break; }
            }
        }

        // SetPosition
        for (int i = 0; i < Positions.Length; i++)
        {
            Bones[i].position = Positions[i];
        }
    }

    private void S()
    {
       
    }
 

}
