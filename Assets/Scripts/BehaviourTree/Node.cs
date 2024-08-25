using System.Collections;
using System.Collections.Generic;

namespace BehaviourTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;
        private Node parent;
        protected List<Node> children = new List<Node>();
        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public Node()
        {
            this.parent = null;
        }

        public Node(List<Node> children) : this()
        {
            SetChildren(children);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetChildren(List<Node> children)
        {
            foreach (Node child in children)
            {
                Attach(child);
            }
        }

        public void Attach(Node child)
        {
            children.Add(child);
            child.parent = this;
        }

        public void Detach(Node child)
        {
            children.Remove(child);
            child.parent = null;
        }

        public object GetData(string key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
            {
                return value;
            }

            Node parentNode = parent;
            while (parentNode != null)
            {
                value = parentNode.GetData(key);
                if (value != null)
                {
                    return value;
                }
                parentNode = parentNode.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            Node parentNode = parent;
            while (parentNode != null)
            {
                bool cleared = parentNode.ClearData(key);
                if (cleared)
                {
                    return true;
                }
                parentNode = parentNode.parent;
            }
            return false;
        }

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public NodeState State { get => state; }
        public Node Parent { get => parent; }
        public List<Node> Children { get => children; }
        public bool HasChildren { get => children.Count > 0; }
    }
}
