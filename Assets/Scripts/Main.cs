using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Main : MonoBehaviour
{

    [HideInInspector]
    GameObject playerGo;
    [HideInInspector]
    PlayerInput playerInput;
    [HideInInspector]
    Camera mainCamera;

    InputAction touchPressAction;

    void Awake()
    {
        TouchSimulation.Enable();
        playerGo = Instantiate(Resources.Load<GameObject>("Player"));
        playerInput = playerGo.GetComponent<PlayerInput>();
        mainCamera = Camera.main;
        touchPressAction = playerInput.actions["TouchPress"];
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    void Update()
    {

        foreach (var action in playerInput.actions)
        {
            Debug.Log(action.name);
            if (action.WasPerformedThisFrame())
            {
                Debug.Log(action + " performed");
            }
        }

        if (Touchscreen.current.press.isPressed)
        {
            var x = Touchscreen.current.position.x.ReadValue();
            var y = Touchscreen.current.position.y.ReadValue();
            var ray = mainCamera.ScreenPointToRay(new Vector3(x, y, 0.0f));
            Debug.DrawRay(mainCamera.transform.position, 10.0f*ray.direction, Color.black);
        } 
    }

    void TouchPressed(InputAction.CallbackContext context)
    {
        var value = context.ReadValue<float>();
        Debug.Log(value);
    }

}
