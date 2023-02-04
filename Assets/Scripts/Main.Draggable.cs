using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    private void SelectDraggable(in RaycastHit hit)
    {
        Debug.Log("SelectedObject is chosen");
        draggingGo = hit.transform.gameObject;

        Debug.Log(draggingGo.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));
        draggingGo.layer = 2;

        // Create Ghost object for preserving BOID logic

        ghostBallGo = Instantiate(Resources.Load<GameObject>("GhostBall"), draggingGo.transform.position, Quaternion.identity);
    }


    private void UnSelectDraggable()
    {
        Debug.Log(draggingGo.layer + " -> " + LayerMask.GetMask("Draggable"));
        draggingGo.layer = 6;
        draggingGo = null;

        // Destroy the Ghost object when the current object is dropped

        Destroy(ghostBallGo);
        ghostBallGo = null;
    }

    private void CheckIfDragging()
    {
        // Nothing being dragged yet
        if (isPressed && draggingGo == null)
        {
            Debug.Log(touchPosition);
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
            {
                SelectDraggable(in hit);
            }
        }
    }

    private void CheckIfDroppedOnTarget()
    {
        // Look for targets when dropping
        if (!isPressed && draggingGo != null)
        {
            SphereCollider thisCollider = draggingGo.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
            if (hitColliders.Length > 0)
            {
                Destroy(hitColliders[0].gameObject);
            }

            UnSelectDraggable();
        }
    }
}
