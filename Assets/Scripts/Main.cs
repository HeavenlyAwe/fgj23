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
    Camera mainCamera;

    InputAction touchPressAction;
    InputAction touchPositionAction;

    public bool isPressed;
    public GameObject selectedGameObject = null;
    private Vector3 touchPosition = Vector3.zero;

    SplitterTools.Splitter splitterTools;

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

    
}
