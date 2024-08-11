using System.Collections.Generic;

public class BuildingData {
    private string code;
    private int healthpoints;
    private Dictionary<string, int> cost;

    public BuildingData(string code, int healthpoints, Dictionary<string, int> cost)
    {
        this.code = code;
        this.healthpoints = healthpoints;
        this.cost = cost;
    }

    public bool IsAffordable()
    {
        foreach (KeyValuePair<string, int> pair in cost)
        {
            if (Globals.AVAILABLE_RESOURCES[pair.Key].CurrentAmount < pair.Value)
            {
                return false;
            }
        }
        return true;
    }

    public string Code { get => code; }
    public int Health { get => healthpoints; }
    public Dictionary<string, int> Cost { get => cost; }
}
