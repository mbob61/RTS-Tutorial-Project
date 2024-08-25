using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskTrySetDestination : Node
{
    CharacterManager characterManager;
    private Ray ray;
    private RaycastHit raycastHit;

    public TaskTrySetDestination(CharacterManager manager) : base()
    {
        this.characterManager = manager;
    }

    public override NodeState Evaluate()
    {
        if (characterManager.IsSelected && Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, Globals.TERRAIN_LAYER))
            {
                Parent.Parent.SetData("destinationPoint", raycastHit.point);
                state = NodeState.SUCCESS;
                return state;
            }

        }
        state = NodeState.FAILURE;
        return state;
    }
}
