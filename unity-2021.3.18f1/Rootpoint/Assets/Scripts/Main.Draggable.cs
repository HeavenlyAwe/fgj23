using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Main : MonoBehaviour
{
    private void StartDragging()
    {
        Debug.Log("SelectedObject is chosen");
        Debug.Log(selectedGo.layer + " -> " + LayerMask.GetMask("Ignore Raycast"));

        if (selectedGo.GetComponent<Blob>().node.Equals(graph.root))
        {
            StopDragging("Ignore Raycast");
            return;
        }

        selectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
        // Create Ghost object for preserving BOID logic
        ghostBallGo = Instantiate(Resources.Load<GameObject>("GhostBall"), selectedGo.transform.position, Quaternion.identity);

        tapCount = 0;
        previouslySelectedGo = null;
    }

    private void StopDragging(string layerName)
    {
        isDragging = false;
        if (selectedGo == null) return;

        Debug.Log("SelectedObject is unchosen");
        Debug.Log(selectedGo.layer + " -> " + LayerMask.NameToLayer(layerName));
        selectedGo.layer = LayerMask.NameToLayer(layerName);
        selectedGo = null;
        // Destroy the Ghost object when the current object is dropped
        Destroy(ghostBallGo);
        ghostBallGo = null;
    }

}
