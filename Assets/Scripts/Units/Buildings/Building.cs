using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacementStatus
{
    VALID,
    INVALID,
    FIXED
}

public class Building : Unit
{
    private BuildingPlacementStatus placementStatus;
    private List<Material> materials;
    private BuildingManager buildingManager;

    public Building(BuildingData data) : this(data, new List<ResourceValue>() { }) { }
    public Building(BuildingData data, List<ResourceValue> production) : base(data, production)
    {
        buildingManager = transform.GetComponent<BuildingManager>();
        materials = new List<Material>();
        foreach (Material material in transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            materials.Add(new Material(material));
        }
        placementStatus = BuildingPlacementStatus.VALID;
        SetMaterials(placementStatus);
    }
  
    public override void Place()
    {
        base.Place();
        // Set the placement state
        placementStatus = BuildingPlacementStatus.FIXED;

        // Set Materials
        SetMaterials(placementStatus);
    }

    public void CheckAndSetPlacementStatus()
    {
        if (placementStatus == BuildingPlacementStatus.FIXED) return;
        placementStatus = buildingManager.CheckPlacementValidity() ? BuildingPlacementStatus.VALID : BuildingPlacementStatus.INVALID;
    }

    public void SetMaterials(BuildingPlacementStatus placement)
    {
        List<Material> materials;
        if (placement == BuildingPlacementStatus.VALID)
        {
            Material refMaterial = Resources.Load("Materials/ValidPlacement") as Material;
            materials = new List<Material>();
            for (int i = 0; i < this.materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacementStatus.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/InvalidPlacement") as Material;
            materials = new List<Material>();
            for (int i = 0; i < this.materials.Count; i++)
            {
                materials.Add(refMaterial);
            }
        }
        else if (placement == BuildingPlacementStatus.FIXED)
        {
            materials = this.materials;
        }
        else
        {
            return;
        }
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }

    public bool HasFixedPlacementStatus { get => placementStatus == BuildingPlacementStatus.FIXED; }
    public bool HasValidPlacementStatus { get => placementStatus == BuildingPlacementStatus.VALID; }
    public int DataIndex
    {
        get                
        {
            for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++) {
                if (Globals.AVAILABLE_BUILDINGS_DATA[i].code == data.code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
