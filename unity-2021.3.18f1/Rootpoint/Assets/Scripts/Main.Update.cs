using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using static UnityEditor.FilePathAttribute;
using System.Xml.Serialization;
using TMPro;

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
        if (resetting) return;

        var materialProperty = new MaterialPropertyBlock();
        // float[] floatArray= new float[] {0.1f, 1f};
        float[] blobArray= new float[1000];
        int blobCount = 0;

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
                    if (collider.GetType() == typeof(BoxCollider))
                    {
                        collideDirection = blob.transform.position - ((BoxCollider)collider).ClosestPoint(blob.transform.position);
                    }
                    else if (collider.GetType() == typeof(SphereCollider))
                    {
                        collideDirection = blob.transform.position - ((SphereCollider)collider).ClosestPoint(blob.transform.position);
                    }
                    else
                    {
                        collideDirection = blob.transform.position - collider.ClosestPoint(blob.transform.position);
                    }
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

            float leftWallX = wallLeft.transform.position.x + wallLeft.GetComponent<BoxCollider>().size.x / 2;
            float rightWallX = wallRight.transform.position.x - wallRight.GetComponent<BoxCollider>().size.x / 2;
            float x = Mathf.Clamp(blob.transform.position.x, leftWallX, rightWallX);
            float y = blob.transform.position.y;
            float z = blob.transform.position.z;
            blob.transform.position = new Vector3(x, y, z);

            blob.GetComponent<Blob>().velocity = velocity;
            blob.GetComponent<Blob>().magnitude = Mathf.Round(velocity.magnitude * 100);

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

            node.position = blob.transform.position;
            var lineRenderer = blob.GetComponent<LineRenderer>();

            var vert1 = node.parents[0].position;
            var vert2 = node.position;
            var vert3 = (node.parents[1] != null) ? node.parents[1].position : node.position;

            lineRenderer.SetPositions(new[] { vert1, vert2, vert3 });
        });

        // blobArray[0] = 1.0f;

        materialProperty.SetInt("blobCount", blobCount);
        materialProperty.SetFloatArray("blobArray", blobArray);
        ShaderPlane.GetComponent<Renderer> ().SetPropertyBlock (materialProperty);

    }

    void Update()
    {
        //CheckIfHoldOrTap();

        if (ui.GetChild(0).gameObject.activeSelf) return; 

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
                    // 1 (2) -> -1/2, 1/2
                    // 2 (3) -> -1  , 0  ,  1 
                    //int divider = Mathf.Clamp(tapCount, 2, 5) + 1;
                    int divider = Mathf.Clamp(tapCount + 1, 2, 5);
                    var startNode = previouslySelectedGo.GetComponent<Blob>().node;
                    if (divider > startNode.value)
                    {
                        divider = startNode.value;
                    }
                    float startX = -((divider - 1) / 2.0f);
                    graph.SuperDivide(startNode, divider);
                    graph.TraverseGraph(startNode, (node) =>
                    {
                        if (!startNode.Equals(node))
                        {
                            var spawnPos = previouslySelectedGo.transform.position + new Vector3(startX, -1, 0);
                            startX += 1;
                            Debug.Log(spawnPos);
                            SpawnNode(node, spawnPos);
                        }
                    });
                    previouslySelectedGo.layer = LayerMask.NameToLayer("Ignore Raycast");
                    selectedGo = null;

                    PlaySound("splitSound");
                }

                tapCount = 0;
                VisualizeTapCount(tapCount);
                tapTimerDone = true;
            }
        }
    }

    public void SpawnNode(Node node, Vector3 spawnPos)
    {
        var clampMin = wallLeft.transform.position.x + 0.1f;
        var clampMax = wallRight.transform.position.x - 0.1f;
        spawnPos = new Vector3(Mathf.Clamp(spawnPos.x, clampMin, clampMax), spawnPos.y, spawnPos.z);

        var go = Instantiate(nodeGo, spawnPos, Quaternion.identity);

        if (squareRootMap.ContainsKey(node.value) || node.value == 1)
        {
            if (node.value != 1)
            {
                PlaySound("scoreSound");
                score += squareRootMap[node.value];
                scoreUI.text = "Score <br>" + score.ToString();
                go.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Success");
            }
            else
            {
                go.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Fail");
            }
            go.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        go.GetComponent<Blob>().node = node;
        go.transform.GetChild(0).GetComponent<TextMesh>().text = node.value.ToString();

        var lineRenderer = go.GetComponent<LineRenderer>();

        node.position = go.transform.position;
        node.gameObject = go;

        var vert1 = node.parents[0].position;
        var vert2 = node.position;
        var vert3 = (node.parents[1] != null) ? node.parents[1].position : node.position;

        lineRenderer.SetPositions(new[] { vert1, vert2, vert3 });
    }
}
