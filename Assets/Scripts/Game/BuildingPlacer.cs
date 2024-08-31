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

    private void OnEnable()
    {
        EventManager.AddListener("<Input>Build", OnBuildInput);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("<Input>Build", OnBuildInput);
    }

    private void Start()
    {
        SpawnBuilding(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            GameManager.instance.gamePlayerParameters.myPlayerId,
            GameManager.instance.startPosition
        );

        SpawnBuilding(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            GameManager.instance.gamePlayerParameters.myPlayerId + 1,
            GameManager.instance.startPosition + new Vector3(-15, 0f, 0f));
    }

    private void Update()
    {
        if (GameManager.instance.gameIsPaused) return;
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

                EventManager.TriggerEvent("UpdatePlacedBuildingProduction", new object[] {buildingToPlace.ComputeProduction(), buildingToPlace.Transform.position });
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
        Building building = new Building(
            Globals.AVAILABLE_BUILDINGS_DATA[index],
            GameManager.instance.gamePlayerParameters.myPlayerId);

        buildingToPlace = building;
        lastPlacementPosition = Vector3.zero;
        EventManager.TriggerEvent("PlaceBuildingOn");

    }

    private void _CancelPlacedBuilding()
    {
        //Destroy the "phantom" building
        if (buildingToPlace == null) return;
        Destroy(buildingToPlace.Transform.gameObject);
        buildingToPlace = null;
        EventManager.TriggerEvent("PlaceBuildingOff");
    }

    private void PlaceBuilding(bool canChain = true)
    {
        // Place the building
        buildingToPlace.ComputeProduction();
        buildingToPlace.Place();

        // Do not allow subsquent builds without pressing the button again
        // If this is desired, comment out the below and uncomment the below block
        //buildingToPlace = null;

        if (canChain)
        {

            //Allow continous building if we have the resources for it
            if (buildingToPlace.IsAffordable())
            {
                PrepareBuildingWithIndexForPlacement(buildingToPlace.DataIndex);
            }
            else
            {
                buildingToPlace = null;
                EventManager.TriggerEvent("PlaceBuildingOff");
            }
        }
            // Update the resources texts to reflect the purchase
            EventManager.TriggerEvent("UpdateResourceTexts");
            // Set the button interactivity based on the update resources
            EventManager.TriggerEvent("SetBuildingButtonInteractivity");
        
    }

    public void SpawnBuilding(BuildingData data, int owner, Vector3 position)
    {
        SpawnBuilding(data, owner, position, new List<ResourceValue>() { });
    }


    public void SpawnBuilding(BuildingData data, int owner, Vector3 position, List<ResourceValue> resources)
    {

        Building previousBuilding = buildingToPlace;
        

        // instantiate headquarters at the beginning of the game
        buildingToPlace = new Building(data, owner, resources);
        buildingToPlace.SetPosition(position);
        // link the data into the manager
        PlaceBuilding(false);
        // make sure we have no building selected when the player starts
        // to play
        buildingToPlace = previousBuilding;
    }

    private void OnBuildInput(object data)
    {
        string code = (string)data;
        for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++)
        {
            if (Globals.AVAILABLE_BUILDINGS_DATA[i].code == code)
            {
                SelectBuildingToPlace(i);
                return;
            }
        }

        // get building code and select it for placement
    }
}