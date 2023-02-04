using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    //private void SelectDraggable(in RaycastHit hit)
    //{
    //    Debug.Log("SelectedObject is chosen");
    //    selectedGo = hit.transform.gameObject;

    //    Debug.Log(selectedGo.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));
    //    selectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
    //    // Create Ghost object for preserving BOID logic

    //    ghostBallGo = Instantiate(Resources.Load<GameObject>("GhostBall"), selectedGo.transform.position, Quaternion.identity);
    //}

    private void StartDragging()
    {
        Debug.Log("SelectedObject is chosen");
        Debug.Log(selectedGo.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));

        selectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
        // Create Ghost object for preserving BOID logic
        ghostBallGo = Instantiate(Resources.Load<GameObject>("GhostBall"), selectedGo.transform.position, Quaternion.identity);

        tapCount = 0;
        previouslySelectedGo = null;
    }

    private void UnSelectDraggable()
    {
        isDragging = false;
        if (selectedGo == null) return;

        Debug.Log("SelectedObject is unchosen");
        Debug.Log(selectedGo.layer + " -> " + LayerMask.GetMask("Draggable"));
        selectedGo.layer = LayerMask.NameToLayer("Draggable");
        selectedGo = null;
        // Destroy the Ghost object when the current object is dropped
        Destroy(ghostBallGo);
        ghostBallGo = null;
    }

    //private void CheckIfHoldOrTap()
    //{
    //    // Nothing being dragged yet
    //    if (isPressed && selectedGo == null)
    //    {
    //        var ray = mainCamera.ScreenPointToRay(touchPosition);
    //        RaycastHit hit;
    //        if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
    //        {
    //            //SelectDraggable(hit);
    //        }
    //    }
    //}

    //private void CheckIfDroppedOnTarget()
    //{
    //    // Look for targets when dropping
    //    if (!isPressed && isDragging && selectedGo != null)
    //    {
    //        SphereCollider thisCollider = selectedGo.GetComponent<SphereCollider>();
    //        Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
    //        if (hitColliders.Length > 0)
    //        {
    //            Destroy(hitColliders[0].gameObject);
    //        }
    //        UnSelectDraggable();
    //    }
    //}
}
