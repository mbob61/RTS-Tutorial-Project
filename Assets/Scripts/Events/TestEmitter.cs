using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEmitter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            EventManager.TriggerEvent("TestEvent");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            EventManager.TriggerCustomEvent("CustomEvent", new CustomEventData(Globals.AVAILABLE_BUILDINGS_DATA[0]));
        }
    }


}
