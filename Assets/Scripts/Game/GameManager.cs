using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    public GamePlayerParameters gamePlayersParameters;
    public GameInputParameters gameInputParameters;

    [SerializeField] private LayerMask terrainLayer;

    private Ray ray;
    private RaycastHit raycastHit;

    public static GameManager instance;
    public Vector3 startPosition;

    [HideInInspector] public bool gameIsPaused;

    public float resourceProductionRate = 1f;

    public TestScriptableObject testScriptableObject;

#if UNITY_EDITOR

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 40f, 100f, 100f));

        int newMyPlayerId = GUILayout.SelectionGrid(
            gamePlayersParameters.myPlayerId,
            gamePlayersParameters.players.Select((p, i) => i.ToString()).ToArray(),
            gamePlayersParameters.players.Length
        );

        GUILayout.EndArea();


        if (newMyPlayerId != gamePlayersParameters.myPlayerId)
        {
            gamePlayersParameters.myPlayerId = newMyPlayerId;
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

        Globals.InitializeGameResources(gamePlayersParameters.players.Length);

        //testScriptableObject.SaveToFile();
        //testScriptableObject.LoadFromFile();
        //Debug.Log(testScriptableObject.myIntField);
        //Debug.Log(testScriptableObject.aFloat);
        //Debug.Log(testScriptableObject.bFloat);
        //Debug.Log(testScriptableObject.myBoolVariable);
        //Debug.Log(testScriptableObject.myColor);
        //Debug.Log(testScriptableObject.binding.displayName);

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

        Debug.Log("Woot");
        DataHandler.SaveGameData();
#endif
    }


}
