using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private Building buildingToPlace = null;

    private Ray ray;
    private RaycastHit raycastHit;
    private Vector3 lastPlacementPosition;
    [SerializeField] private LayerMask terrainLayer;

    private void Start()
    {
        // instantiate headquarters at the beginning of the game

        print(GameManager.instance);

        buildingToPlace = new Building(GameManager.instance.gameParameters.initialBuilding);
        buildingToPlace.SetPosition(GameManager.instance.startPosition);
        // link the data into the manager
        buildingToPlace.Transform.GetComponent<BuildingManager>().Initialize(buildingToPlace);
        PlaceBuilding();
        // make sure we have no building selected when the player starts
        // to play
        _CancelPlacedBuilding();
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

            if (buildingToPlace.HasValidPlacementStatus && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                // Place Building
                PlaceBuilding();
            }
        }
    }

    public void SelectBuildingToPlace(int buildingDataIndex)
    {
        PrepareBuildingWithIndexForPlacement(buildingDataIndex);
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
        if (buildingToPlace == null) return;
        Destroy(buildingToPlace.Transform.gameObject);
        buildingToPlace = null;
    }

    private void PlaceBuilding()
    {
        // Place the building
        buildingToPlace.Place();

        // Do not allow subsquent builds without pressing the button again
        // If this is desired, comment out the below and uncomment the below block
        //buildingToPlace = null;

        //Allow continous building if we have the resources for it
        if (buildingToPlace.IsAffordable())
            {
                PrepareBuildingWithIndexForPlacement(buildingToPlace.DataIndex);
            }
            else
            {
                buildingToPlace = null;
            }

        // Update the resources texts to reflect the purchase
        EventManager.TriggerEvent("UpdateResourceText");
        // Set the button interactivity based on the update resources
        EventManager.TriggerEvent("SetBuildingButtonInteractivity");
    }
}