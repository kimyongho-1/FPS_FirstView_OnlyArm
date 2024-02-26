using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class FootIKData
{
    public HumanBodyBones bodyPart;
    public bool isGround, drawGizmo;
    public string propertyName, tag;
    public Vector3 pos;
    public Quaternion rot;
    [HideInInspector] public Vector3 hitNormal;
    [HideInInspector] public Vector3 hitPoint;
    [HideInInspector] public float hitDist;
    [HideInInspector] public Collider hitCollider;
    public Transform rayOrigin;

    public void OnDrawGizmos(float rayPosOffset, float rayDistance)
    {
        Gizmos.color = Color.yellow;
        Vector3 start = rayOrigin.position + (Vector3.up * rayPosOffset);
        Vector3 end = start + (Vector3.down * (rayDistance + rayPosOffset));
        Gizmos.DrawLine(start, end);
    }
}
