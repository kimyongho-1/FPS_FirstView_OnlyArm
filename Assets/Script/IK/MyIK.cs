using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyIK : MonoBehaviour
{
    public Transform[] bones;
    float[] boneLength;
    float totalBonesLength;
    Vector3[] pos;
    Quaternion[] originRotations;
    Vector3[] originDir;

    public Transform target;
    Quaternion targetOriginRotation;
    public Transform pole;
    public int iterations=5;

    private void Awake()
    {
        bones = new Transform[3];
        boneLength = new float[bones.Length-1];
        pos = new Vector3[bones.Length];
        originRotations = new Quaternion[bones.Length];
        originDir = new Vector3[bones.Length];
        // �������� ���� ȸ���� �ʱ�ȭ
        if (target != null)
        { targetOriginRotation = target.rotation; }

        Transform currBone = transform;
        for (int i = bones.Length-1; i >= 0 ; i--)
        {
            bones[i] = currBone;
            pos[i] = currBone.position;
            originRotations[i] = currBone.rotation;
            currBone = currBone.parent;
            if (i != bones.Length - 1)
            {
                boneLength[i] = (bones[i+1].position - bones[i].position).magnitude;
                totalBonesLength += boneLength[i];

                originDir[i] = (bones[i+1].position -  bones[i].position);
            }

        }
        originDir[2] = (target.position - bones[2].position).normalized;
    }

    private void LateUpdate()
    {
        if (target == null) { return; }

        // ��ٷ� �� �������� ��ġ��Ű�°� �ƴ�, ����,���������� it��ŭ �ݺ��Ͽ� 
        // ��Ȯ�� ��ġ�� �������̱⿡, ��� �ٲ�� ��ġ�������� ���
        // ���� ���� �������� ��ġ������ �ʱ�ȭ
        for (int i = 0; i < bones.Length; i++)
        {
            pos[i] = bones[i].position;
        }

        // ���� Ÿ�ٱ����� �Ÿ��� ��� ��ü�� �������պ��� ũ�ٸ�, Ÿ�ٹ������� ���ϵ��� �����ϰ� �����
        if ((target.position - bones[0].position).sqrMagnitude > totalBonesLength * totalBonesLength)
        {
            for (int i = 1; i < bones.Length; i++)
            {
                Vector3 dir = (target.position - bones[i].position).normalized; // Ÿ������ ���ϴ� ���⺤�� ���ϱ�
                pos[i] = dir * boneLength[i - 1] + pos[i - 1]; // Ÿ�ٹ������� �����̸�ŭ ������, �ڽ��� �θ���ġ���Ϳ��� �����ϵ���
            }
        }

        // Ÿ���� ��ü�� �� ���̳��� �����, FABRIK
        else
        {
            for (int it = 0; it < iterations; it++)
            {

                // Backwards �ڽĿ��� �θ�� : Ÿ���� ��ġ�� ���߱����� (�� �ֻ��� �θ� ����� ��ġ �����ʿ� X)
                for (int i = bones.Length-1; i > 0; i--)
                {
                    if (i == bones.Length - 1)
                    {
                        pos[i] = target.position;
                    }
                    else
                    {
                        pos[i] = pos[i+1] + (pos[i] - pos[i+1]).normalized * boneLength[i] ;

                    }
                }

                // Forward �θ𿡼� �ڽż����� : 
                for (int i = 1; i < bones.Length; i++)
                {
                    pos[i] = (pos[i] - pos[i-1]).normalized * boneLength[i] + pos[i-1];
                }

                if ((target.position - bones[bones.Length - 1].position).sqrMagnitude <= Mathf.Epsilon)
                { break; }
            }
        }

        if (pole != null)
        {
            Plane p = new Plane((pos[0]-pos[2]) , pos[0]);
            Vector3 projectedPole = p.ClosestPointOnPlane(pole.position);
            Vector3 projectedBone = p.ClosestPointOnPlane(pos[1]);
            float angle = Vector3.SignedAngle(projectedBone, projectedPole, p.normal);
            pos[1] = Quaternion.AngleAxis(angle, p.normal) * 
               (pos[1]-pos[0]) +  pos[0];
        }

        for (int i = 0; i < bones.Length; i++)
        {
            if (i == 2)
            {
                bones[i].rotation = target.rotation * Quaternion.Inverse(targetOriginRotation) * originRotations[2];
            }
            else 
            {
                bones[i].rotation = Quaternion.FromToRotation(originDir[i], (pos[i+1]-pos[i]) ) * originRotations[i]  ;
            }

            bones[i].position = pos[i];
        }
    }
}
