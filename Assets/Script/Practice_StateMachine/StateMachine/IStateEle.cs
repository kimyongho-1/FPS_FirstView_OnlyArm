using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateEle 
{
    public void Enter();
    public void Exit();
    public void UserInput();
    public void Update();
    public void PhysicsUpdate();
}