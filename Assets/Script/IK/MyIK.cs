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
        // 목적지의 원본 회전값 초기화
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

        // 곧바로 각 뼈관절을 위치시키는게 아닌, 역순,순반향으로 it만큼 반복하여 
        // 정확한 위치에 놓을것이기에, 계속 바뀌는 위치값변수를 사용
        // 먼저 현재 뼈관절의 위치값으로 초기화
        for (int i = 0; i < bones.Length; i++)
        {
            pos[i] = bones[i].position;
        }

        // 만약 타겟까지의 거리가 모든 하체의 뼈길이합보다 크다면, 타겟방향으로 향하도록 팽팽하게 만들기
        if ((target.position - bones[0].position).sqrMagnitude > totalBonesLength * totalBonesLength)
        {
            for (int i = 1; i < bones.Length; i++)
            {
                Vector3 dir = (target.position - bones[i].position).normalized; // 타겟으로 향하는 방향벡터 구하기
                pos[i] = dir * boneLength[i - 1] + pos[i - 1]; // 타겟방향으로 뼈길이만큼 곱한후, 자신의 부모위치벡터에서 시작하도록
            }
        }

        // 타겟이 하체의 총 길이내에 존재시, FABRIK
        else
        {
            for (int it = 0; it < iterations; it++)
            {

                // Backwards 자식에서 부모순 : 타겟의 위치에 맞추기위해 (단 최상위 부모 골반은 위치 변경필요 X)
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

                // Forward 부모에서 자신순서로 : 
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
