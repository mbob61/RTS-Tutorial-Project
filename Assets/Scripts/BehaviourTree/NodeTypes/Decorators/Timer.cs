using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Timer : Node
    {
        private float delay;
        private float time;

        public delegate void TickEnded();
        public event TickEnded onTickEnded;

        public Timer(float delay, TickEnded onTickEnded = null) : base()
        {
            this.delay = delay;
            this.time = delay;
            this.onTickEnded = onTickEnded;
        }

        public Timer(float delay, List<Node> children, TickEnded onTickEnded = null) : base(children)
        {
            this.delay = delay;
            this.time = delay;
            this.onTickEnded = onTickEnded;
        }

        public override NodeState Evaluate()
        {
            if (!HasChildren) return NodeState.FAILURE;
            if (time <= 0)
            {
                time = delay;
                state = children[0].Evaluate();
                if (onTickEnded != null)
                {
                    onTickEnded();
                }
                state = NodeState.SUCCESS;
            } else
            {
                time -= Time.deltaTime;
                state = NodeState.RUNNING;
            }
            return state;
        }
    }
}