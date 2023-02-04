using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobSim : MonoBehaviour
{
    private List<GameObject> blobs = new List<GameObject>();
    public GameObject BlobNode;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 20; i++) {
            GameObject newBlob = Instantiate(BlobNode, BlobNode.transform.position, Quaternion.identity);
            newBlob.transform.Translate(
                Random.Range(-8.0f, 8.0f),
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
                Vector3 collideDirection = blob.transform.position - collider.gameObject.transform.position;
                newDirection += collideDirection;
            }
            newDirection = Vector3.ClampMagnitude(newDirection, 1);

            blob.transform.Translate(newDirection * Time.deltaTime);
        }
    }
}
