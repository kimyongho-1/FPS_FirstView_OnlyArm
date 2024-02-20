using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class BoneContainer
{
    
    [Range(0f, 1f)] public float weights=1f;
    public Vector3 wsPosition { get { return bone.position; } }
    public Vector3 lsPosition { get { return bone.localPosition; } }
    public Quaternion wsRotation { get { return bone.rotation; } set { bone.rotation = value; } }
    public Quaternion lsRotation { get { return bone.localRotation; } set { bone.localRotation = value; } }

    public Transform bone;
    public Vector3 positionOffset, rotationOffset;

}