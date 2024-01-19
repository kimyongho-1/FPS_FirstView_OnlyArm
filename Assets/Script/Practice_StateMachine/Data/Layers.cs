using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Layers
{
    [field: SerializeField] public LayerMask GroundLayer { get; private set; }

}
