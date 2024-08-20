using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Globals {
    public static BuildingData[] AVAILABLE_BUILDINGS_DATA;
    public static Dictionary<string, GameResource> AVAILABLE_RESOURCES = new Dictionary<string, GameResource>()
    {
        {"gold", new GameResource("Gold", 190) },
        {"wood", new GameResource("Wood", 210) },
        {"stone", new GameResource("Stone", 110) }
    };
    public static List<UnitManager> CURRENTLY_SELECTED_UNITS = new List<UnitManager>();
}
