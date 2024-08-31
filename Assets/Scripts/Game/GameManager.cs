using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    public GamePlayerParameters gamePlayerParameters;
    public GameInputParameters gameInputParameters;

    [SerializeField] private LayerMask terrainLayer;

    private Ray ray;
    private RaycastHit raycastHit;

    public static GameManager instance;
    public Vector3 startPosition;

    [HideInInspector] public bool gameIsPaused;

    public float resourceProductionRate = 1f;

#if UNITY_EDITOR

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 40f, 100f, 100f));

        int newMyPlayerId = GUILayout.SelectionGrid(
            gamePlayerParameters.myPlayerId,
            gamePlayerParameters.players.Select((p, i) => i.ToString()).ToArray(),
            gamePlayerParameters.players.Length
        );

        GUILayout.EndArea();


        if (newMyPlayerId != gamePlayerParameters.myPlayerId)
        {
            gamePlayerParameters.myPlayerId = newMyPlayerId;
            EventManager.TriggerEvent("SetPlayer", newMyPlayerId);
        }
    }
#endif


        private void Awake()
    {
        DataHandler.LoadGameData();

        GetComponent<DayNightCycler>().enabled = gameGlobalParameters.enableDayAndNightCycle;

        startPosition = Utils.MiddleOfScreenPointToWorld();

        gameIsPaused = false;

        Globals.InitializeGameResources(gamePlayerParameters.players.Length);
    }

    public void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (gameIsPaused) return;
        if (Input.anyKeyDown)
        {
            gameInputParameters.CheckForInput();
        }
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
