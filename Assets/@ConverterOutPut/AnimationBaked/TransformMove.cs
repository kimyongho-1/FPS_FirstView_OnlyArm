using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformMove : MonoBehaviour
{
    public Transform target,local;
    public Vector3 offset;
    private void Update()
    {
        local.position = target.position + offset;

    }
    private void LateUpdate()
    {
    }
}
