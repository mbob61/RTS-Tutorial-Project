using UnityEngine;

using BehaviourTree;

public class CheckHasTarget : Node
{
    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        if (currentTarget == null)
        {
            state = NodeState.FAILURE;
            return state;
        }
        object currentTargetOffset = Parent.GetData("currentTargetOffset");
        if (currentTargetOffset == null)
        {
            state = NodeState.FAILURE;
            return state;
        }

        // (in case the target object is gone - for example it died
        // and we haven't cleared it from the data yet)
        if (!(Transform)currentTarget)
        {
            Parent.ClearData("currentTarget");
            state = NodeState.FAILURE;
            return state;
        }

        state = NodeState.SUCCESS;
        return state;
    }
}