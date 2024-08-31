using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct UnitLevelUpData
{
    public List<ResourceValue> levelUpCost;
    public Dictionary<InGameResource, int> updatedResourceProduction;
    public int newAttackDamage;
    public float newAttackRange;

    public UnitLevelUpData(List<ResourceValue> cost,  Dictionary<InGameResource, int> newProduction, int newAttackDamage, float newAttackRange)
    {
        this.levelUpCost = cost;
        this.updatedResourceProduction = newProduction;
        this.newAttackDamage = newAttackDamage;
        this.newAttackRange = newAttackRange;
    }
}

public class Unit {

    protected UnitData data;
    protected Transform transform;
    protected int currentHealth;
    protected string uid;
    protected int level;
    protected List<SkillManager> skillManagers;
    protected int owner;
    protected Dictionary<InGameResource, int> production;

    protected int attackDamage;
    protected float attackRange;
    protected bool levelMaxedOut;

    protected UnitLevelUpData levelUpData;

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

        attackDamage = data.attackDamage;
        attackRange = data.attackRange;
        levelMaxedOut = false;
        levelUpData = GetLevelUpData();
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
                Globals.AVAILABLE_RESOURCES[owner][value.code].AddAmount(-value.amount);
            }

            EventManager.TriggerEvent("PlaySoundByName", "buildingCompleted");
        }
    }

    public bool IsAffordable()
    {
        return data.IsAffordable(owner);
    }

    public void LevelUp()
    {
        if (levelMaxedOut) return;
        level++;

        GameGlobalParameters p = GameManager.instance.gameGlobalParameters;

        production = levelUpData.updatedResourceProduction;

        attackDamage = levelUpData.newAttackDamage;
        attackRange = levelUpData.newAttackRange;

        // consume resources
        foreach(ResourceValue resource in GetLevelUpCost())
        {
            Globals.AVAILABLE_RESOURCES[owner][resource.code].AddAmount(-resource.amount);
        }
        EventManager.TriggerEvent("UpdateResourceTexts");

        // play sound / show nice VFX

        // check if reached max level
        levelMaxedOut = level == p.UnitMaxLevel();

        // prepare data for upgrade to next level
        levelUpData = GetLevelUpData();

        Debug.Log($"Level up to level {level}!");
    }

    public List<ResourceValue> GetLevelUpCost()
    {
        int xpCost = level + 2;
        //int xpCost = (int)GameManager.instance.gameGlobalParameters.experienceEvolutionCurve.Evaluate(level + 2);
        return Globals.ConvertXPCostToGameResources(xpCost, Data.costs.Select(v => v.code));
    }

    public void ProduceResources()
    {
        foreach (KeyValuePair<InGameResource, int> resource in production)
        {
            Globals.AVAILABLE_RESOURCES[owner][resource.Key].AddAmount(resource.Value);
        }
    }

    public void TriggerSkill(int index, GameObject target = null)
    {
        skillManagers[index].Trigger(target);
    }

    public Dictionary<InGameResource, int> ComputeProduction()
    {
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

        levelUpData = GetLevelUpData();

        return production;

    }

    public UnitLevelUpData GetLevelUpData()
    {
        GameGlobalParameters p = GameManager.instance.gameGlobalParameters;

        // update production: multiply production of each game resource
        // by the multiplier defined in the game global parameters
        float resourceProductionMultiplier = p.productionMultiplierCurve.Evaluate(level + 1);
        List<InGameResource> producedResources = production.Keys.ToList();
        Dictionary<InGameResource, int> newProduction = new Dictionary<InGameResource, int>(production.Count);

        foreach (InGameResource r in producedResources)
        {
            int amount = Mathf.RoundToInt(production[r] * resourceProductionMultiplier);
            newProduction[r] = amount;
        }

        // get updated attack parameters
        float attDamageMultiplier = p.attackDamageMultiplierCurve.Evaluate(level + 1);
        int newAttackDamage = Mathf.CeilToInt(attackDamage * attDamageMultiplier);
        float attRangeMultiplier = p.attackRangeMultiplierCurve.Evaluate(level + 1);
        float newAttackRange = attackRange * attRangeMultiplier;

        return new UnitLevelUpData(GetLevelUpCost(), newProduction, newAttackDamage, newAttackRange);

    }

    public UnitData Data { get => data; }
    public string Code { get => data.code; }
    public Transform Transform { get => transform; }
    public int CurrentHP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => data.healthpoints; }
    public string Uid { get => uid; }
    public int Level { get => level; }
    public Dictionary<InGameResource, int> Production { get => production; }
    public List<SkillManager> SkillManagers { get => skillManagers; }
    public int Owner { get => owner; }
    public int AttackDamage { get => attackDamage; }
    public float AttackRange { get => attackRange; }
    public bool LevelMaxedOut { get => levelMaxedOut; }
    public UnitLevelUpData LevelUpData { get => levelUpData; }
}
