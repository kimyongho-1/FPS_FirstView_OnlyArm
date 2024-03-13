using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    public float swayAmountA = 1;
    public float swayAmountB = 2;
    public float swayScale = 600f; // »£»Ì¿« ≈©±‚
    public float swayLerpSpeed = 14;
    public float swayTime;
    public Vector3 swayPosition;

    private void Update()
    {
        CalculateWeaponSway();
    }
    public void CalculateWeaponSway()
    {
        swayTime += Time.deltaTime;
        var targetPosition = LissajousCurve(swayTime, swayAmountA, swayAmountB) / swayScale;

        swayPosition = Vector3.Lerp(swayPosition, targetPosition, Time.smoothDeltaTime * swayLerpSpeed);

        transform.localPosition = swayPosition;
    }
    public Vector3 LissajousCurve(float time, float a, float b)
    {
        return new Vector3(Mathf.Sin(time), a * Mathf.Sin(b * time + Mathf.PI));
    }
}
