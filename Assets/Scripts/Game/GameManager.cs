using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    [SerializeField] private LayerMask terrainLayer;

    private Ray ray;
    private RaycastHit raycastHit;

    public static GameManager instance;
    public Vector3 startPosition;

    private void Awake()
    {
        DataHandler.LoadGameData();

        GetComponent<DayNightCycler>().enabled = gameGlobalParameters.enableDayAndNightCycle;

        startPosition = Utils.MiddleOfScreenPointToWorld();
    }

    public void Start()
    {
        instance = this;

        //// load all possible game parameters assets
        //GameParameters[] gameParametersList = Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        //foreach (GameParameters parameters in gameParametersList)
        //{
        //    Debug.Log(parameters.GetParametersName());
        //    Debug.Log("> Fields shown in-game:");
        //    foreach (string fieldName in parameters.FieldsToShowInGame)
        //    {
        //        Debug.Log($"    {fieldName}");
        //    }
        //}
    }

    void Update()
    {
        CheckUnitNavigation();
    }

    private void CheckUnitNavigation()
    {
        if (Globals.CURRENTLY_SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            print("at least one selected and mouse up");
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, terrainLayer))
            {
                print("I've hit the floor");
                foreach (UnitManager unit in Globals.CURRENTLY_SELECTED_UNITS)
                {
                    if (unit.GetType() == typeof(CharacterManager))
                    {
                        print("I have a character manager");
                        ((CharacterManager)unit).MoveTo(raycastHit.point);
                    }
                }
            }
        }
    }
}
