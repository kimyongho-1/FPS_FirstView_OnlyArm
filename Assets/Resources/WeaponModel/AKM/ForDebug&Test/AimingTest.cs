using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingTest : MonoBehaviour
{
    public Camera CAM;

    public void Update()
    {
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(CAM.transform.position, CAM.transform.forward * 10f);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, transform.forward * 10f);
    }
}
