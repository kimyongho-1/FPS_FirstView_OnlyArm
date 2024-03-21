using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Model : MonoBehaviour
{

    public Animator anim { get; set; }
    public void SetFloat(string name, float val) { anim.SetFloat(name, val); }
    public void SetBool(string name, bool val) { anim.SetBool(name, val); }
    public float GetFloat(string name) { return anim.GetFloat(name); }
    public bool GetBool(string name) {return anim.GetBool(name); }
    public void PlayInFixedTime(string name, int t) { anim.PlayInFixedTime(name, t); }
    public abstract void Lerp(bool b) ;
    public void Play(string name, int layer) { anim.Play(name,layer); }
    public void CrossFadeInFixedTime(string name, float ratio, int layer) { anim.CrossFadeInFixedTime(name, ratio, layer); }
}
