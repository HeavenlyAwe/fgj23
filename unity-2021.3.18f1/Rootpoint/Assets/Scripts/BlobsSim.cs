using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSim : MonoBehaviour
{
    private List<GameObject> blobs = new List<GameObject>();
    public GameObject BlobNode;
    public float Clamping = 1f;
    public float Friction = 0.8f;
    public Vector3 SpawnPoint = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++) {
            GameObject newBlob = Instantiate(BlobNode, SpawnPoint, Quaternion.identity);
            newBlob.transform.Translate(
                Random.Range(-2.5f, 2.5f),
                Random.Range(-4.0f, 4.0f),
                0);
            blobs.Add(newBlob);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float f = 0;
        foreach (GameObject blob in blobs) {
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
        }
    }
}
