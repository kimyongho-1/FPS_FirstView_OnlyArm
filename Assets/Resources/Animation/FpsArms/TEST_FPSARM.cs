using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_FPSARM : MonoBehaviour
{
    public Transform refL, refR;
    public Transform L, R;

    private void LateUpdate()
    {
        L.position = refL.position;
        R.position = refR.position;
    }
}
