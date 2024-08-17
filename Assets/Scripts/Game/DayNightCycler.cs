using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycler : MonoBehaviour
{
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private Transform starsTransform;

    private float starsRefreshRate;
    private float rotationAngleStep;
    private Vector3 rotationAxis;

    private void Awake()
    {
        // apply initial rotation on stars
        starsTransform.rotation = Quaternion.Euler(  gameParameters.dayInitialRatio * 360f, -30f, 0f);

        //Compute relevant calculation parameters
        starsRefreshRate = 0.1f;
        rotationAxis = starsTransform.right;
        rotationAngleStep = 360f * starsRefreshRate / gameParameters.dayLengthInSeconds;
    }

    private void Start()
    {
        StartCoroutine("UpdateStars");
    }

    private IEnumerator UpdateStars()
    {
        while (true)
        {
            starsTransform.Rotate(rotationAxis, rotationAngleStep, Space.World);
            yield return new WaitForSeconds(starsRefreshRate);

        }
    }
}
