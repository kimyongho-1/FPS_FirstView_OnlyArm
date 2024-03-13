using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum process
{ None, Start, Update, LateUpdate }

public class WeaponSway : MonoBehaviour
{
    public process curr;
    public AnimationCurve ProcessRatio;
    public AnimationCurve xPosOffset;

    public AnimationCurve rotZAmount;

    public float AmountToSway;
    public MyPlayerController pc;
    float hor;
    float ver;
    private void Update()
    {
        Vector2 mouseForce = pc.myInput.mouseDir;
        hor = mouseForce.x;
        ver = mouseForce.y;

        if (Mathf.Abs(hor) < AmountToSway && Mathf.Abs(ver) < AmountToSway)
        {
            switch (curr)
            {
                case process.None: return;
                case process.Start:
                case process.LateUpdate:
                case process.Update: curr = process.LateUpdate; DoLateUpdate(); return;
            }
        }
        else 
        {
            switch (curr)
            {
                case process.None: curr = process.Start; DoStart();  return;
                case process.Start: DoStart();return;
                case process.LateUpdate: DoLateUpdate(); return;
                case process.Update: DoUpdate(); return;
            }
        }

        
    }

    float tick;
    public float power;
    void DoStart()
    {
        if (tick > 5) { curr = process.Update; DoUpdate(); return; }

        tick += 1;

        transform.localRotation = Quaternion.Slerp
           (transform.localRotation
           , Quaternion.AngleAxis(rotZAmount.Evaluate(hor), Vector3.forward)
           , power * 3f );
    }
    void DoUpdate()
    {
        tick += 1;

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(xPosOffset.Evaluate(hor),0,0), power);

        transform.localRotation = Quaternion.Slerp
       (transform.localRotation
       , Quaternion.AngleAxis(rotZAmount.Evaluate(hor), Vector3.forward)
       , power );

    }

    void DoLateUpdate()
    {
        tick += 1;
        float lerpSpeed = ProcessRatio.Evaluate(tick / 60f);

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(xPosOffset.Evaluate(hor) * lerpSpeed, 0, 0), power*2f);

        transform.localRotation = Quaternion.Slerp
           (transform.localRotation
           , Quaternion.AngleAxis(rotZAmount.Evaluate(hor) * lerpSpeed, Vector3.forward)
           , power*2f );

        if (tick > 60)
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
               curr = process.None;
            tick = 0;
        }
    }

}
