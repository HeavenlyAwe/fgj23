using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.FilePathAttribute;

public partial class Main : MonoBehaviour
{
    private void UpdateDraggablePosition()
    {
        if (isPressed && draggingGo != null)
        {
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500.0f, LayerMask.GetMask("DraggingPlane")))
            {
                if (draggingGo != null)
                {
                    draggingGo.transform.position = new Vector3(hit.point.x, hit.point.y, 0.0f);
                }
            }
        }
    }

    void Update()
    {
        CheckIfDragging();

        UpdateDraggablePosition();

        CheckIfDroppedOnTarget();
    }
}
