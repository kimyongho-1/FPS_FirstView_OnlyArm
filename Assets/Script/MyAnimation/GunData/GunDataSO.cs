using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDataSO : ScriptableObject
{
    public Vector3 spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float fireRate = 0.1f;

    [Header("Obsolete 아래것들")]
    public List<eachFinger> LeftFingers = new List<eachFinger>();
    public List<eachFinger> RightFingers = new List<eachFinger>();

    Transform FindBoneRecursive(Transform current, string name)
    {
        if (current.name.Contains(name)) return current;
        foreach (Transform child in current)
        {
            Transform found = FindBoneRecursive(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
    public void ApplyLeftRotate(Animator anim)
    {
        for (int i = 0; i < 5; i++)
        {
            Transform tr = FindBoneRecursive(anim.transform, LeftFingers[i].part.ToString());
       
            tr.localRotation = LeftFingers[i].first;
            tr.GetChild(0).localRotation = LeftFingers[i].sec;
            tr.GetChild(0).GetChild(0).localRotation = LeftFingers[i].last;
        }
    }

    public void ApplyRightRotate(Animator anim)
    {
        for (int i = 0; i < 5; i++)
        {
            Transform tr = FindBoneRecursive(anim.transform,RightFingers[i].part.ToString());

            tr.localRotation = RightFingers[i].first;
            tr.GetChild(0).localRotation = RightFingers[i].sec;
            tr.GetChild(0).GetChild(0).localRotation = RightFingers[i].last;
        }
    }
}
