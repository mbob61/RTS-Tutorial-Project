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

    [HideInInspector] public List<Unit> ownedResourceProducingUnits = new List<Unit>();
    private float resourceProductionRate = 1f;
    private Coroutine resourceProductionCoroutine = null;

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

        resourceProductionCoroutine = StartCoroutine("ProduceResources");
    }

    void Update()
    {
        if (gameIsPaused) return;
        CheckUnitNavigation();
    }

    private void CheckUnitNavigation()
    {
        if (Globals.CURRENTLY_SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 1000f, terrainLayer))
            {
                foreach (UnitManager unit in Globals.CURRENTLY_SELECTED_UNITS)
                {
                    if (unit.GetType() == typeof(CharacterManager))
                    {
                        ((CharacterManager)unit).MoveTo(raycastHit.point);
                    }
                }
            }
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
        if (resourceProductionCoroutine != null)
        {
            StopCoroutine(resourceProductionCoroutine);
            resourceProductionCoroutine = null;
        }
    }

    private void OnResumeGame()
    {
        gameIsPaused = false;
        if (resourceProductionCoroutine == null)
        {
            resourceProductionCoroutine = StartCoroutine("ProduceResources");
        }
    }

    private IEnumerator ProduceResources()
    {
        while (true)
        {
            foreach(Unit unit in ownedResourceProducingUnits)
            {
                unit.ProduceResources();
            }
            EventManager.TriggerEvent("UpdateResourceText");
            yield return new WaitForSeconds(resourceProductionRate);
        }
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        DataHandler.SaveGameData();
#endif
    }

}
