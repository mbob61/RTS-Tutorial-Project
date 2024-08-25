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

    [HideInInspector] public bool gameIsPaused;
    public GamePlayerParameters gamePlayerParameters;

    public float resourceProductionRate = 1f;

    private void Awake()
    {
        DataHandler.LoadGameData();

        GetComponent<DayNightCycler>().enabled = gameGlobalParameters.enableDayAndNightCycle;

        startPosition = Utils.MiddleOfScreenPointToWorld();

        gameIsPaused = false;
    }

    public void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (gameIsPaused) return;
    }

    private void OnEnable()
    {
        EventManager.AddListener("PauseGame", OnPauseGame);
        EventManager.AddListener("ResumeGame", OnResumeGame);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PauseGame", OnPauseGame);
        EventManager.RemoveListener("ResumeGame", OnResumeGame);
    }

    private void OnPauseGame()
    {
        gameIsPaused = true;
       
    }

    private void OnResumeGame()
    {
        gameIsPaused = false;
        
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        DataHandler.SaveGameData();
#endif
    }

}
