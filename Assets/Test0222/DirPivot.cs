using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirPivot : MonoBehaviour
{
    public Transform Hight, Low;

    public Vector3 GetHeight(float pitchRatio)
    {
        if (pitchRatio == 0) { return this.transform.position; }
        // 0 ~ 1의 비율로 t값 넣기
        return Vector3.Lerp(Low.position ,Hight.position , (pitchRatio ));
    }
}
