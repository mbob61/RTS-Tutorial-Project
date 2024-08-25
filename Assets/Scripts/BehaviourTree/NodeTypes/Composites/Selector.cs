// The Selector is like a logical OR:
// It iterates through its children and stops as soon as one has succeeded;
// If they all failed, then the Selector node itself returns a failure too

using System.Collections.Generic;

namespace BehaviourTree
{
    public class Selector : Node
    {
        public Selector() : base() { }
        public Selector(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;
                    case NodeState.SUCCESS:
                        return state;
                    case NodeState.RUNNING:
                        state = NodeState.RUNNING;
                        return state;
                    default:
                        continue;
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
    }
}
