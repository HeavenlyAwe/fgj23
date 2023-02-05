using System;
using UnityEngine;

namespace GraphTools
{
    public class Node
    {
        public static int nodeCount = 0;
        public int ID;
        public int value;
        public Node[] parents = new Node[2];
        public Node[] children = new Node[5];
        public int childCount = 0;
        public int depth = -1;
        public Vector3 position = Vector3.zero;
        public GameObject gameObject;
        public bool selected = false;

        public Node(int value, Node parent = null)
        {
            this.value = value;
            parents[0] = parent;
            ID = nodeCount++;
            if (parent != null)
            {
                depth = parent.depth + 1;
                parent.childCount++;
            }
            else depth = 0;
        }

        public Node(int value, Node[] parents)
        {
            this.value = value;
            this.parents = parents;
            ID = nodeCount++;
        }

        public static Node operator +(Node a, Node b)
        {
            var newNode = new Node(a.value + b.value, new[] { a, b });
            a.children[0] = newNode;
            b.children[0] = newNode;
            return newNode;
        } 
    }

    public class Graph
    {
        public Node root;

        public Graph()
        {
            root = new Node(10);
        }

        public Graph(Node root)
        {
            this.root = root;
        }

        public void SuperDivide(Node node, int n)
        {
            if (n > node.value)
            {
                n = node.value;
            }
            var st = new SplitterTools.Splitter();
            var sp = st.CountSplitValues(node.value, n);

            for (int i = 0; i < sp.Count1; i++)
            {
                node.children[i] = new Node(sp.Value1, node);
            }

            for (int i = 0; i < sp.Count2; i++)
            {
                node.children[sp.Count1 + i] = new Node(sp.Value2, node);
            }

            DebugNodes();
        }

        int[] checkedNodes = { };
        public void DebugNodes()
        {
            checkedNodes = new int[Node.nodeCount];
            TraverseGraph((node) =>
            {
                var childString = "";
                foreach (var child in node.children)
                {
                    if (child != null) childString += " " + child.value.ToString();
                }
                Debug.Log(node.value + childString);
            });
        }

        public void TraverseGraph(Action<Node> callback)
        {
            TraverseGraph(root, callback);
        }

        public void TraverseGraph(Node node, Action<Node> callback)
        {
            checkedNodes = new int[Node.nodeCount];
            DFS(node, callback);
        }

        public void DFS(Node root, Action<Node> callback)
        {
            checkedNodes[root.ID] = 1;
            callback?.Invoke(root);
            foreach (var child in root.children)
            {
                if (child == null) continue;
                if (checkedNodes[child.ID] == 0)
                {
                    DFS(child, callback);
                }
            }
        }
    }


}
