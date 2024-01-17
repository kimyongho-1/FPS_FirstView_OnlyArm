using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lissajou : MonoBehaviour
{
    private float _currentTime;
    private Vector3 finalPosition;

    [Header("TRAIL VALUES")]
    public float speedMultiplier;
    [Header("EQUATION VALUES")]
    public float A;
    public float B;
    public float メ;
    public float モ;
    public float delta;
    public TrailRenderer TrailRenderer { get; private set; }
    public void Start() => TrailRenderer = GetComponent<TrailRenderer>();
    public void Update() => CalculateStandardLissajousCurve();
    private void CalculateStandardLissajousCurve()
    {
        _currentTime = Time.time * speedMultiplier;

        finalPosition = transform.position;

        finalPosition.x = A * Mathf.Sin(メ * _currentTime + delta);
        finalPosition.y = B * Mathf.Sin(モ * _currentTime);

        transform.position = finalPosition;
    }
}
//      Idle  
// Walk Front 2 2 3 2 4 1
// walk Left  1 3 2 4 4 1
//      Right 1 3 2 -4 4 1
