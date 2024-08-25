using BehaviourTree;

public class TaskProduceResources : Node
{
    private Unit unit;

    public TaskProduceResources(UnitManager unitManager) : base()
    {
        unit = unitManager.Unit;
    }

    public override NodeState Evaluate()
    {
        unit.ProduceResources();
        state = NodeState.SUCCESS;
        return state;
    }
}
