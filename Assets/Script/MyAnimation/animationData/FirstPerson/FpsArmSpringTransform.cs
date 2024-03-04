using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsArmSpringTransform : MonoBehaviour
{
    public Vector3 Up = new Vector3(0,-1.1f,-0.15f);
    public Vector3 Center = new Vector3(0, -1.5f, 0);
    public Vector3 Down = new Vector3(0,-1.1f, 0.35f);

    public void TransformRotate(float ratio)
    {
        if (ratio <= 0.5)
        { transform.localPosition = Vector3.Lerp(Down,Center, ratio*2f); }
        else
        { transform.localPosition = Vector3.Lerp(Center, Up, ratio-0.5f); }
        
    }

}
