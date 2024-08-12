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

    public void InitializeHealthBar(Transform target, float yOffset)
    {
        this.target = target;
        this.yOffset = yOffset;
    }

    private void Update()
    {
        if (!target || lastTargetPosition == target.position)
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
    }
}
