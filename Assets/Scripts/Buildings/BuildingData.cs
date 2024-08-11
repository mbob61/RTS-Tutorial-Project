// Contains Dats which represents a building
public class BuildingData {
    private string code;
    private int healthpoints;

    public BuildingData(string code, int healthpoints)
    {
        this.code = code;
        this.healthpoints = healthpoints;
    }

    public string Code { get => code; }
    public int Health { get => healthpoints; }
}
