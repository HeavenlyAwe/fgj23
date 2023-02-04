using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using GraphTools;

public partial class Main : MonoBehaviour
{
    Dictionary<int, int> squareRootMap = new Dictionary<int, int>();

    private void PrecalculateSquares()
    {
        for (int squareRoot = 2; squareRoot < 100; squareRoot++)
        {
            squareRootMap[squareRoot * squareRoot] = squareRoot;
        }
    }

    void Awake()
    {
        PrecalculateSquares();
        //EnhancedTouchSupport.Enable();

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
        // Init node graph
        graph = new Graph(new Node(10));

        // Define walls around nodes bounce off of
        var leftPos = mainCamera.ScreenToWorldPoint(Vector3.zero);
        var rightPos = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f));
        var topPos = mainCamera.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f));
        wallLeft.transform.position = new Vector3(leftPos.x, 0.0f, 0.0f);
        wallRight.transform.position = new Vector3(rightPos.x, 0.0f, 0.0f);
        wallTop.transform.position = new Vector3(0.0f, topPos.y, 0.0f);

        // Init root node game object
        var go = Instantiate(Resources.Load<GameObject>("Metaball"), new Vector3(0.0f, 8.0f, 0.0f), Quaternion.identity);
        go.transform.GetChild(0).GetComponent<TextMesh>().text = graph.root.value.ToString();
        go.GetComponent<Blob>().node = graph.root;
        graph.root.gameObject = go;
        graph.root.position = go.transform.position;
    }
}
