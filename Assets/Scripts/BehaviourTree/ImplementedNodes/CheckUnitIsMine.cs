using BehaviourTree;

public class CheckUnitIsMine : Node
{
    private bool unitIsMine;

    public CheckUnitIsMine(UnitManager unit) : base()
    {
        unitIsMine = unit.Unit.Owner == GameManager.instance.gamePlayersParameters.myPlayerId;
    }

    public override NodeState Evaluate()
    {
        state = unitIsMine ? NodeState.SUCCESS : NodeState.FAILURE;
        return state;
    }
}
