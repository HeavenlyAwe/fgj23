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
        nodeGo = Resources.Load<GameObject>("Metaball");
        playerInput = playerGo.GetComponent<PlayerInput>();
        mainCamera = Camera.main;

        touchPressAction = playerInput.actions["TouchPress"];
        touchPositionAction = playerInput.actions["TouchPosition"];

        mousePressAction = playerInput.actions["MousePress"];
    }

    void Start()
    {

        graph = new Graph(new Node(10));

        //graph.SuperDivide(graph.root, 3);
        //graph.SuperDivide(graph.root.children[0], 2);

        graph.TraverseGraph((node) =>
        {
            var go = Instantiate(Resources.Load<GameObject>("Metaball"), new Vector3 (0.0f, 8.0f, 0.0f), Quaternion.identity);
            node.gameObject = go;
            go.transform.GetChild(0).GetComponent<TextMesh>().text = node.value.ToString();
            go.GetComponent<Blob>().node = node;
            node.position = go.transform.position;

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
