using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FARBIK : MonoBehaviour
{
    public Transform Target;
    public Transform[] Bones;
    public Vector3[] Positions;
    public float[] BonesLength;
    int ChainLen = 3; // ���������� ���� ���� (4���� ���̶�� 3���� ���ἱ)
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
            current = current.parent; // �Ƹ���� ������, �θ����� �ʱ�ȭ
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
        // Bones[0] �ְ� Leaf������Ʈ
        // sqrMag ��������� ��Ʈ ��� ���ϱ����� (�߰��� totalLen�� �����ϱ�)
        // �� ���������� ���̸� ���Ͽ� ������ totalLen�� init�Լ����� ���� ����
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
                // ���� ���� �����κк��� ����
                for (int i = Positions.Length-1; i >0; i--)
                {
                    // ���ϴ� ���� ���̶��
                    if (i == Positions.Length - 1)
                    {
                        // Ÿ����, �� ���� ���̺��� ª�� �����̰�
                        // ���� ���� ���� ���̶�� Ÿ����ġ�� ���� ����
                        Positions[i] = Target.position;
                    }
                    else 
                    {
                        // ���� ������ �ƴ� �̿� ��Ʈ��
                        // �ڽ��� �ڽĿ��� �ڽ����� ���ϴ� ���⺤�� ���ϱ� �ڽ��� �� ����
                        // �� �� ��ġ�� �Ҵ�
                        Positions[i] = (Positions[i] - Positions[i + 1]).normalized * BonesLength[i];
                    }
                }
                #endregion

                #region Forward
                for (int i = 1; i < Positions.Length; i++)
                {
                    // �ڽ��� �θ𿡼� �ڽ����� ���ϴ� ���⺤�ͱ��ϱ�
                    Positions[i] = Positions[i - 1] +
                        (Positions[i] - Positions[i - 1]).normalized * BonesLength[i-1];
                 
                }
                #endregion
                // ���� �˼��κ��� Ÿ���̶� �ʹ� ������ ���� ����
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
