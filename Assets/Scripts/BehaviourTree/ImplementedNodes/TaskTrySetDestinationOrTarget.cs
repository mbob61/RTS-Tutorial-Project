using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class TaskTrySetDestinationOrTarget : Node
{
    CharacterManager manager;

    private Ray ray;
    private RaycastHit raycastHit;

    private const float samplingRange = 12f;
    private const float samplingRadius = 1.8f;

    public TaskTrySetDestinationOrTarget(CharacterManager manager) : base()
    {
        this.manager = manager;
    }

    public override NodeState Evaluate()
    {
        if (manager.IsSelected && Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //follow target
            if (Physics.Raycast(ray, out raycastHit, 1000f, Globals.UNIT_LAYER))
            {
                UnitManager um = raycastHit.collider.GetComponent<UnitManager>();
                if (um != null)
                {
                    if (manager.SelectIndex == 0)
                    {
                        List<Vector2> targetOffsets = ComputeFormationTargetOffsets();
                        EventManager.TriggerEvent("TargetFormationOffsets", targetOffsets);
                        //Parent.Parent.SetData("currentTarget", raycastHit.transform);
                        //ClearData("destinationPoint");
                    }
                    state = NodeState.SUCCESS;
                    return state;
                }
            }

            // go to ground hit point

            else if (Physics.Raycast(ray,out raycastHit, 1000f, Globals.TERRAIN_LAYER))
            {
                if (manager.SelectIndex == 0)
                {
                    List<Vector3> targetPositions = ComputeFormationTargetPositions(raycastHit.point);
                    EventManager.TriggerEvent("TargetFormationPositions", targetPositions);

                }
                //ClearData("currentTarget");
                //Parent.Parent.SetData("destinationPoint", raycastHit.point);
                state = NodeState.SUCCESS;
                return state;
            }
        }
        state = NodeState.FAILURE;
        return state;
    }

    private List<Vector2> ComputeFormationTargetOffsets()
    {
        int nSelectedUnits = Globals.CURRENTLY_SELECTED_UNITS.Count;
        List<Vector2> offsets = new List<Vector2>(nSelectedUnits);
        // leader unit goes to the exact target point
        offsets.Add(Vector2.zero);
        if (nSelectedUnits == 1) // (abort early if no other unit is selected)
            return offsets;

        // next units have offsets computed with a Poisson disc sampling
        offsets.AddRange(Utils.SampleOffsets(
            nSelectedUnits - 1, samplingRadius, samplingRange * Vector2.one));
        return offsets;
    }

    private List<Vector3> ComputeFormationTargetPositions(Vector3 hitPoint)
    {
        int nSelectedUnits = Globals.CURRENTLY_SELECTED_UNITS.Count;
        List<Vector3> positions = new List<Vector3>(nSelectedUnits);
        // leader unit goes to the exact target point
        positions.Add(hitPoint);
        if (nSelectedUnits == 1) // (abort early if no other unit is selected)
            return positions;

        // next units have positions computed with a Poisson disc sampling
        positions.AddRange(Utils.SamplePositions(nSelectedUnits - 1, samplingRadius, samplingRange * Vector2.one, hitPoint));
        return positions;
    }
}