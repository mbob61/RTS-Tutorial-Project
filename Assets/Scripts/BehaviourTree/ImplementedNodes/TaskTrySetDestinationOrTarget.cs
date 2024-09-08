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

                    // assign the current target transform
                    Parent.Parent.SetData("currentTarget", raycastHit.transform);

                    if (manager.SelectIndex == 0)
                    {
                        List<Vector2> targetOffsets = ComputeFormationTargetOffsets();
                        EventManager.TriggerEvent("TargetFormationOffsets", targetOffsets);
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
        { 
            return offsets;
        }

        // next units have offsets computed with the chosen formation pattern:
        // - None -> "random" using Poisson disc sampling
        // - Line
        // - Grid
        // - XCross
        if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.None)
        {
            offsets.AddRange(Utils.SampleOffsets(nSelectedUnits - 1, samplingRadius, samplingRange * Vector2.one));
        }
        else
        {
            Vector3 dir = raycastHit.point - manager.transform.position;
            if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.Line)
            {
                offsets = UnitFormation.GetLineOffsets(nSelectedUnits, samplingRadius, dir);
            }
            else if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.Grid)
            {
                offsets = UnitFormation.GetGridOffsets(nSelectedUnits, samplingRadius, dir);
            }
            else if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.XCross)
            {
                offsets = UnitFormation.GetXCrossOffsets(nSelectedUnits, samplingRadius, dir);
            }
            }
        return offsets;
    }

    private List<Vector3> ComputeFormationTargetPositions(Vector3 hitPoint)
    {
        int nSelectedUnits = Globals.CURRENTLY_SELECTED_UNITS.Count;
        List<Vector3> positions = new List<Vector3>(nSelectedUnits);
        // leader unit goes to the exact target point
        positions.Add(hitPoint);
        if (nSelectedUnits == 1) // (abort early if no other unit is selected)
        { 
            return positions;
        }

        // next units have positions computed with the chosen formation pattern:
        // - None -> "random" using Poisson disc sampling
        // - Line
        // - Grid
        // - XCross
        if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.None)
        {
            positions.AddRange(Utils.SamplePositions(nSelectedUnits - 1, samplingRadius, samplingRange * Vector2.one, hitPoint));
        }
        else
        {
            Vector3 dir = hitPoint - manager.transform.position;
            if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.Line)
            {
                positions = UnitFormation.GetLinePositions(nSelectedUnits, samplingRadius, dir, hitPoint);
            }
            else if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.Grid)
            {
                positions = UnitFormation.GetGridPositions(nSelectedUnits, samplingRadius, dir, hitPoint);
            }
            else if (Globals.UNIT_FORMATION_TYPE == UnitFormationType.XCross)
            {
                positions = UnitFormation.GetXCrossPositions(nSelectedUnits, samplingRadius, dir, hitPoint);
            }
        }
        return positions;
    }

    public void SetFormationTargetOffset(List<Vector2> targetOffsets)
    {
        int i = manager.SelectIndex;
        if (i < 0) return; // unit isn't selected anymore
        ClearData("destinationPoint");
        Parent.Parent.SetData("currentTargetOffset", targetOffsets[i]);
    }

    public void SetFormationTargetPosition(List<Vector3> targetPositions)
    {
        int i = manager.SelectIndex;
        if (i < 0) return; // (unit is not selected anymore)
        ClearData("currentTarget");
        ClearData("currentTargetOffset");
        Parent.Parent.SetData("destinationPoint", targetPositions[i]);
    }
}