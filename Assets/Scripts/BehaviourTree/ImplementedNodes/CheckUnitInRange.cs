using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTree;

public class CheckUnitInRange : Node
{
    UnitManager manager;
    float range;

    public CheckUnitInRange(UnitManager manager, bool useAttackRange): base()
    {
        this.manager = manager;
        this.range = useAttackRange
            ? manager.Unit.AttackRange
            : ((CharacterData)manager.Unit.Data).buildRange;
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
        // and we haven't cleared it from the data yet)
        if (!target)
        {
            Parent.ClearData("currentTarget");
            Parent.ClearData("currentTargetOffset");
            state = NodeState.FAILURE;
            return state;
        }

        Vector3 scale = target.Find("Mesh").localScale;
        float targetSize = Mathf.Max(scale.x, scale.z) * 1.2f;

        float distance = Vector3.Distance(manager.transform.position, target.position);
        bool isInRange = (distance - targetSize) <= range;
        state = isInRange ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;

    }
}
