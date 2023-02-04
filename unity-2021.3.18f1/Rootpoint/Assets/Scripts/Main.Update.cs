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
        var materialProperty = new MaterialPropertyBlock();
        // float[] floatArray= new float[] {0.1f, 1f};
        float[] blobArray= new float[1000];
        int blobCount = 0;

        graph.TraverseGraph((node) =>
        {
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
                if (blobComponent != null) {
                    // Closest point doesn't work if spheres already intersect too much, then they just merge
                    collideDirection = blob.transform.position - collider.gameObject.transform.position;
                } else {
                    // Object position doesn't work with walls. We want to move away from wall and not from the (possibly) far away origin
                    collideDirection = blob.transform.position - collider.ClosestPoint(blob.transform.position);
                }
                float magnitude = collideDirection.magnitude;
                if (magnitude == 0) {
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
            blob.GetComponent<Blob>().magnitude = Mathf.Round(velocity.magnitude*100);

            blobArray[blobCount * 5] = blob.transform.position.x;
            blobArray[blobCount * 5 + 1] = blob.transform.position.y;

            // Turnwise R, G & B just for testing
            if (blobCount % 3 == 0)
            {
                blobArray[blobCount * 5 + 2] = 1.0f;
                blobArray[blobCount * 5 + 3] = 0.0f;
                blobArray[blobCount * 5 + 4] = 0.0f;
            } else if (blobCount % 3 == 1)
            {
                blobArray[blobCount * 5 + 2] = 0.0f;
                blobArray[blobCount * 5 + 3] = 1.0f;
                blobArray[blobCount * 5 + 4] = 0.0f;
            } else
            {
                blobArray[blobCount * 5 + 2] = 0.0f;
                blobArray[blobCount * 5 + 3] = 0.0f;
                blobArray[blobCount * 5 + 4] = 1.0f;
            }
            blobCount++;
        });

        // blobArray[0] = 1.0f;

        materialProperty.SetInt("blobCount", blobCount);
        materialProperty.SetFloatArray("blobArray", blobArray);
        ShaderPlane.GetComponent<Renderer> ().SetPropertyBlock (materialProperty);

    }

    void Update()
    {
        CheckIfHoldOrTap();

        UpdateDraggablePosition();

        UpdateBlobMovement();

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
