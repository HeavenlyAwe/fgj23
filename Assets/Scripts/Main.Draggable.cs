using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    private void SelectDraggable(in RaycastHit hit)
    {
        Debug.Log("SelectedObject is chosen");
        selectedGameObject = hit.transform.gameObject;

        Debug.Log(selectedGameObject.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));
        selectedGameObject.layer = 2;

        // Create Ghost object for preserving BOID logic

        ghostBallGo = Instantiate(Resources.Load<GameObject>("GhostBall"), selectedGameObject.transform.position, Quaternion.identity);
    }


    private void UnSelectDraggable()
    {
        Debug.Log(selectedGameObject.layer + " -> " + LayerMask.GetMask("Draggable"));
        selectedGameObject.layer = 6;
        selectedGameObject = null;

        // Destroy the Ghost object when the current object is dropped

        Destroy(ghostBallGo);
        ghostBallGo = null;
    }
}
