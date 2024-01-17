using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : OnGroundState
{
    public WalkState(PlayerControlStateMachine playerControlStateMachine) : base(playerControlStateMachine)
    {
    }

}