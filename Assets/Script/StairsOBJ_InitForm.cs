using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsOBJ_InitForm : MonoBehaviour
{
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform t = transform.GetChild(i);
            t.localPosition = new Vector3(0, t.localScale.y* i, 0.25f * i);
        }
    }
}
