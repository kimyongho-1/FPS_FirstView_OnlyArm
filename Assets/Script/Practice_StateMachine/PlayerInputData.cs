using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInputData : MonoBehaviour
{
    public float slopeSpeed = 1f;
    public float moveSpeed = 5f;
    public float mouseSensiX = 5f;
    public float mouseSensiY = 5f;

    public Action WalkToggle;
    public Action MovementCanceled;
}
