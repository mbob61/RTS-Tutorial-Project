using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Unit", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public string description;
    public int healthpoints;
    public GameObject unitPrefab;
    public List<ResourceValue> costs;
    public List<SkillData> skills = new List<SkillData>();
    public InGameResource[] producedResources;

    [Header("General Sounds")]
    public AudioClip onSelectSound;

    [Header("Attack")]
    public float attackRange;
    public int attackDamage;
    public float attackRate;
    public float fieldOfViewRange;

    public bool IsAffordable()
    {
        return Globals.CanBuy(costs);
    }
}


[System.Serializable]
public class ResourceValue
{
    public InGameResource code;
    public int amount = 0;

    public ResourceValue(InGameResource code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}