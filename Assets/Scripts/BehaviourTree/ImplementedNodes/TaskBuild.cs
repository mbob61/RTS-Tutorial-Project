using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskBuild : Node
{
    int buildPower;

    public TaskBuild(UnitManager manager): base()
    {
        buildPower = ((CharacterData)manager.Unit.Data).buildPower;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = GetData("currentTarget");
        BuildingManager bm = ((Transform)currentTarget).GetComponent<BuildingManager>();
        bool finishedBuilding = bm.Build(buildPower);
        if (finishedBuilding)
        {
            ClearData("currentTarget");
            ClearData("currentTargetOffset");
        }


        state = NodeState.SUCCESS;
        return state;
    }
}
