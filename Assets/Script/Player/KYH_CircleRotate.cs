using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KYH_CircleRotate : MonoBehaviour
{
    public KYH_Mover mover;
    public AnimationCurve addY,addZ;
    public Vector2 yClamp, zClamp;
    void Update()
    {
        float xRatio = (mover.pitch + 80f) / 160f;

        transform.localPosition =
            new Vector3(0, addY.Evaluate(xRatio), addZ.Evaluate(xRatio));
    }
}
