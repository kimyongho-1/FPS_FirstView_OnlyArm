using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyCapsuleData 
{
    public CapsuleCollider collider;

    public void Initialize(MyBaseController controller)
    {
        if (collider == null) { collider = controller.GetComponent<CapsuleCollider>(); }


    }

}