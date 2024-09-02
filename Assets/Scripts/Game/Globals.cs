using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public enum InGameResource
{
    Gold,
    Wood,
    Stone
}

public class Globals {
    public static BuildingData[] AVAILABLE_BUILDINGS_DATA;
    public static List<UnitManager> CURRENTLY_SELECTED_UNITS = new List<UnitManager>();

    public static Dictionary<InGameResource, GameResource>[] AVAILABLE_RESOURCES;

    public static void InitializeGameResources(int playerCount)
    {
        AVAILABLE_RESOURCES = new Dictionary<InGameResource, GameResource>[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            AVAILABLE_RESOURCES[i] = new Dictionary<InGameResource, GameResource>()
            {
                { InGameResource.Gold, new GameResource("Gold", 1000) },
                { InGameResource.Wood, new GameResource("Wood", 1000) },
                { InGameResource.Stone, new GameResource("Stone", 1000) }
            };
        }
    }

    public static Dictionary<InGameResource, int> XP_CONVERSION_TO_RESOURCE = new Dictionary<InGameResource, int>()
    {
        { InGameResource.Gold, 100 },
        { InGameResource.Wood, 80 },
        { InGameResource.Stone, 40 },
    };

    public static LayerMask UNIT_LAYER = LayerMask.GetMask("Unit");
    public static LayerMask TREE_LAYER = LayerMask.GetMask("Tree");
    public static LayerMask ROCK_LAYER = LayerMask.GetMask("Rock");
    public static LayerMask TERRAIN_LAYER = LayerMask.GetMask("Terrain");

    public static List<ResourceValue> ConvertXPCostToGameResources(int xpCost, IEnumerable<InGameResource> costResources)
    {
        // distribute the xp cost between all possible resources, always
        // starting with 1 unit of every allowed resource type and then
        // picking the rest from allowed resource types

        // sort resources by xp cost
        List<InGameResource> sortedResources = costResources
            .OrderBy(r => XP_CONVERSION_TO_RESOURCE[r])
            .ToList();
        int numberOfCostResources = sortedResources.Count();

        Dictionary<InGameResource, int> xpCostToResources = new Dictionary<InGameResource, int>();
        foreach(InGameResource r in sortedResources)
        {
            if (xpCost == 0) break;
            xpCostToResources[r] = 1;
            xpCost--;
        }

        int i = 0;
        while (xpCost > 0)
        {
            xpCostToResources[sortedResources[i]]++;
            xpCost--;
            i = (i + 1) % numberOfCostResources;
        }

        return xpCostToResources
            .Select(pair => new ResourceValue(pair.Key, pair.Value * XP_CONVERSION_TO_RESOURCE[pair.Key]))
            .ToList();
    }

    public static bool CanBuy(List<ResourceValue> cost)
    {
        return CanBuy(GameManager.instance.gamePlayersParameters.myPlayerId, cost);
    }

    // Check if we have the resources to pay for a cost
    public static bool CanBuy(int playerID, List<ResourceValue> cost)
    {
        foreach(ResourceValue resource in cost)
        {
            if (AVAILABLE_RESOURCES[playerID][resource.code].CurrentAmount < resource.amount)
            {
                return false;
            }
        }
        return true;
    }
}
