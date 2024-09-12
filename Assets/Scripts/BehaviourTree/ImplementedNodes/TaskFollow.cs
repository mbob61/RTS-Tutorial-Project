using UnityEngine;
using BehaviourTree;

public class TaskFollow : Node
{
    CharacterManager manager;
    Vector3 lastTargetPosition;
    Transform lastTarget;
    float targetSize;
    float range;

    public TaskFollow(CharacterManager manager) : base()
    {
        this.manager = manager;
        lastTargetPosition = Vector3.zero;
        lastTarget = null;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = GetData("currentTarget");
        Vector2 currentTargetOffset = (Vector2)GetData("currentTargetOffset");
        Transform target = (Transform)currentTarget;

        if (target != lastTarget)
        {
            Vector3 s = target
                .Find("Mesh")
                .GetComponent<MeshFilter>()
                .sharedMesh.bounds.size / 2;
            targetSize = Mathf.Max(s.x, s.z);

            int targetOwner = target.GetComponent<UnitManager>().Unit.Owner;
            range = (targetOwner != GameManager.instance.gamePlayersParameters.myPlayerId)
                ? manager.Unit.AttackRange
                : ((CharacterData)manager.Unit.Data).buildRange;
            lastTarget = target;
        }


        Vector3 targetPosition = GetTargetPosition(target, currentTargetOffset);

        if (targetPosition != lastTargetPosition)
        {
            manager.MoveTo(targetPosition);
            lastTargetPosition = targetPosition;
        }

        // check if the agent has reached destination
        float d = Vector3.Distance(manager.transform.position, manager.agent.destination);
        if (d <= manager.agent.stoppingDistance)
        {
            // if target is not mine: clear the data
            // (else keep it for the TaskBuild node)
            int targetOwner = ((Transform)currentTarget).GetComponent<UnitManager>().Unit.Owner;
            if (targetOwner != GameManager.instance.gamePlayersParameters.myPlayerId)
            {
                ClearData("currentTarget");
                ClearData("currentTargetOffset");
            }
            state = NodeState.SUCCESS;
            return state;
        }

        state = NodeState.RUNNING;
        return state;
    }

    private Vector3 GetTargetPosition(Transform target, Vector2 offset)
    {
        Vector3 p = manager.transform.position;
        Vector3 t = new Vector3(target.position.x + offset.x, target.position.y, target.position.z + offset.y) - p;
        // (add a little offset to avoid bad collisions)
        float d = targetSize + range - 0.05f;
        float r = d / t.magnitude;
        return p + t * (1 - r);
    }
}