using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskMoveToDestination : Node
{
    CharacterManager manager;

    public TaskMoveToDestination(CharacterManager characterManager) : base()
    {
        this.manager = characterManager;
    }

    public override NodeState Evaluate()
    {
        object destinationPoint = GetData("destinationPoint");
        Vector3 destination = (Vector3)destinationPoint;

        // Check to see if the destination point was changed.
        // and we need to re-update the agent's destination
        if (Vector3.Distance(destination, manager.agent.destination) > 0.2f)
        {
            bool canMove = manager.MoveTo(destination);
            state = canMove ? NodeState.RUNNING : NodeState.FAILURE;
            return state;
        }

        // check to see if the agent has reached the destination
        float d = Vector3.Distance(manager.transform.position, manager.agent.destination);
        if (d <= manager.agent.stoppingDistance)
        {
            ClearData("destinationPoint");
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }
}
