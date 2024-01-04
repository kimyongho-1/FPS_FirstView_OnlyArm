using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DebugUI : MonoBehaviour
{
    public PlayerMovement pm;
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        text.text = $"rb.velocity : {Mathf.Round(pm.rb.velocity.x)}, {Mathf.Round(pm.rb.velocity.y) } ,{Mathf.Round(pm.rb.velocity.z)}";
        text.text += $" ANGLE : { Math.Round(pm.Angle,2)}";
    }
}
