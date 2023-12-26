using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    #region Variable

    /// <summary>
    /// 각 뼈대 : Foot일 경우 발|무릎|골반(하체기준 최고부모 LeafBone)
    /// </summary>
    public Transform[] bones; 
    /// <summary>
    /// 렌더링직전, foot이 위치해야할 월드상 위치배열
    /// </summary>
    public Vector3[] positions;
    Vector3[] startDir;
    Quaternion[] startRotationBone;
    Quaternion startRotationTarget;
    Quaternion startRotationRoot;
    /// <summary>
    /// 각 뼈끼리의 간격 길이 (무릎과 발사이의 길이는 변하지않아야한다)
    /// </summary>
    public float[] bonesLength;

    /// <summary>
    /// Fabrik 알고리즘에서, 역순+부모순으로 위치값 계산을 반복할 횟수 (증가시 정확도 증가)
    /// </summary>
    public int iterations;
    /// <summary>
    /// 발이 위치할 목표방향
    /// </summary>
    public Transform Target;
    public Transform Pole;
    float totalBonesLength = 0;// 각 뼈사이의 총 길이합 (골바에서 발까지의 총 신체길이)

    #endregion

    private void Awake()
    {
        bones = new Transform[3];  // 발->무릎->골반 총 3개
        positions = new Vector3[3]; // 각 뼈가 새로위치할 다음 위치값이기에, 뼈의 수와 동일하게
        bonesLength = new float[2]; // 발~무릎 + 무릎~골반  총 2개의 뼈길이
        startDir = new Vector3[3];
        startRotationBone = new Quaternion[3];
        startRotationTarget = Target.rotation;
        // 스크립트를 사용한 발TRANSFORM에서부터 골반 찾기
       Transform bone = transform;
       startDir[2] = Target.position - bone.position;
       for (int i = bones.Length - 1; i >= 0; i--)
       {
            bones[i] = bone;
            startRotationBone[i] = bone.rotation;
            if (i != bones.Length - 1)
            {
                startDir[i] = bones[i + 1].position - bones[i].position;
                bonesLength[i] = (bones[i + 1].position - bones[i].position).magnitude;
                totalBonesLength += bonesLength[i];
            }
            
            bone = bone.parent;
        }

    }
    
    /// <summary>
    /// Update에서 뼈의 회전+위치등을 적용시
    /// 애니메이션 상태가 업데이트 될떄마다, 
    /// 본에 적용한 위치회전등이 실행 애니메이션에 의해 덮어씌여 오버라이드 되는 문제 발생
    /// 따라서 LateUpdate에 작성 및 실행시 정상작동 확인
    /// Update-OnAnimatorIK-LateUpdate 순서
    /// </summary>
    private void LateUpdate()
    {
        // to do : weight가 0이어도 예외
        if (Target == null) { return; }

        // 뼈를 곧바로 이동이 아닌, 보정하는 방식으로 계산
        for (int i = 0; i < positions.Length; i++)
        { positions[i] = bones[i].position; }

        var RootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var RootRotDiff = RootRot * Quaternion.Inverse(startRotationRoot);
        // 골반에서 최종 위치까지의 거리가 하체의 총길이보다 크다면
        // 타겟 방향으로 뻗어나가게만 위치 설정후 끝내기
        if ((Target.position - bones[0].position).sqrMagnitude >= totalBonesLength * totalBonesLength)
        {
            Vector3 dir = (Target.position - positions[0]).normalized;
            // 최고부모 하체골반은 움직일 필요 X
            for (int i = 1; i < bones.Length; i++)
            {
                #region 설명
                // 자신의 부모위치에서, 타겟방향으로 이동시 위치값으로 설정
                // EX : 무릎의 경우, 골반에서 타겟방향으로의 방향벡터를 구하기
                // 방향벡터 * (골반에서 무릎까지의 본 길이) 를 한 위치벡터 구하기
                // 골반에서 위의 방향벡터를 더한 벡터가 무릎이 위치해야할 위치벡터
                #endregion
                positions[i] = dir * bonesLength[i - 1] + positions[i - 1];
            }

        }

        // 타겟이 하체의길이 범위내 있을시 ( ik 특유의 구부러진 상태 고려 + 정확한 위치계산 필요)
        else
        {
            #region 한번이 아닌 iterations만큼 위치계산을 반복하는 이유
            // FARBIK 알고리즘의 경우,
            // Backward -> Forward 순서로 위치계산을 하는데
            // 역순(발에서 골반으로)계산을 통해 각 뼈대를 목표(Target)에 가깝게 이동
            // 순방향(골반에서 발)계산을 통해 관절을 조정하여, 육안상 정확하게 계산

            // 여기서 iterations가 증가할수록 목표위치에 정확해진다
            // 허나 반복문을 과하게 수행할 필요가 없는 경우 (이미 정확한 위치에 있는지)
            // 반복문 끝에 예외처리를 하여 현재 반복문을 빠져나가도록 설정
            #endregion
            // iterations만큼 뼈가 위치할 Vec3를 반복 계산
            for (int it = 0; it < iterations; it++)
            {
                // Back역순계산  발 -> 관절 (관절뼈대 위치 계산 피하기)
                for (int i = bones.Length-1; i > 0 ; i--)
                {
                    // 발의 경우, 타겟이 하체범위내이기에, 타겟 위치로 고정하고 끝내기
                    if (i == bones.Length - 1)
                    {
                        positions[i] = Target.position;
                        continue;
                    }

                    // 발에서 무릎으로 향하는 역방향으로 우선 위치시키기
                    positions[i] = positions[i+1] +
                        (positions[i] - positions[i+1]).normalized * bonesLength[i];
                }

                // 순방향 계산, 역순방향에서 못한 신체구조상의 위치계산 (0번쨰 인덱스 뼈골반은 움직여선 안되기에 생략)
                for (int i = 1; i < bones.Length; i++)
                {
                    // 자신의 부모위치에서, 부모에서 나로 향하는 방향벡터 * 뼈 길이
                    // 를 취한 계산이 현재 뼈가 위치해야할 위치벡터
                    positions[i] = positions[i-1] +
                        (positions[i]- positions[i - 1]).normalized * bonesLength[i-1];
                }

                // iterations만큼 계산하지만, 이미 위치가 정확할시
                // 마저 계산 안하고 반복문 빠져나가기
                if ((bones[bones.Length - 1].position - Target.position).sqrMagnitude
                    <= Mathf.Epsilon)
                {
                    break;
                }
            }

        }

        if (Pole != null)
        {
            for (int i = 1; i < bones.Length-1; i++)
            {
                // 발에서 골반으로 향하는 방향을 노멀벡터로하는 투영평면 생성 + 골반의 위치에 투영평면이 통과하도록
                var plane = new Plane((positions[i + 1] - positions[i - 1]), positions[i-1]);

                // 현재 Pole위치에서 투영평면으로 수직을 이루면서 가장 가까운 위치구하기
                var projectedPole = plane.ClosestPointOnPlane(Pole.position);
                // 무릎위치에서 투영평면으로 수직을 이루면서 가장 가까운 위치구하기
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);

                // 골반에서 시작하여 투영평면에서 위치한 각 점들사이의 각도 구하기
                var angle = Vector3.SignedAngle(projectedBone - positions[i-1]
                    , projectedPole - positions[i-1], plane.normal);

                // 위치값에 회전을 곱하여 변환된 위치값으로 초기화
                positions[i] = Quaternion.AngleAxis(angle, plane.normal)
                    * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
       
        }


        for (int i = 0; i < positions.Length; i++)
        {
            //발
            if (i == positions.Length - 1)
            {
                bones[i].rotation = Target.rotation
                    * Quaternion.Inverse(startRotationTarget)
                    * startRotationBone[i];
            }
            else
            {
                bones[i].rotation =
                    Quaternion.FromToRotation(startDir[i], positions[i + 1]
                    - positions[i]) * startRotationBone[i];
            }
            bones[i].position = positions[i];
        }

    }

    private void OnDrawGizmos()
    {
        var current = this.transform;
        for (int i = 0; i < bones.Length-1 && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            UnityEditor.Handles.matrix = Matrix4x4.TRS(current.position,
                Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position)
                , new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
