using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditManager :MonoBehaviour
{
    public static EditManager instance;
    public static EditManager Instance 
    { 
        get 
        {
            return instance;
        } 
    }

    private void Awake()
    {
        instance = this;
    }

    public SpineRotate SR;
}
