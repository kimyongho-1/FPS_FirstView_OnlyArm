using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CapsuleColliderUtility 
{ 
    public CapsuleColliderData CapsuleColliderData { get; private set; }
    [field : SerializeField]public DefaultColliderData DefaultColliderData { get; private set; }
    [field: SerializeField] public SlopeData SlopeData { get; private set; }

    public void Initialize(GameObject go)
    {
        if (CapsuleColliderData != null)
        { return; }
        CapsuleColliderData = new CapsuleColliderData();

        CapsuleColliderData.Initialize(go);
    }
    public void CalculateCapsuleColliderDimension()
    {
        SetCapsuleColliderRadius(DefaultColliderData.Radius);
        SetCapsuleColliderHeight(DefaultColliderData.Height * (1f-SlopeData.StepHeightRatio));
        ReCalculateCenter();
        if (CapsuleColliderData.Collider.height / 2f < CapsuleColliderData.Collider.radius)
        {
            SetCapsuleColliderRadius(CapsuleColliderData.Collider.height / 2f);
        }

        CapsuleColliderData.UpdateColliderData();
    }

    private void ReCalculateCenter()
    {
        float colHeightDiff = DefaultColliderData.Height - CapsuleColliderData.Collider.height;
        Vector3 newColliderCenter = new Vector3(0f,DefaultColliderData.CenterY + (colHeightDiff /2f), 0f);
        CapsuleColliderData.Collider.center = newColliderCenter;

    }

    private void SetCapsuleColliderRadius(float radius)
    {
        CapsuleColliderData.Collider.radius = radius;   
    }
    private void SetCapsuleColliderHeight(float height)
    {
        CapsuleColliderData.Collider.height = height;
    }
}