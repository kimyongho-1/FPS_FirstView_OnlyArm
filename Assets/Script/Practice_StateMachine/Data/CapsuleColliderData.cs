using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class CapsuleColliderData 
{
    [field :SerializeField]public CapsuleCollider Collider { get; private set; }
    public Vector3 ColliderCenterInLocalSpace { get; private set; }

    public void Initialize(GameObject go)
    {
        if (Collider != null)
        { return; }

        Collider = go.GetComponent<CapsuleCollider>();
        
        UpdateColliderData();
    }
    public void UpdateColliderData()
    {
        ColliderCenterInLocalSpace = Collider.center;
    }
}