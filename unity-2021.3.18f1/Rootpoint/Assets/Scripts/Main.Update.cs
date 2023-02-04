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

    void UpdateBlobMovement()
    {
        graph.TraverseGraph((node) =>
        {
            if (node.gameObject == null || node.Equals(graph.root)) return;

            GameObject blob = node.gameObject;
            SphereCollider thisCollider = blob.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(
                thisCollider.transform.position,
                thisCollider.radius);
            Vector3 newDirection = new Vector3(0, 0, 0);
            foreach (Collider collider in hitColliders)
            {
                Blob blobComponent = collider.gameObject.GetComponent<Blob>();
                Vector3 collideDirection;
                if (blobComponent != null)
                {
                    // If node is currently selected and dragged by player, don't contribute to collisions
                    if (blobComponent.node.selected)
                    {
                        continue;
                    }
                    // Closest point doesn't work if spheres already intersect too much, then they just merge
                    collideDirection = blob.transform.position - collider.gameObject.transform.position;
                }
                else
                {
                    // Object position doesn't work with walls. We want to move away from wall and not from the (possibly) far away origin
                    collideDirection = blob.transform.position - collider.ClosestPoint(blob.transform.position);
                }
                float magnitude = collideDirection.magnitude;
                if (magnitude == 0)
                {
                    magnitude = 0.001f;
                }
                float inverse = 1 / magnitude;
                newDirection += Vector3.Scale(collideDirection, new Vector3(inverse, inverse, inverse));
            }
            newDirection = Vector3.ClampMagnitude(newDirection, Clamping);
            Vector3 velocity = blob.GetComponent<Blob>().velocity;
            velocity += newDirection;
            velocity *= Friction;// * Time.deltaTime;

            blob.transform.Translate(velocity * Time.deltaTime);
            blob.GetComponent<Blob>().velocity = velocity;
            blob.GetComponent<Blob>().magnitude = Mathf.Round(velocity.magnitude * 100);

            node.position = blob.transform.position;
            var lineRenderer = blob.GetComponent<LineRenderer>();
            lineRenderer.SetPositions(new[] { blob.transform.position, node.parents[0].position });
        });
    }

    void Update()
    {
        //CheckIfHoldOrTap();

        UpdateDraggablePosition();

        UpdateBlobMovement();

        //CheckIfDroppedOnTarget();

        tapTimer += Time.deltaTime;

        if (previouslySelectedGo != null)
        {
            if (tapTimer >= 0.7f && !tapTimerDone)
            {
                Debug.Log("Tap timer run out");

                if (!isDragging)
                {
                    int i = 0;
                    var startNode = previouslySelectedGo.GetComponent<Blob>().node;
                    graph.SuperDivide(startNode, Mathf.Clamp(tapCount + 1, 2, 5));
                    graph.TraverseGraph(startNode, (node) =>
                    {
                        if (!startNode.Equals(node))
                        {
                            var clampMin = wallLeft.transform.position.x + 0.1f;
                            var clampMax = wallRight.transform.position.x - 0.1f;
                            var spawnPos = previouslySelectedGo.transform.position + new Vector3(i++, -1, 0);
                            spawnPos = new Vector3(Mathf.Clamp(spawnPos.x, clampMin, clampMax), spawnPos.y, spawnPos.z);

                            var go = Instantiate(nodeGo, spawnPos, Quaternion.identity);
                            if (node.value == 1)
                            {
                                go.layer = LayerMask.NameToLayer("Ignore Raycast");
                            }
                            go.GetComponent<Blob>().node = node;
                            go.transform.GetChild(0).GetComponent<TextMesh>().text = node.value.ToString();

                            var lineRenderer = go.GetComponent<LineRenderer>();

                            node.position = go.transform.position;
                            node.gameObject = go;

                            lineRenderer.SetPositions(new[] { node.position, node.parents[0].position });
                        }
                    });
                    previouslySelectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
                }

                tapCount = 0;
                tapTimerDone = true;
            }
        }
    }
}
