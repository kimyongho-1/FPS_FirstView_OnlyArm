using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MyCapsuleData 
{
    public CapsuleCollider collider;
    public LayerMask groundMask;
    public AnimationCurve slopeCurve;
    
    [Range(0.6f, 1.6f)] public float colHeight;
    public float rayLength = 0.01f;

    public void Initialize(MyBaseController controller)
    {
        if (collider == null) { collider = controller.GetComponent<CapsuleCollider>(); }


    }
    public void ColliderSetting()
    {
        float MinColHeight = 0.6f;
        float MaxColHeight = 1.6f;
        float normalizedValue = (colHeight - MinColHeight) / (MaxColHeight - MinColHeight);

        collider.center = new Vector3(0, Mathf.Lerp(1.5f, 1f, normalizedValue), 0);
        
        collider.height = colHeight;
    }

}