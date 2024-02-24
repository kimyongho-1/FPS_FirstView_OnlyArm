using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirPivot : MonoBehaviour
{
    public Transform Hight, Low;

    public Vector3 GetHeight(float pitchRatio)
    {
        if (pitchRatio == 0) { return this.transform.position; }
        // 0 ~ 1�� ������ t�� �ֱ�
        return Vector3.Lerp(Low.position ,Hight.position , (pitchRatio ));
    }
}
