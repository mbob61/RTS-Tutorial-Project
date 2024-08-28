using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskTrySetDestinationOrTarget : Node
{
    CharacterManager _manager;

    private Ray ray;
    private RaycastHit raycastHit;

    public TaskTrySetDestinationOrTarget(CharacterManager manager) : base()
    {
        _manager = manager;
    }

    public override NodeState Evaluate()
    {
        if (_manager.IsSelected && Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, Globals.UNIT_LAYER))
            {
                UnitManager um = raycastHit.collider.GetComponent<UnitManager>();
                if (um != null)
                {
                    Parent.Parent.SetData("currentTarget", raycastHit.transform);
                    ClearData("destinationPoint");
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            else if (Physics.Raycast(ray,out raycastHit, 1000f, Globals.TERRAIN_LAYER))
            {
                ClearData("currentTarget");
                Parent.Parent.SetData("destinationPoint", raycastHit.point);
                state = NodeState.SUCCESS;
                return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }
}