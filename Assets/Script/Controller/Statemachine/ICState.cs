using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICState
{
    public void Enter();
    public void Exit();
    public void FixedUpdate();
    public void Update();
    public void AnimatorUpdate();

}