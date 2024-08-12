using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer buildingPlacer;
    [SerializeField] private Color invalidColor;

    [SerializeField] private Transform buildingMenuParent;
    [SerializeField] private GameObject buildingButtonPrefab;

    [SerializeField] private Transform resourceUIParent;
    [SerializeField] private GameObject resourceDisplayPrefab;

    [SerializeField] private GameObject infoPanelParent;
    [SerializeField] private GameObject infoPanelResourceDisplayPrefab;

    private Dictionary<string, TextMeshProUGUI> resourceTextFields;
    private Dictionary<string, Button> buildingButtons;

    private TextMeshProUGUI infoPanelTitleText;
    private TextMeshProUGUI infoPanelDescriptionText;
    private Transform infoPanelResourcesParent;

    private void Awake()
    {
        buildingPlacer = GetComponent<BuildingPlacer>();

        // Info Panel 
        Transform infoPanelTransform = infoPanelParent.transform;
        infoPanelTitleText = infoPanelTransform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
        infoPanelDescriptionText = infoPanelTransform.Find("Content/Description").GetComponent<TextMeshProUGUI>();
        infoPanelResourcesParent = infoPanelTransform.Find("Content/Resources");

        ShowInfoPanel(false);
        
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
            button.GetComponent<BuildingButton>().Initialize(Globals.AVAILABLE_BUILDINGS_DATA[i]);
        }


        // Add the hover listeners for the buttons

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
        EventManager.AddCustomListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceText", UpdateResourcesTexts);
        EventManager.RemoveListener("SetBuildingButtonInteractivity", SetBuildingButtonInteractivity);
        EventManager.RemoveCustomListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
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
        foreach (UnitData data in Globals.AVAILABLE_BUILDINGS_DATA)
        {
            buildingButtons[data.code].interactable = data.IsAffordable();
        }
    }

    private void OnHoverBuildingButton(CustomEventData data)
    {
        SetInfoPanel(data.unitData);
        ShowInfoPanel(true);
    }

    private void OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    public void SetInfoPanel(UnitData data)
    {
        // update texts
        if (data.code != "")
        {
            infoPanelTitleText.text = data.code;
        }
        if (data.description != "")
        {
            infoPanelDescriptionText.text = data.description;
        }

        // clear resource costs and reinstantiate new ones
        foreach (Transform child in infoPanelResourcesParent)
            Destroy(child.gameObject);

        if (data.costs.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in data.costs)
            {
                g = GameObject.Instantiate(infoPanelResourceDisplayPrefab, infoPanelResourcesParent);
                t = g.transform;
                t.Find("Amount").GetComponent<TextMeshProUGUI>().text = resource.code + ": " + resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{resource.code}");

                if (Globals.AVAILABLE_RESOURCES[resource.code].CurrentAmount < resource.amount)
                {
                    t.Find("Amount").GetComponent<TextMeshProUGUI>().color = invalidColor;
                }
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanelParent.SetActive(show);
    }
}