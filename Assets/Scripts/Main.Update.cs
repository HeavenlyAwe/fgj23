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
        if (isDragging)
        {
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500.0f, LayerMask.GetMask("DraggingPlane")))
            {
                if (selectedGo != null)
                {
                    selectedGo.transform.position = Vector3.Lerp(selectedGo.transform.position, new Vector3(hit.point.x, hit.point.y, 0.0f), 0.1f);
                }
            }
        }
    }

    void Update()
    {
        CheckIfHoldOrTap();

        UpdateDraggablePosition();

        //CheckIfDroppedOnTarget();
    
        touchTimer += Time.deltaTime;
        tapTimer += Time.deltaTime;

        //if (selectedGo != null)
        //{
        //    if (touchTimer > 0.5f && !isDragging)
        //    {
        //        //if (isPressed)
        //        //{
        //        //    StartDragging();
        //        //}
        //        //else
        //        {
        //            int i = 0;
        //            var startNode = selectedGo.GetComponent<Blob>().node;
        //            graph.SuperDivide(startNode, tapCount+1);
        //            graph.TraverseGraph(startNode, (node) =>
        //            {
        //                if (!startNode.Equals(node))
        //                {
        //                    var go = Instantiate(nodeGo, selectedGo.transform.position + new Vector3(i++, -1, 0), Quaternion.identity);
        //                    go.GetComponent<Blob>().node = node;
        //                    go.transform.GetChild(0).GetComponent<TextMesh>().text = node.value.ToString();
        //                    node.position = go.transform.position;
        //                }
        //            });
        //            selectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
        //        }
        //    }
        //    else if (!isDragging)
        //    {
        //        StartDragging();
        //    }
        //}

        // if touchTimer > threshold -> start dragging
        // else -> tapCounter++
    
    }
}
