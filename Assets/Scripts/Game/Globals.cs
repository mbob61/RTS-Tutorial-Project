using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum InGameResource
{
    Gold,
    Wood,
    Stone
}

public class Globals {
    public static BuildingData[] AVAILABLE_BUILDINGS_DATA;
    public static List<UnitManager> CURRENTLY_SELECTED_UNITS = new List<UnitManager>();

    public static Dictionary<InGameResource, GameResource> AVAILABLE_RESOURCES =
            new Dictionary<InGameResource, GameResource>()
        {
        { InGameResource.Gold, new GameResource("Gold", 1000) },
        { InGameResource.Wood, new GameResource("Wood", 1000) },
        { InGameResource.Stone, new GameResource("Stone", 1000) }
        };

    public static LayerMask UNIT_LAYER = LayerMask.GetMask("Unit");
    public static LayerMask TREE_LAYER = LayerMask.GetMask("Tree");
    public static LayerMask ROCK_LAYER = LayerMask.GetMask("Rock");
    public static LayerMask TERRAIN_LAYER = LayerMask.GetMask("Terrain");
}
