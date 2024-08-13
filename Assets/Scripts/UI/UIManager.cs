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

    [SerializeField] private Transform resourceParent;
    [SerializeField] private GameObject resourceDisplayPrefab;

    [SerializeField] private GameObject infoPanelParent;
    [SerializeField] private GameObject infoPanelResourceDisplayPrefab;

    [SerializeField] private Transform selectedUnitsParent;
    [SerializeField] private GameObject selectedUnitDisplayPrefab;

    [SerializeField] private Transform selectionGroupsParent;

    [SerializeField] private GameObject gameResourceProductionPrefab;
    [SerializeField] private GameObject selectedUnitMenuParent;
    private RectTransform selectedUnitContentRectTransform;
    private RectTransform selectedUnitButtonsRectTransform;
    private TextMeshProUGUI selectedUnitTitleText;
    private TextMeshProUGUI selectedUnitLevelText;
    private Transform selectedUnitResourcesProductionParent;
    private Transform selectedUnitActionButtonsParent;


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
            button.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = Globals.AVAILABLE_BUILDINGS_DATA[i].unitName;

            Button buttonObject = button.GetComponent<Button>();
            AddBuildingButtonListener(buttonObject, i);

            buildingButtons[code] = buttonObject;
            if (!Globals.AVAILABLE_BUILDINGS_DATA[i].IsAffordable())
            {
                buttonObject.interactable = false;
            }
            button.GetComponent<BuildingButton>().Initialize(Globals.AVAILABLE_BUILDINGS_DATA[i]);
        }

        // Resource information top bar
        resourceTextFields = new Dictionary<string, TextMeshProUGUI>();
        foreach (KeyValuePair<string, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            GameObject resourceDisplayObject = Instantiate(resourceDisplayPrefab, resourceParent);
            resourceDisplayPrefab.name = pair.Key;
            resourceTextFields[pair.Key] = resourceDisplayObject.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            SetResourcesText(pair.Key, pair.Value);
        }

        // hide all selection group buttons
        for (int i = 1; i <= 9; i++)
        {
            ToggleSelectionGroupButton(i, false);
        }

        Transform selectedUnitMenuTransform = selectedUnitMenuParent.transform;
        selectedUnitContentRectTransform = selectedUnitMenuTransform.Find("Content").GetComponent<RectTransform>();
        selectedUnitButtonsRectTransform = selectedUnitMenuTransform.Find("Buttons").GetComponent<RectTransform>();
        selectedUnitTitleText = selectedUnitMenuTransform.Find("Content/Title").GetComponent<TextMeshProUGUI>();
        selectedUnitLevelText = selectedUnitMenuTransform.Find("Content/Level").GetComponent<TextMeshProUGUI>();
        selectedUnitResourcesProductionParent = selectedUnitMenuTransform.Find("Content/ResourcesParent");
        selectedUnitActionButtonsParent = selectedUnitMenuTransform.Find("Buttons/SpecificActions");

        ShowSelectedUnitMenu(false);

    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceText", OnUpdateResourcesTexts);
        EventManager.AddListener("SetBuildingButtonInteractivity", OnSetBuildingButtonInteractivity);
        EventManager.AddCustomListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
        EventManager.AddCustomListener("SelectUnit", _OnSelectUnit);
        EventManager.AddCustomListener("DeselectUnit", _OnDeselectUnit);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceText", OnUpdateResourcesTexts);
        EventManager.RemoveListener("SetBuildingButtonInteractivity", OnSetBuildingButtonInteractivity);
        EventManager.RemoveCustomListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
        EventManager.RemoveCustomListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveCustomListener
            ("DeselectUnit", _OnDeselectUnit);
    }

    private void AddBuildingButtonListener(Button button, int globalBuildingIndex)
    {
        button.onClick.AddListener(() => buildingPlacer.SelectBuildingToPlace(globalBuildingIndex));
    }

    private void SetResourcesText(string resourceName, GameResource value)
    {
        resourceTextFields[resourceName].text = value.Name + "\n" +  value.CurrentAmount.ToString();
    }

    public void OnUpdateResourcesTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            SetResourcesText(pair.Key, pair.Value);
        }
    }

    public void OnSetBuildingButtonInteractivity()
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

    private void _OnSelectUnit(CustomEventData data)
    {
        AddSelectedUnitToUIList(data.unit);
        SetSelectedUnitMenu(data.unit);
        ShowSelectedUnitMenu(true);
    }

    private void _OnDeselectUnit(CustomEventData data)
    {
        RemoveSelectedUnitFromUIList(data.unit.Code);
        if (Globals.CURRENTLY_SELECTED_UNITS.Count == 0)
        {
            ShowSelectedUnitMenu(false);
        }
        else
        {
            SetSelectedUnitMenu(Globals.CURRENTLY_SELECTED_UNITS[Globals.CURRENTLY_SELECTED_UNITS.Count - 1].Unit);

        }
    }

    private void AddSelectedUnitToUIList( Unit unit)
    {
        // if there is another unit of the same type already selected,
        // increase the counter
        Transform alreadyInstantiatedUnitDisplay = selectedUnitsParent.Find(unit.Code);
        if (alreadyInstantiatedUnitDisplay != null)
        {
            TextMeshProUGUI t = alreadyInstantiatedUnitDisplay.Find("Count").GetComponent<TextMeshProUGUI>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }
        // else create a brand new counter initialized with a count of 1
        else
        {
            GameObject g = GameObject.Instantiate(selectedUnitDisplayPrefab, selectedUnitsParent);
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<TextMeshProUGUI>().text = "1";
            t.Find("Title").GetComponent<TextMeshProUGUI>().text = unit.Data.unitName;
        }
    }

    private void RemoveSelectedUnitFromUIList(string code)
    {
        Transform unitDisplayItem = selectedUnitsParent.Find(code);
        if (unitDisplayItem == null) return;

        TextMeshProUGUI t = unitDisplayItem.Find("Count").GetComponent<TextMeshProUGUI>();
        int count = int.Parse(t.text);
        count -= 1;
        if (count == 0)
        {
            DestroyImmediate(unitDisplayItem.gameObject);
        }
        else
        {
            t.text = count.ToString();
        }
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

    private void ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenuParent.SetActive(show);
    }

    public void ToggleSelectionGroupButton(int index, bool visible)
    {
        selectionGroupsParent.Find(index.ToString()).gameObject.SetActive(visible);
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanelParent.SetActive(show);
    }

    private void SetSelectedUnitMenu(Unit unit)
    {
        // adapt content panel heights to match info to display
        int contentHeight = 60 + unit.Production.Count * 16;
        selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);
        selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        selectedUnitButtonsRectTransform.sizeDelta = new Vector2(70, Screen.height - contentHeight - 20);
        // update texts
        selectedUnitTitleText.text = unit.Data.unitName;
        selectedUnitLevelText.text = $"Level {unit.Level}";
        // clear resource production and reinstantiate new one
        foreach (Transform child in selectedUnitResourcesProductionParent)
            Destroy(child.gameObject);
        if (unit.Production.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in unit.Production)
            {
                g = GameObject.Instantiate(gameResourceProductionPrefab, selectedUnitResourcesProductionParent);
                t = g.transform;
                t.Find("Text").GetComponent<TextMeshProUGUI>().text = $"{resource.code}: +{resource.amount}";
            }
        }
    }
}