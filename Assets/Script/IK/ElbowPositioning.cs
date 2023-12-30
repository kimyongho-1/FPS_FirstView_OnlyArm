using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElbowPositioning : MonoBehaviour
{

    public MainPlayer playerInputs;
    public Vector3[] pos = new Vector3[2];
    public float ratio;
    public bool doRight = false;
    private void Update()
    {
        ratio = Mathf.Clamp01((playerInputs.y + 75f) / 150f);
        if (!doRight) { return; }
        transform.localPosition = Vector3.Lerp(pos[0], pos[1], ratio);

    }
}
