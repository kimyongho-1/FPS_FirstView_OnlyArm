using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Transform  Target;
    Vector3 targetDir;
    Vector3 normal;
    Vector3 projected;
    // Update is called once per frame
    void Update()
    {
        targetDir = Target.position - transform.position;
        normal = transform.forward * len;
        projected = Vector3.Project(targetDir , normal );
    }
    public float len;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position + projected, 0.2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position + normal, 0.3f);
    }
}
