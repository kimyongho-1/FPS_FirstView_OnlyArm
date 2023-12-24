using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabrikTest : MonoBehaviour
{
    [SerializeField]
    Transform[] bones;
    float[] bonesLengths;
    [SerializeField]
    int solverIterations = 5; //  FABRIK�� ��� �����Ұ�����

    [SerializeField]
    Transform targetPosition;

    private void Start()
    {
        bonesLengths = new float[bones.Length];

        //�� ���� ���̸� ���
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
        // ���� ���� ��ġ�� ����
        for (int i = 0; i < bones.Length; i++)
        {
            finalBonePositions[i] = bones[i].position;
        }

        // iterations����ŭ FABRIK����
        for (int i = 0; i < solverIterations; i++)
        {
            finalBonePositions = SolveForwardPositions(
                SolveInversePositions(finalBonePositions));
        }

        // ����� �� ���� ����
        for (int i = 0; i < bones.Length; i++)
        {
            bones[i].position = finalBonePositions[i];
            if (i != bones.Length - 1)
            {
                bones[i].rotation =
                    Quaternion.LookRotation(finalBonePositions[i + 1] - bones[i].position);
            }
            else // ������ �ε�����
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
            // Ÿ�ٰ� ���尡��� �ܰ����� Ÿ������ ��ġȭ
            if (i == forwardPositions.Length - 1)
            {
                inversePositions[i] = targetPosition.position;
            }
            else
            {
                // ���� �ݺ��� ���������̱⿡, i+1 ����(���� �ε���) ��ġ��
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
