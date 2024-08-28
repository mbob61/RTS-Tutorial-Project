using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;
using System.Linq;

public class CheckEnemyInFOVRange : Node
{
    private UnitManager manager;
    private float fovRadius;
    private int unitOwner;

    private Vector3 position;

    public CheckEnemyInFOVRange(UnitManager unitManager) : base()
    {
        this.manager = unitManager;
        fovRadius = unitManager.Unit.Data.fieldOfViewRange;
        unitOwner = unitManager.Unit.Owner;
    }

    public override NodeState Evaluate()
    {
        position = manager.transform.position;
        IEnumerable<Collider> enemiesInRange = Physics.OverlapSphere(position, fovRadius, Globals.UNIT_LAYER).Where(delegate (Collider c)
        {
            UnitManager um = c.GetComponent<UnitManager>();
            if (um == null) return false;
            return um.Unit.Owner != unitOwner;
        });
        if (enemiesInRange.Any())
        {
            Parent.SetData("currentTarget", enemiesInRange.OrderBy(x => (x.transform.position - position).sqrMagnitude).First().transform);
            state = NodeState.SUCCESS;
            return state;
        }
        state = NodeState.FAILURE;
        return state;
    }

}
