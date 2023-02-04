using GraphTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.FilePathAttribute;

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

    public bool isPressed, isDragging;
    public GameObject selectedGo = null;
    public Vector3 selectedGoPos = Vector3.zero;
    public Vector2 touchPosition = Vector2.zero;
    public Vector2 originalTouchPosition = Vector2.zero;

    Graph graph;
    GameObject nodeGo;

    SplitterTools.Splitter splitterTools;

    float touchTimer;
    int tapCount = 0;

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
        isDragging = (Vector2.Distance(touchPosition, originalTouchPosition) > 20.0f && selectedGo != null);
        if (!oldIsDragging && isDragging) StartDragging(); 
    }

    private void TouchPressedStarted(InputAction.CallbackContext context)
    {
        var x = Touchscreen.current.position.x.ReadValue();
        var y = Touchscreen.current.position.y.ReadValue();

        touchPosition = new Vector2(x, y);
        originalTouchPosition = touchPosition;

        isPressed = true;

        var ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
        {
            tapCount = (hit.transform.gameObject.Equals(selectedGo)) ? tapCount + 1 : 1;
            selectedGo = hit.transform.gameObject;

            touchTimer = 0.0f;
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
                Destroy(hitColliders[0].gameObject);
            }
            UnSelectDraggable();
        }
    }

}
