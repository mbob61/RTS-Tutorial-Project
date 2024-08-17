using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameParameters gameParameters;

    private void Awake()
    {
        DataHandler.LoadGameData();

        GetComponent<DayNightCycler>().enabled = gameParameters.enableDayAndNightCycle;
    }
}
