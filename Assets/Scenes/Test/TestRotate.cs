using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public Transform spine, gun, targetTrans;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightangle, 0.1f);

        Gizmos.color = Color.green;
        // BC 선분 : C점으로 향하는
        Gizmos.DrawLine(spine.position, targetTrans.position);
        // BA 선분 : A로 향하는
        Gizmos.DrawLine(spine.position, rightangle);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(desiredRelTargetPos, 0.1f);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(desiredRelTargetPos, spine.position);


    }
    Vector3 rightangle, desiredRelTargetPos; Vector3 spineToTargetDir, spineToRelTargetDir;
    public float forwardL;
    void Update()
    {
        Vector3 rightAnglePoint = Vector3.Project(spine.position - gun.position, gun.forward * forwardL); //Get point to create 90 degree angle for right angle
      
        rightAnglePoint = gun.position + rightAnglePoint; //transform point to world space
        rightangle = rightAnglePoint;
        float sideC = Vector3.Distance(spine.position, targetTrans.position); //Get hypotenuse

        float sideA = Vector3.Distance(spine.position, rightAnglePoint); //Get sideA
        
        float sideB = Mathf.Sqrt((sideC * sideC) - (sideA * sideA)); //Get sideB. (C squared - A squared = B squared)

        desiredRelTargetPos = rightAnglePoint + (gun.forward * sideB); //relative target point (if target were to rotate around spine to align with gun's forward direction)
       
        spineToTargetDir = targetTrans.position - spine.position; //spine to target position direction
        spineToRelTargetDir = desiredRelTargetPos - spine.position; //spine to desired target position relative to setup
        
        Vector3 rotAxis = Vector3.Cross(spineToTargetDir, spineToRelTargetDir); //get rotation axis

        float rotAngle = Mathf.Sqrt(Vector3.Dot(spineToTargetDir, spineToTargetDir) * Vector3.Dot(spineToRelTargetDir, spineToRelTargetDir)) 
            + Vector3.Dot(spineToTargetDir, spineToRelTargetDir); //Get rotation angle

        Quaternion inverseRot = new Quaternion(rotAxis.x, rotAxis.y, rotAxis.z, rotAngle).normalized; //Construct new Quaternion

        spine.rotation = Quaternion.Inverse(inverseRot) * spine.rotation; //Apply rotation
    }
}
