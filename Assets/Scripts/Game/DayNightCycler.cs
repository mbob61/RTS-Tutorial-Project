using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycler : MonoBehaviour
{
    [SerializeField] private Transform starsTransform;

    private float starsRefreshRate;
    private float rotationAngleStep;
    private Vector3 rotationAxis;

    private Coroutine starsCoroutine = null;

    private void Start()
    {

        // apply initial rotation on stars
        starsTransform.rotation = Quaternion.Euler(GameManager.instance.gameGlobalParameters.dayInitialRatio * 360f, -30f, 0f);
        // compute relevant calculation parameters
        starsRefreshRate = 0.1f;
        rotationAxis = starsTransform.right;
        rotationAngleStep = 360f * starsRefreshRate / GameManager.instance.gameGlobalParameters.dayLengthInSeconds;

        // (we add a little safety check so that if the script is
        // enabled while the game is paused, the system doesn't
        // yet start its coroutine)
        if (!GameManager.instance.gameIsPaused)
        {
            starsCoroutine = StartCoroutine("UpdateStars");
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener("PauseGame", OnPauseGame);
        EventManager.AddListener("ResumeGame", OnResumeGame);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PauseGame", OnPauseGame);
        EventManager.RemoveListener("ResumeGame", OnResumeGame);
    }

    private IEnumerator UpdateStars()
    {
        float rotation = 0f;
        while (true)
        {
            rotation = (rotation + rotationAngleStep) % 360f;
            starsTransform.Rotate(rotationAxis, rotationAngleStep, Space.World);

            // check for specific time of day, to play matching sound if need be
            if (rotation <= 90f && rotation + rotationAngleStep > 90f)
            {
                EventManager.TriggerEvent("PlaySoundByName", "onNightStartSound");
            }

            if (rotation <= 270f && rotation + rotationAngleStep > 270f)
            {
                EventManager.TriggerEvent("PlaySoundByName", "onDayStartSound");
            }

            yield return new WaitForSeconds(starsRefreshRate);

        }
    }

    private void OnPauseGame()
    {
        if (starsCoroutine != null)
        {
            StopCoroutine(starsCoroutine);
            starsCoroutine = null;
        }
    }

    private void OnResumeGame()
    {
        if (starsCoroutine == null)
        {
            starsCoroutine = StartCoroutine("UpdateStars");
        }
    }
}
