using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameParameters gameParameters;
    [SerializeField] private LayerMask terrainLayer;

    private Ray ray;
    private RaycastHit raycastHit;

    private void Awake()
    {
        DataHandler.LoadGameData();

        GetComponent<DayNightCycler>().enabled = gameParameters.enableDayAndNightCycle;
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
