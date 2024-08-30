using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    private Unit selectedUnit;
    [SerializeField] private GameObject unitSkillButtonPrefab;

    [SerializeField] private GameObject gameSettingsPanel;

    private Dictionary<InGameResource, TextMeshProUGUI> resourceTextFields;

    private Dictionary<string, Button> buildingButtons;

    private TextMeshProUGUI infoPanelTitleText;
    private TextMeshProUGUI infoPanelDescriptionText;
    private Transform infoPanelResourcesParent;

    [Header("Placed Building Production")]
    public RectTransform placedBuildingProductionRectTransform;

    public float rightAmount = 40f;
    public float upAmount = 10f;

    [Header("Units Selection")]
    public GameObject selectedUnitMenuUpgradeButton;
    public GameObject selectedUnitMenuDestroyButton;

    private List<ResourceValue> selectedUnitNextLevelCost;

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
        resourceTextFields = new Dictionary<InGameResource, TextMeshProUGUI>();
        foreach (KeyValuePair<InGameResource, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            GameObject resourceDisplayObject = Instantiate(resourceDisplayPrefab, resourceParent);
            resourceDisplayPrefab.name = pair.Key.ToString();
            resourceTextFields[pair.Key] = resourceDisplayObject.transform.Find("Amount").GetComponent<TextMeshProUGUI>();
            SetResourcesText(pair.Key, pair.Value.CurrentAmount);
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

        gameSettingsPanel.SetActive(false);
        placedBuildingProductionRectTransform.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", OnUpdateResourcesTexts);
        EventManager.AddListener("SetBuildingButtonInteractivity", OnSetBuildingButtonInteractivity);
        EventManager.AddListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
        EventManager.AddListener("SelectUnit", _OnSelectUnit);
        EventManager.AddListener("DeselectUnit", _OnDeselectUnit);

        EventManager.AddListener("UpdatePlacedBuildingProduction", OnUpdatePlacedBuildingProduction);
        EventManager.AddListener("PlaceBuildingOn", OnPlaceBuildingOn);
        EventManager.AddListener("PlaceBuildingOff", OnPlaceBuildingOff);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", OnUpdateResourcesTexts);
        EventManager.RemoveListener("SetBuildingButtonInteractivity", OnSetBuildingButtonInteractivity);
        EventManager.RemoveListener("HoverBuildingButton", OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", OnUnhoverBuildingButton);
        EventManager.RemoveListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveListener("DeselectUnit", _OnDeselectUnit);

        EventManager.RemoveListener("UpdatePlacedBuildingProduction", OnUpdatePlacedBuildingProduction);
        EventManager.RemoveListener("PlaceBuildingOn", OnPlaceBuildingOn);
        EventManager.RemoveListener("PlaceBuildingOff", OnPlaceBuildingOff);
    }

    private void OnUpdatePlacedBuildingProduction(object data)
    {
        object[] values = (object[])data;
        Dictionary<InGameResource, int> production = (Dictionary<InGameResource, int>)values[0];
        Vector3 position = (Vector3)values[1];

        // clear current list
        foreach (Transform child in placedBuildingProductionRectTransform.gameObject.transform)
        {
            Destroy(child.gameObject);
        }

        // add one "resource cost" prefab per resource
        GameObject g;
        Transform t;
        foreach (KeyValuePair<InGameResource, int> pair in production)
        {
            g = GameObject.Instantiate(gameResourceProductionPrefab, placedBuildingProductionRectTransform.transform);
            t = g.transform;
            t.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = $"{pair.Key}: +{pair.Value}";
        }

        // resize container to fit the right number of lines
        //placedBuildingProductionRectTransform.sizeDelta = new Vector2(80, 24 * production.Count);

        // place container top-right of the "phantom" building
        //placedBuildingProductionRectTransform.position = (Vector2)Camera.main.WorldToScreenPoint(position);
        placedBuildingProductionRectTransform.position = (Vector2)Camera.main.WorldToScreenPoint(position) + Vector2.right * rightAmount + Vector2.up * upAmount;
    }

    private void OnPlaceBuildingOn()
    {
        placedBuildingProductionRectTransform.gameObject.SetActive(true);
    }

    private void OnPlaceBuildingOff()
    {
        placedBuildingProductionRectTransform.gameObject.SetActive(false);
    }

    private void AddBuildingButtonListener(Button button, int globalBuildingIndex)
    {
        button.onClick.AddListener(() => buildingPlacer.SelectBuildingToPlace(globalBuildingIndex));
    }

    private void SetResourcesText(InGameResource resource, int value)
    {
        resourceTextFields[resource].text = resource.ToString() + ": " + value.ToString();
    }

    public void OnUpdateResourcesTexts()
    {
        foreach(KeyValuePair<InGameResource, GameResource> pair in Globals.AVAILABLE_RESOURCES)
        {
            SetResourcesText(pair.Key, pair.Value.CurrentAmount);
        }

        CheckBuyLimits();
    }

    public void OnSetBuildingButtonInteractivity()
    {
        foreach (UnitData data in Globals.AVAILABLE_BUILDINGS_DATA)
        {
            buildingButtons[data.code].interactable = data.IsAffordable();
        }
    }

    private void OnHoverBuildingButton(object data)
    {

        SetInfoPanel((UnitData) data);
        ShowInfoPanel(true);
    }

    private void OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    private void _OnSelectUnit(object data)
    {
        Unit unit = (Unit)data;

        AddSelectedUnitToUIList(unit);
        SetSelectedUnitMenu(unit);
        ShowSelectedUnitMenu(true);
    }

    private void _OnDeselectUnit(object data)
    {
        Unit unit = (Unit) data;

        RemoveSelectedUnitFromUIList(unit.Code);
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
        selectedUnit = unit;

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

        // clear skills and make new ones
        foreach(Transform child in selectedUnitActionButtonsParent)
        {
            Destroy(child.gameObject);
        }
        if (unit.SkillManagers.Count > 0)
        {
            GameObject gm;
            Transform t;
            Button b;

            for (int i = 0; i < unit.SkillManagers.Count; i++)
            {
                gm = GameObject.Instantiate(unitSkillButtonPrefab, selectedUnitActionButtonsParent);
                t = gm.transform;
                b = gm.GetComponent<Button>();
                unit.SkillManagers[i].SetButton(b);
                t.Find("Title").GetComponent<TextMeshProUGUI>().text = unit.SkillManagers[i].skill.skillName;
                AddUnitSkillButtonListener(b, i);
            }
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
        SetInfoPanel(data.unitName, data.description, data.costs);
    }

    public void SetInfoPanel(string name, string description, List<ResourceValue> resourceCosts)
    {
        // update texts
        if (name != "")
        {
            infoPanelTitleText.text = name;
        }
        if (description != "")
        {
            infoPanelDescriptionText.text = description;
        }

        // clear resource costs and reinstantiate new ones
        foreach (Transform child in infoPanelResourcesParent)
        {
            Destroy(child.gameObject);
        }


        if (resourceCosts.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in resourceCosts)
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
        selectedUnit = unit;
        selectedUnitNextLevelCost = selectedUnit.GetLevelUpCost();


        bool unitIsMine = unit.Owner == GameManager.instance.gamePlayerParameters.myPlayerId;

        // adapt content panel heights to match info to display
        int contentHeight = unitIsMine ? 60 + unit.Production.Count * 16 : 60;
        selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);
        selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        selectedUnitButtonsRectTransform.sizeDelta = new Vector2(70, Screen.height - contentHeight - 20);

        // update texts
        selectedUnitTitleText.text = unit.Data.unitName;
        selectedUnitLevelText.text = $"Level {unit.Level}";
        // clear resource production
        foreach (Transform child in selectedUnitResourcesProductionParent)
            Destroy(child.gameObject);
        // reinstantiate new ones (if I own the unit)
        if (unitIsMine && unit.Production.Count > 0)
        {
            GameObject g; Transform t;
            foreach (KeyValuePair<InGameResource, int> resource in unit.Production)
            {
                g = GameObject.Instantiate( gameResourceProductionPrefab, selectedUnitResourcesProductionParent);
                t = g.transform;
                t.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = $"{resource.Key}: +{resource.Value}";
            }
        }
        // clear skills
        foreach (Transform child in selectedUnitActionButtonsParent)
            Destroy(child.gameObject);
        // reinstantiate new ones (if I own the unit)
        if (unitIsMine && unit.SkillManagers.Count > 0)
        {
            GameObject g; Transform t; Button b;
            for (int i = 0; i < unit.SkillManagers.Count; i++)
            {
                g = GameObject.Instantiate( unitSkillButtonPrefab, selectedUnitActionButtonsParent);
                t = g.transform;
                b = g.GetComponent<Button>();
                unit.SkillManagers[i].SetButton(b);
                t.Find("Title").GetComponent<TextMeshProUGUI>().text = unit.SkillManagers[i].skill.skillName;
                AddUnitSkillButtonListener(b, i);
            }
        }

        // hide upgrade/destroy buttons if I don't own the building
        selectedUnitMenuUpgradeButton.SetActive(unitIsMine);
        selectedUnitMenuDestroyButton.SetActive(unitIsMine);

        // if unit is mine, check if it can level up
        if (unitIsMine)
        {
            selectedUnitMenuUpgradeButton.GetComponent<Button>().interactable = Globals.CanBuy(selectedUnitNextLevelCost);
        }
    }

    private void AddUnitSkillButtonListener(Button b, int index)
    {
        b.onClick.AddListener(() => selectedUnit.TriggerSkill(index));
    }

    public void ToggleGameSettingsPanel()
    {
        bool showGameSettingsPanel = !gameSettingsPanel.activeSelf;
        gameSettingsPanel.SetActive(showGameSettingsPanel);
        EventManager.TriggerEvent(showGameSettingsPanel ? "PauseGame" : "ResumeGame");
    }

    public void HoverLevelButton()
    {
        if (selectedUnit.LevelMaxedOut) return;
        UpdateSelectedUnitLevelUpInfoPanel();
        // get latest info
        ShowInfoPanel(true);
    }

    private void UpdateSelectedUnitLevelUpInfoPanel()
    {
        int nextLevel = selectedUnit.Level + 1;
        SetInfoPanel("Level up", $"Upgrade unit to level {nextLevel}", selectedUnitNextLevelCost);
    }

    public void UnhoverLevelUpBotton()
    {
        ShowInfoPanel(false);
    }

    public void ClickLevelUpButton()
    {
        selectedUnit.LevelUp();
        SetSelectedUnitMenu(selectedUnit);
        SetInfoPanel(selectedUnit.Data);
        if (selectedUnit.LevelMaxedOut)
        {
            selectedUnitMenuUpgradeButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Maxed out";
            selectedUnitMenuUpgradeButton.GetComponent<Button>().interactable = false;
            ShowInfoPanel(false);
        }
        else
        {
            UpdateSelectedUnitLevelUpInfoPanel();
            CheckBuyLimits();
        }

    }

    private void CheckBuyLimits()
    {
        // chek if level up button is disabled or not
        if (
            selectedUnit != null &&
            selectedUnit.Owner == GameManager.instance.gamePlayerParameters.myPlayerId &&
            !selectedUnit.LevelMaxedOut &&
            Globals.CanBuy(selectedUnitNextLevelCost)
        )
            selectedUnitMenuUpgradeButton.GetComponent<Button>().interactable = true;

        //// check if building buttons are disabled or not
        //OnCheckBuildingButtons();

        // check if buy/upgrade is affordable: update text colors
        if (infoPanelParent.activeSelf)
        {
            foreach (Transform resourceDisplay in infoPanelResourcesParent)
            {
                InGameResource resourceCode = (InGameResource)System.Enum.Parse(
                    typeof(InGameResource),
                    resourceDisplay.Find("Amount").GetComponent<TextMeshProUGUI>().text.Split(":")[0]
                );
                print(resourceCode);
                //InGameResource resourceCode = (InGameResource)System.Enum.Parse(
                //    typeof(InGameResource),
                //    resourceDisplay.Find("Amount").GetComponent<TextMeshProUGUI>().text.Split(":")[0]
                //);
                //TextMeshProUGUI txt = resourceDisplay.Find("Amount").GetComponent<TextMeshProUGUI>();
                //int resourceAmount = int.Parse(txt.text);
                //if (Globals.AVAILABLE_RESOURCES[resourceCode].CurrentAmount < resourceAmount)
                //    txt.color = invalidColor;
                //else
                //    txt.color = Color.white;
            }
        }

    }
}