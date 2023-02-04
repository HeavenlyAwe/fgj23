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
    
    SplitterTools.Splitter splitterTools;

    void Awake()
    {
        TouchSimulation.Enable();
        playerGo = Instantiate(Resources.Load<GameObject>("Player"));
        playerInput = playerGo.GetComponent<PlayerInput>();
        mainCamera = Camera.main;
        touchPressAction = playerInput.actions["TouchPress"];
    }
    
    void Start()
    {
        splitterTools = new SplitterTools.Splitter();

        SplitterTools.SplitterValue value = splitterTools.CountSplitValues(25, 4);

        Debug.Log(value.Count1 + "x " + value.Value1 + " and " + value.Count2 + "x " + value.Value2);
    }

    private void OnEnable()
    {
        //touchPressAction.performed += TouchPressed;
    }

    void Update()
    {

        //foreach (var action in playerInput.actions)
        //{
        //    Debug.Log(action.name);
        //    if (action.WasPerformedThisFrame())
        //    {
        //        Debug.Log(action + " performed");
        //    }
        //}

        //if (Touchscreen.current.press.isPressed)
        //{
        //    var x = Touchscreen.current.position.x.ReadValue();
        //    var y = Touchscreen.current.position.y.ReadValue();
        //    var ray = mainCamera.ScreenPointToRay(new Vector3(x, y, 0.0f));
        //    Debug.DrawRay(mainCamera.transform.position, 10.0f*ray.direction, Color.black);
        //} 
    }

    //void TouchPressed(InputAction.CallbackContext context)
    //{
    //    var value = context.ReadValue<float>();
    //    Debug.Log(value);
    //}
}
