using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Animations;

namespace GraphTools
{
    public class Node
    {
        public static int nodeCount = 0;
        public int ID;
        public int value;
        public Node[] parents = new Node[2];
        public Node[] children = new Node[5];

        public Node(int value, Node parent = null)
        {
            this.value = value;
            parents[0] = parent;
            nodeCount++;
        }

        public Node(int value, Node[] parents)
        {
            this.value = value;
            this.parents = parents;
            nodeCount++;
        }

        public Node(int value, Node[] parents, Node[] children) 
        {
            if (children.Length > this.children.Length)
            {
                Debug.LogError("Child count exceeds allowed number!");
                return;
            }

            this.value = value;
            this.parents = parents;

            nodeCount++;
        }

        public static Node operator +(Node a, Node b) => new(a.value + b.value, new[] { a, b });
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
            var st = new SplitterTools.Splitter();
            var sp = st.CountSplitValues(node.value, n);

            for (int i = 0; i < sp.Count1; i++)
            {
                node.children[i] = new Node(sp.Value1, node);
            }

            for (int i = 0; i < sp.Count2; i++)
            {
                node.children[sp.Count1 - 1 + i] = new Node(sp.Value1, node);
            }
        }

        int[] checkedNodes = { };
        public void DebugNodes()
        {
            checkedNodes = new int[Node.nodeCount];
            TraverseGraph(root, () =>
            {
                foreach (var child in root.children)
                {
                    Debug.Log(child.value);
                }
            });
        }

        public void TraverseGraph(Node root, Action callback) 
        {
            checkedNodes[root.ID] = 1;
            callback?.Invoke();
            foreach (var child in root.children)
            {
                if (checkedNodes[child.ID] == 0)
                {
                    TraverseGraph(child, callback);
                } 
            }

        }
    }
}
