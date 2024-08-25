using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckUnitHasDestination : Node
{
    public override NodeState Evaluate()
    {
        object destinationPoint = GetData("destinationPoint");
        if (destinationPoint == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        state = NodeState.SUCCESS;
        return state;
    }
}
