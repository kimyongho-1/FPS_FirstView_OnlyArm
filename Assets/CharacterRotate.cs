using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRotate : MonoBehaviour
{
    [SerializeField] BoneContainer[] spine;
    public Transform Target;
    public bool spine0, spine1, spine2, head;
    private void OnDrawGizmos()
    {
        return;
        if (spine0 == true) { Gizmos.color = Color.red; Gizmos.DrawLine(spine[0].bone.transform.position, spine[0].bone.transform.position + spine[0].bone.transform.forward * 46f); }

        if (spine1 == true)
        { Gizmos.color = Color.green; Gizmos.DrawLine(spine[1].bone.transform.position, spine[1].bone.transform.position + spine[1].bone.transform.forward * 46f); }

        if (spine2 == true)
        { Gizmos.color = Color.blue; Gizmos.DrawLine(spine[2].bone.transform.position, spine[2].bone.transform.position+ spine[2].bone.transform.forward * 46f); }
        if (head == true)
        { Gizmos.color = Color.black; Gizmos.DrawLine(spine[3].bone.transform.position + spine[3].positionOffset
            , spine[3].bone.transform.position + spine[3].bone.transform.forward * 46f); }
    }

    private void LateUpdate()
    {
        // 애니메이션 재생중일떈 함수 실행 x
        // Aim();
    }
    public void Aim()
    {
        if (Target == null) { return; }
        // Spine부터
        Vector3 targetPos = Target.position;
        for (int i = 0; i < spine.Length; i++) 
        {
            Vector3 currTargetPosition = targetPos - spine[i].positionOffset;
            Quaternion rotation = Quaternion.LookRotation(currTargetPosition);

            spine[i].wsRotation = Quaternion.Lerp(spine[i].wsRotation, rotation , spine[i].weights) * Quaternion.Euler(spine[i].rotationOffset);
            //spine[i].wsRotation = rotation * Quaternion.Euler(spine[i].rotationOffset);
        }

        // Head는 spine의 자식으로, spine후로 진행
    }


}
