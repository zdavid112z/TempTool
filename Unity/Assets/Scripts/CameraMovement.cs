using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    public EventSystem eventSystem;
    private float distanceToTarget = 165;

    private Vector3 previousPosition;
    private bool dragging = false;

    public float zoomSpeed = 1f;

    void Start()    
    {
    }

    private bool IsOverUI()
    {
        PointerEventData eventData = new PointerEventData(null)
        {
            position = Input.mousePosition
        };
        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(eventData, results);
        return results.Count != 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            dragging = !IsOverUI();
        }
        else if (Input.GetMouseButton(0) && dragging)
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            Vector3 direction = previousPosition - newPosition;

            float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
            float rotationAroundXAxis = direction.y * 180; // camera moves vertically

            cam.transform.position = target.position;

            cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
            cam.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!

            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));

            previousPosition = newPosition;

        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && !IsOverUI())
        {
            cam.fieldOfView -= zoomSpeed / 8;
            if (cam.fieldOfView < 20.0f)
            {
                cam.fieldOfView = 20.0f;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && !IsOverUI())
        {
            cam.fieldOfView += zoomSpeed / 8;
            if (cam.fieldOfView > 60.0f)
            {
                cam.fieldOfView = 60.0f;
            }
        }
    }
}
