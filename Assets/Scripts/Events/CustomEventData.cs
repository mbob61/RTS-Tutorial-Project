using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventData
{
    public UnitData unitData;
    public Unit unit;

    public CustomEventData(UnitData unitData)
    {
        this.unitData = unitData;
        this.unit = null;
    }

    public CustomEventData(Unit unit)
    {
        this.unit = unit;
        this.unitData = null;
    }
}

[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }