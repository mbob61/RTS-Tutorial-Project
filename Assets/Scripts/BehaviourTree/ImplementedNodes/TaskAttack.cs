using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskAttack : Node
{
    UnitManager manager;

    public TaskAttack(UnitManager manager): base()
    {
        this.manager = manager;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = GetData("currentTarget");
        manager.Attack((Transform)currentTarget);
        state = NodeState.SUCCESS;
        return state;
    }
}
