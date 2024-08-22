using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit {

    protected UnitData data;
    protected Transform transform;
    protected int currentHealth;
    protected string uid;
    protected int level;
    protected List<ResourceValue> production;
    protected List<SkillManager> skillManagers;

    public Unit(UnitData data) : this(data, new List<ResourceValue>() { }){}
    public Unit(UnitData data, List<ResourceValue> production)
    {
        this.data = data;
        this.currentHealth = data.healthpoints;

        GameObject instantiedUnit = GameObject.Instantiate(data.unitPrefab) as GameObject;
        this.transform = instantiedUnit.transform;

        this.uid = System.Guid.NewGuid().ToString();
        this.level = 1;
        this.production = production;

        skillManagers = new List<SkillManager>();
        SkillManager sm;
        foreach (SkillData skill in data.skills)
        {
            sm = instantiedUnit.AddComponent<SkillManager>();
            sm.Initialize(skill, instantiedUnit);
            skillManagers.Add(sm);
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void Place()
    {
        //Remove the "is_trigger" from the collider so the buildings can have collisions
        transform.GetComponent<BoxCollider>().isTrigger = false;

        // Update the players resources by deducting the cost of the building
        foreach (ResourceValue value in data.costs)
        {
            Globals.AVAILABLE_RESOURCES[value.code].UpdateAmount(-value.amount);
        }

        EventManager.TriggerEvent("PlaySoundByName", "buildingCompleted");
    }

    public bool IsAffordable()
    {
        return data.IsAffordable();
    }

    public void LevelUp()
    {
        level++;
    }

    public void ProduceResources()
    {
        foreach (ResourceValue resource in this.production)
        {
            Globals.AVAILABLE_RESOURCES[resource.code].UpdateAmount(resource.amount);
        }
    }

    public void TriggerSkill(int index, GameObject target = null)
    {
        skillManagers[index].Trigger(target);
    }

    public UnitData Data { get => data; }
    public string Code { get => data.code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => data.healthpoints; }
    public string Uid { get => uid; }
    public int Level { get => level; }
    public List<ResourceValue> Production { get => production; }
    public List<SkillManager> SkillManagers { get => skillManagers; }

}
