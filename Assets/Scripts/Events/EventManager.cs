/* Adapted from:
 * https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomEvent : UnityEvent<object>
{
}

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> events;
    private Dictionary<string, CustomEvent> customEvents;
    private static EventManager eventManager;

    public static EventManager Instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be an active object with an EventManager component");
                } else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }

    private void Init()
    {
        if (events == null)
        {
            events = new Dictionary<string, UnityEvent>();
            customEvents = new Dictionary<string, CustomEvent>();
        }
    }

    public static void AddListener(string eventName, UnityAction listener)
    {
        UnityEvent evt = null;
        if (Instance.events.TryGetValue(eventName, out evt))
        {
            evt.AddListener(listener);
        } else
        {
            evt = new UnityEvent();
            evt.AddListener(listener);
            Instance.events.Add(eventName, evt);
        }
    }

    public static void AddListener(string eventName, UnityAction<object> listener)
    {
        CustomEvent evt = null;
        if (Instance.customEvents.TryGetValue(eventName, out evt))
        {
            evt.AddListener(listener);
        } else
        {
            evt = new CustomEvent();
            evt.AddListener(listener);
            Instance.customEvents.Add(eventName, evt);
        }
    }

    public static void RemoveListener(string eventName, UnityAction listener)
    {
        if (eventManager == null) return;
        UnityEvent evt = null;
        if (Instance.events.TryGetValue(eventName, out evt))
        {
            evt.RemoveListener(listener);
        }
    }

    public static void RemoveListener(string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        CustomEvent evt = null;
        if (Instance.customEvents.TryGetValue(eventName, out evt))
        {
            evt.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent evt = null;
        if (Instance.events.TryGetValue(eventName, out evt))
        {
            evt.Invoke();
        }
    }
    public static void TriggerEvent(string eventName, object data)
    {
        CustomEvent evt = null;
        if (Instance.customEvents.TryGetValue(eventName, out evt))
        {
            evt.Invoke(data);
        }
    }

}
