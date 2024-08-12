using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventData
{
    public UnitData unitData;

    public CustomEventData(UnitData unitData)
    {
        this.unitData = unitData;
    }
}

[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }