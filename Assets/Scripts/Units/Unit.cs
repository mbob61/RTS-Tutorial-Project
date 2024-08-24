using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit {

    protected UnitData data;
    protected Transform transform;
    protected int currentHealth;
    protected string uid;
    protected int level;
    protected List<SkillManager> skillManagers;
    protected int owner;
    protected Dictionary<InGameResource, int> production;

    public Unit(UnitData data, int owner) : this(data, owner, new List<ResourceValue>() { }){}
    public Unit(UnitData data, int owner, List<ResourceValue> production)
    {
        this.data = data;
        this.currentHealth = data.healthpoints;
        this.owner = owner;

        GameObject instantiedUnit = GameObject.Instantiate(data.unitPrefab) as GameObject;
        this.transform = instantiedUnit.transform;
        transform.GetComponent<UnitManager>().SetOwnerMaterial(owner);

        this.uid = System.Guid.NewGuid().ToString();
        this.level = 1;
        this.production = production.ToDictionary(rv => rv.code, rv => rv.amount);


        skillManagers = new List<SkillManager>();
        SkillManager sm;
        foreach (SkillData skill in data.skills)
        {
            sm = instantiedUnit.AddComponent<SkillManager>();
            sm.Initialize(skill, instantiedUnit);
            skillManagers.Add(sm);
        }

        transform.GetComponent<UnitManager>().Initialize(this);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public virtual void Place()
    {
        //Remove the "is_trigger" from the collider so the buildings can have collisions
        transform.GetComponent<BoxCollider>().isTrigger = false;

        if (owner == GameManager.instance.gamePlayerParameters.myPlayerId)
        {
            // Update the players resources by deducting the cost of the building
            foreach (ResourceValue value in data.costs)
            {
                Globals.AVAILABLE_RESOURCES[value.code].AddAmount(-value.amount);
            }

            EventManager.TriggerEvent("PlaySoundByName", "buildingCompleted");

            if (production.Count > 0)
            {
                GameManager.instance.ownedResourceProducingUnits.Add(this);
            }
        }
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
        foreach (KeyValuePair<InGameResource, int> resource in production)
        {
            Globals.AVAILABLE_RESOURCES[resource.Key].AddAmount(resource.Value);
        }
    }

    public void TriggerSkill(int index, GameObject target = null)
    {
        skillManagers[index].Trigger(target);
    }

    public Dictionary<InGameResource, int> ComputeProduction()
    {
        Debug.Log(data.producedResources.Length);
        if (data.producedResources.Length == 0) return null;

        GameGlobalParameters globalParameters = GameManager.instance.gameGlobalParameters;
        GamePlayerParameters playerParameters = GameManager.instance.gamePlayerParameters;
        Vector3 currentPosition = transform.position;


        if (data.producedResources.Contains(InGameResource.Gold))
        {
            int bonusBuildingsCount =
                Physics.OverlapSphere(currentPosition, globalParameters.goldBonusRange, Globals.UNIT_LAYER)
                .Where(delegate (Collider c) {
                    BuildingManager m = c.GetComponent<BuildingManager>();
                    if (m == null) return false;
                    return m.Unit.Owner == playerParameters.myPlayerId;
                })
                .Count();

            production[InGameResource.Gold] = globalParameters.baseGoldProduction + bonusBuildingsCount * globalParameters.bonusGoldProductionPerBuilding;
        }

        if (data.producedResources.Contains(InGameResource.Wood))
        {
            int treesScore =
                Physics.OverlapSphere(currentPosition, globalParameters.woodProductionRange, Globals.TREE_LAYER)
                .Select((c) => globalParameters.woodProductionFunction(Vector3.Distance(currentPosition, c.transform.position)))
                .Sum();
            production[InGameResource.Wood] = treesScore;
        }

        if (data.producedResources.Contains(InGameResource.Stone))
        {
            int rocksScore =
                Physics.OverlapSphere(currentPosition, globalParameters.woodProductionRange, Globals.ROCK_LAYER)
                .Select((c) => globalParameters.stoneProductionFunction(Vector3.Distance(currentPosition, c.transform.position)))
                .Sum();
            production[InGameResource.Stone] = rocksScore;
        }

        return production;

    }

    public UnitData Data { get => data; }
    public string Code { get => data.code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => data.healthpoints; }
    public string Uid { get => uid; }
    public int Level { get => level; }
    public Dictionary<InGameResource, int> Production { get => production; }
    public List<SkillManager> SkillManagers { get => skillManagers; }
    public int Owner { get => owner; }
}
