/* Adapted from:
 * https://learn.unity.com/tutorial/create-a-simple-messaging-system-with-events#5cf5960fedbc2a281acd21fa */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private static EventManager eventManager;
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManager script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.init();
                }
            }
            return eventManager;
        }
    }

    private Dictionary<string, UnityEvent> events;
    private Dictionary<string, CustomEvent> customEvents;
   
    private void init()
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
        if (instance.events.TryGetValue(eventName, out evt))
        {
            evt.AddListener(listener);
        } else
        {
            evt = new UnityEvent();
            evt.AddListener(listener);
            instance.events.Add(eventName, evt);
        }
    }

    public static void RemoveListener(string eventName, UnityAction listener)
    {
        if (eventManager == null)
        {
            return;
        }

        UnityEvent evt = null;
        if (instance.events.TryGetValue(eventName, out evt))
        {
            evt.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName)
    {
        UnityEvent evt = null;
        if (instance.events.TryGetValue(eventName, out evt))
        {
            evt.Invoke();
        }
    }

    public static void AddCustomListener(string eventName, UnityAction<CustomEventData> listener)
    {
        CustomEvent evt = null;
        if (instance.customEvents.TryGetValue(eventName, out evt))
        {
            evt.AddListener(listener);
        }
        else
        {
            evt = new CustomEvent();
            evt.AddListener(listener);
            instance.customEvents.Add(eventName, evt);
        }
    }

    public static void RemoveCustomListener(string eventName, UnityAction<CustomEventData> listener)
    {
        if (eventManager == null) return;
        CustomEvent evt = null;
        if (instance.customEvents.TryGetValue(eventName, out evt))
            evt.RemoveListener(listener);
    }

    public static void TriggerCustomEvent(string eventName, CustomEventData data)
    {
        CustomEvent evt = null;
        if (instance.customEvents.TryGetValue(eventName, out evt))
            evt.Invoke(data);
    }
}
