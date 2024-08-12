using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building", order = 1)]
public class BuildingData : ScriptableObject
{
    public string code;
    public string buildingName;
    public string description;
    public int healthpoints;
    public GameObject buildingPrefab;
    public List<ResourceValue> costs;

    public bool IsAffordable()
    {
        foreach (ResourceValue cost in costs)
        {
            if (Globals.AVAILABLE_RESOURCES[cost.code].CurrentAmount < cost.amount)
            {
                return false;
            }
        }
        return true;
    }
}


[System.Serializable]
public class ResourceValue
{
    public string code = "";
    public int amount = 0;

    public ResourceValue(string code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}