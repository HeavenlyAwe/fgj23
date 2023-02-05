using GraphTools;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections;
using TMPro;

public partial class Main : MonoBehaviour
{

    [HideInInspector]
    GameObject playerGo;
    [HideInInspector]
    PlayerInput playerInput;
    [HideInInspector]
    GameObject ghostBallGo;

    [HideInInspector]
    Camera mainCamera;

    InputAction touchPressAction;
    InputAction touchPositionAction;

    InputAction mousePressAction;

    [Header("Informative Values")]
    public bool isPressed;
    public bool isDragging;
    public bool isScrolling = false;

    public GameObject selectedGo = null;
    public GameObject previouslySelectedGo = null;

    public Vector2 touchPosition = Vector2.zero;
    public Vector2 originalTouchPosition = Vector2.zero;

    public int score = 0;

    // Blob movement properties
    [Header("Blob Movement")]
    public float Clamping = 1f;
    public float Friction = 0.8f;

    public GameObject ShaderPlane;
    public float[] blobArray= new float[1000];

    Graph graph;
    GameObject nodeGo;

    SplitterTools.Splitter splitterTools;

    bool tapTimerDone = false;
    float tapTimer;
    public int tapCount = 0;
    public Image[] tiles;

    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallTop;

    public Transform ui;
    public float maxScrollDistance = 40;

    public AudioSource effectSource;
    public AudioSource backgroundMusic;

    bool resetting = false;
    TextMeshProUGUI scoreUI;

    private void OnEnable()
    {
        //TouchSimulation.Enable();

        touchPositionAction.performed += TouchPositionPerformed;

        touchPressAction.started += TouchPressedStarted;
        touchPressAction.canceled += TouchPressedCanceled;


        mousePressAction.started += TouchPressedStarted;
        mousePressAction.canceled += TouchPressedCanceled;
    }

    private void OnDisable()
    {
        //TouchSimulation.Disable();

        touchPositionAction.performed -= TouchPositionPerformed;

        touchPressAction.started -= TouchPressedStarted;
        touchPressAction.canceled -= TouchPressedCanceled;



        mousePressAction.started -= TouchPressedStarted;
        mousePressAction.canceled -= TouchPressedCanceled;
    }

    private void TouchPositionPerformed(InputAction.CallbackContext context)
    {
        touchPosition = context.ReadValue<Vector2>();

        if (isScrolling)
        {
            Vector3 scrollTarget = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, 10.0f));
            Vector3 scrollDelta = new Vector3(0, scrollStart.y - scrollTarget.y, 0);
            mainCamera.transform.Translate(scrollDelta);
            mainCamera.transform.position = new Vector3(0, Mathf.Clamp(mainCamera.transform.position.y, -maxScrollDistance, 1), -10);
        }
        else
        {
            var oldIsDragging = isDragging;
            isDragging = ((Vector2.Distance(touchPosition, originalTouchPosition) > 20.0f && selectedGo != null) || isDragging);
            if (!oldIsDragging && isDragging) StartDragging();
        }
    }


    private void TouchPressedStarted(InputAction.CallbackContext context)
    {
        if (isPressed) return;

        var x = Touchscreen.current.position.x.ReadValue();
        var y = Touchscreen.current.position.y.ReadValue();

        touchPosition = new Vector2(x, y);
        originalTouchPosition = touchPosition;

        isPressed = true;

        var ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
        {
            Debug.Log("  Selecting -->" + previouslySelectedGo + " " + selectedGo);
            selectedGo = hit.transform.gameObject;

            tapCount = (previouslySelectedGo == selectedGo) ? tapCount + 1 : 1;
            tapCount = tapCount == 5 ? 1 : tapCount;
            VisualizeTapCount(tapCount);

            previouslySelectedGo = selectedGo;

            selectedGo.GetComponent<Blob>().node.selected = true;

            tapTimer = 0.0f;
            tapTimerDone = false;
        }
        else if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("DraggingPlane")))
        {
            isScrolling = true;
            scrollStart = hit.point;
        }

        PlaySound("mergeSound");
    }

    Vector3 scrollStart = Vector3.zero;

    private void TouchPressedCanceled(InputAction.CallbackContext context)
    {
        isPressed = false;
        isScrolling = false;
        var resultLayer = "Draggable";
        if (isDragging && selectedGo != null)
        {
            SphereCollider thisCollider = selectedGo.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
            if (hitColliders.Length > 0 && !thisCollider.Equals(hitColliders[0]))
            {
                Node node1 = thisCollider.gameObject.GetComponent<Blob>().node;
                Node node2 = hitColliders[0].gameObject.GetComponent<Blob>().node;

                hitColliders[0].gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                resultLayer = "Ignore Raycast";

                MergeTwoNodes(node1, node2);
                //Destroy(hitColliders[0].gameObject);
            }
            selectedGo.GetComponent<Blob>().node.selected = false;
        }
        StopDragging(resultLayer);
    }

    private void MergeTwoNodes(Node node1, Node node2)
    {
        var newNode = node1 + node2;
        var spawnPos = new Vector3((node1.position.x + node2.position.x) / 2, (node1.position.y + node2.position.y) / 2 - 1.0f, 0.0f);
        SpawnNode(newNode, spawnPos);
        PlaySound("mergeSound");
    }

    public void PlaySound(string key)
    {
        effectSource.PlayOneShot(audioMap[key]);
    }

    //IEnumerator ResetRoutine(float duration)
    //{
    //    yield return new WaitForSeconds(duration);
    //    ResetGame();
    //}

    //IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    //{
    //    float currentTime = 0;
    //    float start = audioSource.volume;
    //    while (currentTime < duration)
    //    {
    //        currentTime += Time.deltaTime;
    //        audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
    //        yield return null;
    //    }
    //    yield break;
    //}

    //public void GameBackButton()
    //{
    //    PlaySound("scoreSound");
    //    float duration = audio["scoreSound"].length - 2.0f;
    //    StartCoroutine(StartFade(backgroundMusic, duration, 0.0f));
    //    StartCoroutine(ResetRoutine(duration));
    //}


    public void VisualizeTapCount(int n)
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (i <= n)
            {
                tiles[i].color = Color.red;
            }
            else
            {
                tiles[i].color = Color.white;
            }
        }
    }
}
