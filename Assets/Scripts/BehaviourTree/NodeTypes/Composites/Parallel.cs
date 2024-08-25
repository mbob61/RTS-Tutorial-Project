// The Parallel processes all of its children at the same time
// and only "computes" its success/failure state at the end of all child executions.
// Here, I'm going to choose a "one" success policy;
// in other words, I'll say that my Parallel node succeeds if at least one of its child nodes has succeeded:

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Parallel : Node
    {
        public Parallel() : base() { }
        public Parallel(List<Node> children) : base(children) { }

        public override NodeState Evaluate()
        {
            bool aChildIsRunning = false;
            int failedChildCount = 0;
            foreach(Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        failedChildCount++;
                        continue;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        aChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            if (failedChildCount == children.Count)
            {
                state = NodeState.FAILURE;
            } else
            {
                state = aChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            }
            return state;
        }
    }
}
