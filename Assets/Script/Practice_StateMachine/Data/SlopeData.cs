using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlopeData
{
    [field: SerializeField][field: Range(0, 1f)] public float StepHeightRatio { get; private set; } = 0.25f;
    [field: SerializeField][field: Range(0, 5f)] public float rayDist { get; private set; } = 5f;
    [field: SerializeField][field: Range(0, 50f)] public float stepReachForce { get; private set; } = 25f;
}