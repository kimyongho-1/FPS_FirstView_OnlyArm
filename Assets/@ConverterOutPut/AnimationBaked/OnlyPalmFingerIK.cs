using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyPalmFingerIK : MonoBehaviour
{
    public Animator anim;
    public PalmData LH, RH;

    public bool capture;
    public bool GenerateOnClicked;
    public akDataSO data;

    private void Awake()
    {
        return;
        data.ApplyLeftRotate(anim);
        data.ApplyRightRotate(anim);
    }
    private void OnValidate()
    {

        if (!GenerateOnClicked) { return; }
        if (LH == null || RH == null) { return; }
        LH.list.Clear();
        RH.list.Clear();
        for (int i = 0; i < 5; i++)
        {
            LH.list.Add(new eachFinger(LH.Palm.GetChild(i), i));
            RH.list.Add(new eachFinger(RH.Palm.GetChild(i), i));
        }
        GenerateOnClicked = false;
    }

    private void LateUpdate()
    {
        return;
        for (int i = 0; i < 5; i++)
        {
            LH.GripPose();
            RH.GripPose();
        }
    }
 
}
public enum PalmChild
{
    Thumb, Index, Middle, Ring, Pinky,
}
[System.Serializable]
public class eachFinger
{
    public eachFinger(Transform t, int i) { tr = t; part = (PalmChild)i; }
    public Transform tr;
    public PalmChild part;
    public Quaternion first, sec, last;
}
[System.Serializable]
public class PalmData
{
    public Transform Palm;
    public Animator anim;
    public bool isLeft;
    public List<eachFinger> list = new List<eachFinger>();


    public void Generate()
    {
        for (int i = 0; i < 5; i++)
        {
            list.Add(new eachFinger(Palm.GetChild(i), i ));
        }
    }
    public void SetGripPose()
    {

        for (int i = 0; i < list.Count; i++)
        {

            list[i].tr.localRotation = list[i].first;
            list[i].tr.GetChild(0).localRotation = list[i].sec;
            list[i].tr.GetChild(0).GetChild(0).localRotation = list[i].last;
        }
    }
    public void GripPose()
    {
        
        for (int i = 0; i < list.Count; i++)
        {

            list[i].tr.localRotation = list[i].first;
            list[i].tr.GetChild(0).localRotation = list[i].sec;
            list[i].tr.GetChild(0).GetChild(0).localRotation = list[i].last;
        }
    }
}