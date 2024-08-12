using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer buildingPlacer;
    [SerializeField] private Transform buildingMenuParent;
    [SerializeField] private GameObject buildingButtonPrefab;

    [SerializeField] private Transform resourceUIParent;
    [SerializeField] private GameObject resourceDisplayPrefab;

    private Dictionary<string, TextMeshProUGUI> resourceTextFields;
    private Dictionary<string, Button> buildingButtons;

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>();

        // Building Purchase Buttons
        buildingButtons = new Dictionary<string, Button>();
        for (int i = 0; i < Globals.AVAILABLE_BUILDINGS_DATA.Length; i++)
        {
            GameObject button = GameObject.Instantiate(buildingButtonPrefab, buildingMenuParent);

            string code = Globals.AVAILABLE_BUILDINGS_DATA[i].code;
            button.name = code;
            button.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = Globals.AVAILABLE_BUILDINGS_DATA[i].buildingName;

            Button buttonObject = button.GetComponent<Button>();
            AddBuildingButtonListener(buttonObject, i);

            buildingButtons[code] = buttonObject;
            if (!Globals.AVAILABLE_BUILDINGS_DATA[i].IsAffordable())
            {
                Debug.Log("Unaffordable");
                buttonObject.interactable = false;
            }
        }

        // Resource information top bar
        resourceTextFields = new Dictionary<string, TextMeshProUGUI>();
        foreach (KeyValuePair<string, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            GameObject resourceDisplayObject = Instantiate(resourceDisplayPrefab, resourceUIParent);
            resourceDisplayPrefab.name = pair.Key;
            resourceTextFields[pair.Key] = resourceDisplayObject.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            SetResourcesText(pair.Key, pair.Value);
        } 
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceText", UpdateResourcesTexts);
        EventManager.AddListener("SetBuildingButtonInteractivity", SetBuildingButtonInteractivity);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceText", UpdateResourcesTexts);
        EventManager.RemoveListener("SetBuildingButtonInteractivity", SetBuildingButtonInteractivity);
    }

    private void AddBuildingButtonListener(Button button, int globalBuildingIndex)
    {
        button.onClick.AddListener(() => buildingPlacer.SelectBuildingToPlace(globalBuildingIndex));
    }

    private void SetResourcesText(string resourceName, GameResource value)
    {
        resourceTextFields[resourceName].text = value.Name + "\n" +  value.CurrentAmount.ToString();
    }

    public void UpdateResourcesTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            SetResourcesText(pair.Key, pair.Value);
        }
    }

    public void SetBuildingButtonInteractivity()
    {
        foreach (BuildingData data in Globals.AVAILABLE_BUILDINGS_DATA)
        {
            buildingButtons[data.code].interactable = data.IsAffordable();
        }
    }
}