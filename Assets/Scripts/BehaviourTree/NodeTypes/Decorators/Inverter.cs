//The Inverter is like a logical NOT:
//it transforms a failed child into a success and vice-versa:

using System.Collections.Generic;

namespace BehaviourTree
{
    public class Inverter : Node
    {
        public Inverter() : base() { }
        public Inverter(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            if (!HasChildren) return NodeState.FAILURE;
            switch (children[0].Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.SUCCESS;
                    return state;
                case NodeState.SUCCESS:
                    state = NodeState.FAILURE;
                    return state;
                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;
                default:
                    state = NodeState.FAILURE;
                    return state;
            }
        }
    }
}
