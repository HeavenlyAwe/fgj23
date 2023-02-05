using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using GraphTools;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using TMPro;

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

    Dictionary<string, AudioClip> audioMap = new Dictionary<string, AudioClip>();

    private void LoadAudio()
    {
        audioMap.Add("splitSound", Resources.Load<AudioClip>("Sounds/Blob_apart"));
        audioMap.Add("mergeSound", Resources.Load<AudioClip>("Sounds/Blob_together"));
        audioMap.Add("scoreSound", Resources.Load<AudioClip>("Sounds/Blob_points_1"));
        audioMap.Add("background", Resources.Load<AudioClip>("Sounds/jedentaghund"));
    }

    void Awake()
    {
        PrecalculateSquares();
        LoadAudio();
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
        scoreUI = ui.Find("GameOverlay").Find("ScoreOverlay").Find("ScoreText").GetComponent<TextMeshProUGUI>();

        ui.Find("MainMenu").Find("RestartButton").GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
        {
            ResetGame();
        });

        // Define walls around nodes bounce off of
        var leftPos = mainCamera.ScreenToWorldPoint(Vector3.zero);
        var rightPos = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, 0.0f));
        var topPos = mainCamera.ScreenToWorldPoint(new Vector3(0.0f, Screen.height, 0.0f));

        wallLeft.transform.position = new Vector3(leftPos.x, 0.0f, 0.0f);
        wallRight.transform.position = new Vector3(rightPos.x, 0.0f, 0.0f);
        wallTop.transform.position = new Vector3(0.0f, topPos.y, 0.0f);

        InitGraph();

        VisualizeTapCount(0);
    }

    void InitGraph(int startVal = 10)
    {
        Node.nodeCount = 0;
        // Init node graph
        var root = new Node(startVal);
        graph = new Graph(root);
        // Init root node game object
        var go = Instantiate(Resources.Load<GameObject>("Metaball"), new Vector3(0.0f, 6.5f, 0.0f), Quaternion.identity);
        go.transform.GetChild(0).GetComponent<TextMesh>().text = root.value.ToString();
        scoreUI.text = "Score <br> 0";
        go.GetComponent<Blob>().node = root;
        root.gameObject = go;
        root.position = go.transform.position;
    }

    public void ResetGame()
    {
        resetting = true;
        var blobs = FindObjectsOfType<Blob>();
        foreach (var blob in blobs)
        {
            Destroy(blob.gameObject);
        }

        StartCoroutine(WaitUntilGraphDestroyed(() =>
        {
            InitGraph();
            resetting = false;
        }));
    }

    IEnumerator WaitUntilGraphDestroyed(Action callback)
    {
        while (FindObjectsOfType<Blob>().Length > 0)
        {
            yield return null;
        }

        callback?.Invoke();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
