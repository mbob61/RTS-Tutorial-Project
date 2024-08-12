using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestListener : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.AddListener("TestEvent", HandleEvent);
        EventManager.AddCustomListener("CustomEvent", HandleCustomEvent);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("TestEvent", HandleEventOnDisable);
    }

    private void HandleEvent()
    {
        Debug.Log("Event has been handled");
    }

    private void HandleEventOnDisable()
    {
        Debug.Log("Listener Removed for this event");
    }

    private void HandleCustomEvent(CustomEventData data)
    {
        Debug.Log("Event received: " + data.buildingData.code);
    }
}
