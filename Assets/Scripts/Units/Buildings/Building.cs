using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacementStatus
{
    VALID,
    INVALID,
    FIXED
}

public class Building
{
    private BuildingData buildingData;
    private Transform transform;
    private int currentHealth;
    private BuildingPlacementStatus placementStatus;
    private List<Material> materials;
    private BuildingManager buildingManager;

    public Building(BuildingData buildingData)
    {
        this.buildingData = buildingData;
        currentHealth = buildingData.healthpoints;

        GameObject g = GameObject.Instantiate(buildingData.buildingPrefab, new Vector3(0, 1000, 0), Quaternion.identity) as GameObject;
        transform = g.transform;

        placementStatus = BuildingPlacementStatus.VALID;

        materials = new List<Material>();
        foreach (Material material in transform.Find("Mesh").GetComponent<Renderer>().materials)
        {
            materials.Add(new Material(material));
        }

        buildingManager = g.GetComponent<BuildingManager>();
        placementStatus = BuildingPlacementStatus.VALID;
        SetMaterials(placementStatus);


        // (set the materials to match the "valid" initial state)
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Place()
    {
        // Set the placement state
        placementStatus = BuildingPlacementStatus.FIXED;

        // Set Materials
        SetMaterials(placementStatus);

        //Remove the "is_trigger" from the collider so the buildings can have collisions
        transform.GetComponent<BoxCollider>().isTrigger = false;

        // Update the players resources by deducting the cost of the building
        foreach (ResourceValue value in buildingData.costs)
        {
            Globals.AVAILABLE_RESOURCES[value.code].UpdateAmount(-value.amount);
        }
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

    public bool CanBuy()
    {
        return buildingData.IsAffordable();
    }

    public bool HasFixedPlacementStatus { get => placementStatus == BuildingPlacementStatus.FIXED; }
    public bool HasValidPlacementStatus { get => placementStatus == BuildingPlacementStatus.VALID; }
    public string Code { get => buildingData.code; }
    public Transform Transform { get => transform; }
    public int HP { get => currentHealth; set => currentHealth = value; }
    public int MaxHP { get => buildingData.healthpoints; }
    public int DataIndex
    {
        get                
        {
            for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++) {
                if (Globals.AVAILABLE_BUILDINGS_DATA[i].code == buildingData.code)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
