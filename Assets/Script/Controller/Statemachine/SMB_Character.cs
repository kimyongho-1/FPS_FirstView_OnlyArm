using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMB_Character : SMB_Base
{
    public SMB_Character(MyBaseController owner)
    {
        StandIdleState = new StandIdleState(owner);
        StandWalkState = new StandWalkState(owner);
        StandRunState =  new StandRunState (owner);    
        
        ChangeState(StandIdleState);
    }

}
