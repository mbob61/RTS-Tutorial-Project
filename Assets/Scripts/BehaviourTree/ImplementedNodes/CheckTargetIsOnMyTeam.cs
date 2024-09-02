using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckTargetIsOnMyTeam : Node
{
    private int myPlayerId;

    public CheckTargetIsOnMyTeam(UnitManager manager) : base()
    {
        myPlayerId = GameManager.instance.gamePlayersParameters.myPlayerId;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        UnitManager um = ((Transform)currentTarget).GetComponent<UnitManager>();
        if (um == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        state = um.Unit.Owner == myPlayerId ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
