using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    private Building buildingToPlace = null;

    private Ray ray;
    private RaycastHit raycastHit;
    private Vector3 lastPlacementPosition;
    [SerializeField] private LayerMask terrainLayer;

    void Start()
    {
        // for now, we'll automatically pick our first
        // building type as the type we want to build
        PrepareBuildingWithIndexForPlacement(0);
    }

    private void Update()
    {
        if (buildingToPlace != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _CancelPlacedBuilding();
                return;
            }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, terrainLayer))
            {
                buildingToPlace.SetPosition(raycastHit.point);
                if (lastPlacementPosition != raycastHit.point)
                {
                    buildingToPlace.CheckAndSetPlacementStatus();
                }
                lastPlacementPosition = raycastHit.point;
            }

            if (buildingToPlace.HasValidPlacementStatus && Input.GetMouseButtonDown(0))
            {
                // Place Building
                PlaceBuilding();
            }
        }
    }

    private void PrepareBuildingWithIndexForPlacement(int index)
    {
        //Destroy the previous "phantom" if there is one
        if (buildingToPlace != null && !buildingToPlace.HasFixedPlacementStatus)
        {
            Destroy(buildingToPlace.Transform.gameObject);
        }
        Building building = new Building(Globals.AVAILABLE_BUILDINGS_DATA[index]);
        building.Transform.GetComponent<BuildingManager>().Initialize(building);
        buildingToPlace = building;
        lastPlacementPosition = Vector3.zero;
    }

    private void _CancelPlacedBuilding()
    {
        //Destroy the "phantom" building
        Destroy(buildingToPlace.Transform.gameObject);
        buildingToPlace = null;
    }

    private void PlaceBuilding()
    {
        buildingToPlace.Place();
        //keep on building the same type of building
        PrepareBuildingWithIndexForPlacement(buildingToPlace.DataIndex);
    }

}