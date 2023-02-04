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
    void Update()
    {
        // Nothing being dragged yet
        if (isPressed && selectedGameObject == null)
        {
            Debug.Log(touchPosition);
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            //Debug.DrawRay(mainCamera.transform.position, 100.0f * ray.direction, Color.black);
            if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
            {
                SelectDraggable(in hit);
            }
        }

        if (isPressed && selectedGameObject != null)
        {
            var ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 500.0f, LayerMask.GetMask("DraggingPlane")))
            {
                if (selectedGameObject != null)
                {
                    selectedGameObject.transform.position = new Vector3(hit.point.x, hit.point.y, 0.0f);
                }
            }
        }

        // Look for targets when dropping
        if (!isPressed && selectedGameObject != null)
        {
            SphereCollider thisCollider = selectedGameObject.GetComponent<SphereCollider>();
            Collider[] hitColliders = Physics.OverlapSphere(thisCollider.transform.position, thisCollider.radius, LayerMask.GetMask("Draggable"));
            if (hitColliders.Length > 0)
            {
                Destroy(hitColliders[0].gameObject);
            }


            //var ray = mainCamera.ScreenPointToRay(touchPosition);
            //RaycastHit hit;
            ////Debug.DrawRay(mainCamera.transform.position, 100.0f * ray.direction, Color.black);
            //if (Physics.Raycast(ray, out hit, 300.0f, LayerMask.GetMask("Draggable")))
            //{
            //    Destroy(hit.transform.gameObject);
            //}s

            UnSelectDraggable();
        }
    }
}
