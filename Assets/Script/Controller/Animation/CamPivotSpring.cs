using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPivotSpring : MonoBehaviour
{
    public WeaponSway weaponSway;
    public Vector3 Down = new Vector3(0, 0.5f, 0.5f);
    public Vector3 Up = new Vector3(0, 0.2f, 0);
    public void Rotate(float pitch)
    {
        Vector3 Center = new Vector3(0,0.1f,0);
        if (pitch > 0)
        {
            //Vector3 Down = new Vector3(0, 0.35f, 0.5f);
            float ratio = pitch / 80f;
            transform.localPosition = Vector3.Lerp(Center, Down, ratio);
        }
        else
        {
            //Vector3 Up = new Vector3(0, 0.2f, 0);
            float ratio = pitch / -80f;
            transform.localPosition = Vector3.Lerp(Center, Up, ratio);
        }

    }

}
