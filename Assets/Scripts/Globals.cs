using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals {
    public static BuildingData[] AVAILABLE_BUILDINGS_DATA = new BuildingData[]
    {
        new BuildingData("House", 100, new Dictionary<string, int>()
        {
            { "gold", 100 },
            { "wood", 120 }
        }),
        new BuildingData("Tower", 50, new Dictionary<string, int>()
        {
            { "gold", 80 },
            //{ "wood", 80 },
            //{ "stone", 100 }
        })
    };
    public static Dictionary<string, GameResource> AVAILABLE_RESOURCES = new Dictionary<string, GameResource>()
    {
        {"gold", new GameResource("Gold", 190) },
        {"wood", new GameResource("Wood", 210) },
        {"stone", new GameResource("Stone", 110) }
    };
}
