using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.FilePathAttribute;
using GraphTools;
public partial class Main : MonoBehaviour
{
    void Awake()
    {
        EnhancedTouchSupport.Enable();

        playerGo = Instantiate(Resources.Load<GameObject>("Player"));
        playerInput = playerGo.GetComponent<PlayerInput>();
        mainCamera = Camera.main;

        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];
    }

    void Start()
    {
        splitterTools = new SplitterTools.Splitter();

        SplitterTools.SplitterValue value = splitterTools.CountSplitValues(25, 4);

        Debug.Log(value.Count1 + "x " + value.Value1 + " and " + value.Count2 + "x " + value.Value2);

        //splitterTools = new SplitterTools.Splitter();
        //SplitterTools.SplitterValue value = splitterTools.CountSplitValues(25, 4);
        //Debug.Log(value.Count1 + "x " + value.Value1 + " and " + value.Count2 + "x " + value.Value2);

        var graph = new Graph();
        graph.SuperDivide(graph.root, 3);
        graph.SuperDivide(graph.root.children[0], 2);
        graph.DebugNodes();

        graph.TraverseGraph((node) =>
        {
            var go = Instantiate(Resources.Load<GameObject>("Metaball"));
            go.transform.GetChild(0).GetComponent<TextMesh>().text = node.value.ToString();

            go.GetComponent<Blob>().node = node;

            // Set simulation position based on node data (skip root)
            if (node.parents.Length != 0)
            {
                go.transform.position = node.position;
            }

            // Update positions for all childs of this node in data
            for (int i = 0; i < node.children.Length; i++)
            {
                if (node.children[i] == null) continue;

                node.children[i].position = go.transform.position + new Vector3(i, -1, 0);
            }
        });
    }
}
