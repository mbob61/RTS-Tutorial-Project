//The Sequence is like a logical AND:
//it iterates through its children and only succeeds if all children succeeded:

using System.Linq;
using System.Collections.Generic;

namespace BehaviourTree
{
    public class Sequence : Node
    {
        private bool isRandom;

        public Sequence() : base()
        {
            isRandom = false;
        }
        public Sequence(bool isRandom) : base()
        {
            this.isRandom = isRandom;
        }
        public Sequence(List<Node> children, bool isRandom = false) : base(children)
        {
           this.isRandom = isRandom;
        }

        public static List<T> Shuffle<T>(List<T> list)
        {
            System.Random r = new System.Random();
            return list.OrderBy(x => r.Next()).ToList();
        }

        public override NodeState Evaluate()
        {
            bool anyChildIsRunning = false;
            if (isRandom)
            {
                children = Shuffle(children);
            }

            foreach(Node child in children)
            {
                switch (child.Evaluate())
                {
                    case NodeState.FAILURE:
                        state = NodeState.FAILURE;
                        return state;
                    case NodeState.SUCCESS:
                        continue;
                    case NodeState.RUNNING:
                        anyChildIsRunning = true;
                        continue;
                    default:
                        state = NodeState.SUCCESS;
                        return state;
                }
            }
            state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
            return state;
        }
    }
}
