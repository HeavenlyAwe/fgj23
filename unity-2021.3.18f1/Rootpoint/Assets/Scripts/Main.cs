using GraphTools;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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
    
    public GameObject selectedGo = null;
    public GameObject previouslySelectedGo = null;

    public Vector2 touchPosition = Vector2.zero;
    public Vector2 originalTouchPosition = Vector2.zero;

    public int score = 0;

    // Blob movement properties
    [Header("Blob Movement")]
    public float Clamping = 1f;
    public float Friction = 0.8f;

    Graph graph;
    GameObject nodeGo;

    SplitterTools.Splitter splitterTools;

    bool tapTimerDone = false;
    float tapTimer;
    public int tapCount = 0;

    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallTop;

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
        var oldIsDragging = isDragging;
        isDragging = ( (Vector2.Distance(touchPosition, originalTouchPosition) > 20.0f && selectedGo != null) || isDragging);
        if (!oldIsDragging && isDragging) StartDragging();
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
            previouslySelectedGo = selectedGo;

            selectedGo.GetComponent<Blob>().node.selected = true;

            tapTimer = 0.0f;
            tapTimerDone = false;
        }
    }

    private void TouchPressedCanceled(InputAction.CallbackContext context)
    {
        isPressed = false;
        if (isDragging && selectedGo != null)
        {
            SphereCollider thisCollider = selectedGo.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
            if (hitColliders.Length > 0 && !thisCollider.Equals(hitColliders[0]))
            {
                Node node1 = thisCollider.gameObject.GetComponent<Blob>().node;
                Node node2 = hitColliders[0].gameObject.GetComponent<Blob>().node;
                MergeTwoNodes(node1, node2);
                Destroy(hitColliders[0].gameObject);
            }
        }
        selectedGo.GetComponent<Blob>().node.selected = false;
        StopDragging();
    }

    private void MergeTwoNodes(Node node1, Node node2)
    {
        var sum = node1.value + node2.value;
        Debug.Log(sum);

        if (squareRootMap.ContainsKey(sum))
        {
            PlayScoreSound();
            score += squareRootMap[sum];
        }
    }


    public void PlayScoreSound()
    {
        GetComponent<AudioSource>().Play();
    }
}
