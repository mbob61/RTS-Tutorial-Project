using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    public static BuildingPlacer instance;
    private UnitManager builderManager;
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
        instance = this;
        SpawnBuilding(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            GameManager.instance.gamePlayersParameters.myPlayerId,
            GameManager.instance.startPosition
        );

        SpawnBuilding(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            GameManager.instance.gamePlayersParameters.myPlayerId + 1,
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
    public void SelectBuildingToPlace(BuildingData buildingData, UnitManager builder = null)
    {
        PrepareBuildingWithIndexForPlacement(buildingData);
        this.builderManager = builder;
    }

    public void SelectBuildingToPlace(int buildingDataIndex)
    {
        PrepareBuildingWithIndexForPlacement(buildingDataIndex);
    }

    private void PrepareBuildingWithIndexForPlacement(int index)
    {
        PrepareBuildingWithIndexForPlacement(Globals.AVAILABLE_BUILDINGS_DATA[index]);
    }

    private void PrepareBuildingWithIndexForPlacement(BuildingData data)
    {
        // destroy the previous "phantom" if there is one
        if (buildingToPlace != null && !buildingToPlace.HasFixedPlacementStatus)
        {
            Destroy(buildingToPlace.Transform.gameObject);
        }
        Building building = new Building( data, GameManager.instance.gamePlayersParameters.myPlayerId);
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

        // if there is a worker assigned to this construction,
        // warn its behaviour tree and deselect the building
        if (builderManager != null)
        {
            //builderManager.Select();
            builderManager
                .GetComponent<CharacterBT>()
                .StartBuildingConstruction(buildingToPlace.Transform);
            builderManager = null;

            buildingToPlace.Place();

            EventManager.TriggerEvent("PlaceBuildingOff");
            buildingToPlace = null;
        }
        else
        {
            // Place the building
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
        }        
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
        buildingToPlace.SetConstructionHP(buildingToPlace.MaxHP);
        // make sure we have no building selected when the player starts
        // to play
        buildingToPlace = previousBuilding;
    }

    private void OnBuildInput(object data)
    {
        //string code = (string)data;
        //for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++)
        //{
        //    if (Globals.AVAILABLE_BUILDINGS_DATA[i].code == code)
        //    {
        //        SelectBuildingToPlace(i);
        //        return;
        //    }
        //}


        // check to see if there is at least one selected unit
        // that can build -> we arbitrarily choose the first one
        if (Globals.CURRENTLY_SELECTED_UNITS.Count == 0) return;

        UnitManager um = null;
        foreach (UnitManager selected in Globals.CURRENTLY_SELECTED_UNITS)
        {
            if (selected is CharacterManager cm && ((CharacterData)cm.Unit.Data).buildPower > 0)
            {
                um = cm;
                break;
            }
        }
        if (um == null) return;
        builderManager = um;


        string buildingCode = (string)data;
        for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++)
        {
            if (Globals.AVAILABLE_BUILDINGS_DATA[i].code == buildingCode)
            {
                SelectBuildingToPlace(i);
                return;
            }
        }
    }
}