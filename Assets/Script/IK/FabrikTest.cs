using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabrikTest : MonoBehaviour
{
    [SerializeField]
    Transform[] bones;
    float[] bonesLengths;
    [SerializeField]
    int solverIterations = 5; //  FABRIK을 몇번 적용할것인지

    [SerializeField]
    Transform targetPosition;

    private void Start()
    {
        bonesLengths = new float[bones.Length];

        //각 뼈의 길이를 계산
        for (int i = 0; i < bones.Length; i++)
        {
            if (i < bones.Length - 1)
            {
                bonesLengths[i] = (bones[i + 1].position - bones[i].position).magnitude;
            }
            else 
            {
                bonesLengths[i] = 0;
            }
        }
    }
    private void Update()
    {
        SolveIK();
    }
    void SolveIK()
    {
        Vector3[] finalBonePositions = new Vector3[bones.Length];
        // 뼈의 현재 위치를 저장
        for (int i = 0; i < bones.Length; i++)
        {
            finalBonePositions[i] = bones[i].position;
        }

        // iterations수만큼 FABRIK적용
        for (int i = 0; i < solverIterations; i++)
        {
            finalBonePositions = SolveForwardPositions(
                SolveInversePositions(finalBonePositions));
        }

        // 결과를 각 뼈에 저장
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = finalBonePositions[i];
            if (i != bones.Length - 1)
            {
                bones[i].rotation =
                    Quaternion.LookRotation(finalBonePositions[i + 1] - bones[i].position);
            }
            else // 마지막 인덱스만
            {
                bones[i].rotation =
                    Quaternion.LookRotation(targetPosition.position - bones[i].position);
            }
        }
    }

    Vector3[] SolveInversePositions(Vector3[] forwardPositions)
    {
        Vector3[] inversePositions = new Vector3[forwardPositions.Length];
        for (int i = (forwardPositions.Length-1); i >= 0 ; i--)
        {
            // 타겟과 가장가까운 외각뼈는 타겟으로 위치화
            if (i == forwardPositions.Length - 1)
            {
                inversePositions[i] = targetPosition.position;
            }
            else
            {
                // 현재 반복문 역순진행이기에, i+1 다음(이전 인덱스) 위치값
                Vector3 posPrimaSiguiente = inversePositions[i+1];
                Vector3 posBaseActual = forwardPositions[i];
                Vector3 direction = (posBaseActual - posPrimaSiguiente).normalized;
                float longitud = bonesLengths[i];
                inversePositions[i] = posPrimaSiguiente + (direction * longitud);
            }
        }
        return inversePositions;
    }

    Vector3[] SolveForwardPositions(Vector3[] inversePositions)
    {
        Vector3[] forwardPositions = new Vector3[inversePositions.Length];
        for (int i = 0; i < inversePositions.Length; i++)
        {
            if (i == 0)
            {
                forwardPositions[i] = bones[0].position;
            }
            else
            {
                Vector3 posPrima = inversePositions[i];
                Vector3 posPrimaSegund = forwardPositions[i - 1];
                Vector3 direction = (posPrima - forwardPositions[i - 1]).normalized;
                float longitud = bonesLengths[i - 1];
                forwardPositions[i] = posPrimaSegund + (direction * longitud);
            }
        }
        
        return forwardPositions;
    }

}
