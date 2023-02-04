using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.FilePathAttribute;

public class Main : MonoBehaviour
{

    [HideInInspector]
    GameObject playerGo;
    [HideInInspector]
    PlayerInput playerInput;
    [HideInInspector]
    Camera mainCamera;

    InputAction touchPressAction;
    InputAction touchPositionAction;

    public bool isPressed;
    public GameObject selectedGameObject = null;
    private Vector3 touchPosition = Vector3.zero;

    SplitterTools.Splitter splitterTools;

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
    }

    private void TouchPositionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("TouchDeltaPerformed() called");
        Vector2 location = context.ReadValue<Vector2>();

        touchPosition = location;
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable() called");

        TouchSimulation.Enable();

        touchPositionAction.performed += TouchPositionPerformed;

        touchPressAction.started += TouchStarted;
        touchPressAction.canceled += TouchCanceled;
    }

    private void OnDisable()
    {
        TouchSimulation.Disable();

        touchPositionAction.performed -= TouchPositionPerformed;

        touchPressAction.started -= TouchStarted;
        touchPressAction.canceled -= TouchCanceled;
    }

    private void TouchStarted(InputAction.CallbackContext context)
    {
        var x = Touchscreen.current.position.x.ReadValue();
        var y = Touchscreen.current.position.y.ReadValue();

        touchPosition = new Vector3(x, y, 0.0f);

        isPressed = true;
    }

    private void TouchCanceled(InputAction.CallbackContext context)
    {
        isPressed = false;
    }

    private void SelectDraggable(in RaycastHit hit)
    {
        Debug.Log("SelectedObject is chosen");
        selectedGameObject = hit.transform.gameObject;

        Debug.Log(selectedGameObject.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));
        selectedGameObject.layer = 2;

        // Create Ghost object for preserving BOID logic
    }


    private void UnSelectDraggable()
    {
        Debug.Log(selectedGameObject.layer + " -> " + LayerMask.GetMask("Draggable"));
        selectedGameObject.layer = 6;
        selectedGameObject = null;

        // Destroy the Ghost object when the current object is dropped
    }

    void Update()
    {
        // Nothing being dragged yet
        if (isPressed && selectedGameObject == null)
        {
            Debug.Log(touchPosition);
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            //Debug.DrawRay(mainCamera.transform.position, 100.0f * ray.direction, Color.black);
            if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
            {
                SelectDraggable(in hit);
            }
        }

        if (isPressed && selectedGameObject != null)
        {
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500.0f, LayerMask.GetMask("DraggingPlane")))
            {
                if (selectedGameObject != null)
                {
                    selectedGameObject.transform.position = new Vector3(hit.point.x, hit.point.y, 0.0f);
                }
            }
        }
        
        // Look for targets when dropping
        if (!isPressed && selectedGameObject != null)
        {
            SphereCollider thisCollider = selectedGameObject.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
            if (hitColliders.Length > 0)
            {
                Destroy(hitColliders[0].gameObject);
            }

            
            //var ray = mainCamera.ScreenPointToRay(touchPosition);
            //RaycastHit hit;
            ////Debug.DrawRay(mainCamera.transform.position, 100.0f * ray.direction, Color.black);
            //if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
            //{
            //    Destroy(hit.transform.gameObject);
            //}

            UnSelectDraggable();
        }
    }
}
