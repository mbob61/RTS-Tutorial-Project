using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckEnemyInAttackRange : Node
{
    private UnitManager manager;
    private float attackRange;

    public CheckEnemyInAttackRange(UnitManager unitManager)
    {
        this.manager = unitManager;
        this.attackRange = manager.Unit.Data.attackRange;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        if (currentTarget == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        Transform target = (Transform)currentTarget;

        // (in case the target object is gone - for example it died
        // and we haven't cleared it from the data yet
        if (!target)
        {
            Parent.ClearData("currentTarget");
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 scale = target.Find("Mesh").localScale;
        float targetSize = Mathf.Max(scale.x, scale.z);

        float distance = Vector3.Distance(manager.transform.position, target.position);
        bool isInRange = (distance - targetSize) <= attackRange;

        state = isInRange ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
