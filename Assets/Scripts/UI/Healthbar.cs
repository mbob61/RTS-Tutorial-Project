using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    private Transform target;
    private Vector3 lastTargetPosition;
    private Vector2 position;
    private float yOffset;

    private Transform _camera;
    private Vector3 lastCameraPosition;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

    public void InitializeHealthBar(Transform target, float yOffset)
    {
        this.target = target;
        this.yOffset = yOffset;
    }

    private void Update()
    {
        if (lastCameraPosition == _camera.position && target && lastCameraPosition == target.position)
        {
            return;
        }
        SetPosition();
    }

    public void SetPosition()
    {
        if (!target) return;
        position = Camera.main.WorldToScreenPoint(target.position);
        position.y += yOffset;
        rectTransform.position = position;
        lastTargetPosition = target.position;
        lastCameraPosition = _camera.position;
    }
}
