using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFootIK : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.5f, Vector3.down);
        if (Physics.SphereCast(ray, 0.05f, out RaycastHit hit, 0.50f))
        { transform.position = hit.point + Vector3.up * 0.05f; }
    }
}
