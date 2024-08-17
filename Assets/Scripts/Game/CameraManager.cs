using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraManager : MonoBehaviour
{
    [Header("Camera movement settings")]
    [SerializeField] private float translationSpeed = 60f;
    [SerializeField] private float mouseBorderDelay = 0.5f;

    [Header("Zoom settings")]
    [SerializeField] private float zoomSpeed = 30f;
    [SerializeField] private float minZoom = 8;
    [SerializeField] private float maxZoom = 15;

    [Header("Other References")]
    [SerializeField] private float desiredAltitude = 10f;
    [SerializeField] private LayerMask terrainLayer;

    private Camera _camera;
    private RaycastHit raycastHit;
    private Ray ray;
    private Vector3 forwardVector;

    private int mouseOnScreenBorder;

    private Coroutine mouseOnScreenCoroutine;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        forwardVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        mouseOnScreenBorder = -1;
        mouseOnScreenCoroutine = null;
    }

    private void Update()
    {
        if (mouseOnScreenBorder >= 0)
        {
            TranslateCamera(mouseOnScreenBorder);
        }
        else
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                TranslateCamera(0);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                TranslateCamera(1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                TranslateCamera(2);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                TranslateCamera(3);
            }
        }

        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
        {
            Zoom(Input.mouseScrollDelta.y > 0f ? 1 : -1);
        }
    }

    private void TranslateCamera(int direction)
    {
        if (direction == 0) //Top
        {
            transform.Translate(forwardVector * Time.deltaTime * translationSpeed, Space.World);
        }
        else if (direction == 1)  // right
        {
            transform.Translate(transform.right * Time.deltaTime * translationSpeed);
        }
        else if (direction == 2)  // bottom
        {
            transform.Translate(-forwardVector * Time.deltaTime * translationSpeed, Space.World);
        }
        else if (direction == 3)  // left
        {
            transform.Translate(-transform.right * Time.deltaTime * translationSpeed);
        }

        // translate camera at proper altitude: cast a ray to the ground
        // and move up the hit point
        ray = new Ray(transform.position, Vector3.up * -1000f);
        if (Physics.Raycast(ray, out raycastHit, 1000f, terrainLayer))
        {
            transform.position = raycastHit.point + Vector3.up * desiredAltitude;
        }
    }

    private IEnumerator SetMouseOnScreenBorder(int borderIndex)
    {
        yield return new WaitForSeconds(mouseBorderDelay);
        mouseOnScreenBorder = borderIndex;
    }

    private void Zoom(int zoomDir)
    {
        // apply zoom
        _camera.orthographicSize += zoomDir * Time.deltaTime * zoomSpeed;
        // clamp camera distance
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, minZoom, maxZoom);
    }

    public void OnMouseEnterScreenBorder(int borderIndex)
    {
        mouseOnScreenCoroutine = StartCoroutine(SetMouseOnScreenBorder(borderIndex));
    }

    public void OnMouseExitScreenBorder()
    {
        StopCoroutine(mouseOnScreenCoroutine);
        mouseOnScreenBorder = -1;
    }
}
