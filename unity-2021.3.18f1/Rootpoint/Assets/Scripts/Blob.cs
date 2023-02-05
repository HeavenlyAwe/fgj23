using GraphTools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blob : MonoBehaviour
{
    public Node node;
    public Vector3 velocity;
    public float magnitude;
    public Vector3 color;
    private bool colorSet = false;

    public Vector3 getColor() {
        if (!colorSet)
        {
            colorSet = true;
            color = new Vector3(
                Random.Range(0f, 1f), // 0.4f, 0.6f
                Random.Range(0f, 1f), // 0.4f, 0.6f
                Random.Range(0f, 1f) // 0.4f, 0.6
            );
        }
        return color;
    }
}
