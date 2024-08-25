using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;
    private Transform canvasTransform;
    private GameObject healthBar;
    protected BoxCollider boxCollider;
    public virtual Unit Unit { get; set; }
    private bool selected = false;
    public bool IsSelected { get => selected; }

    public AudioSource contextualSource;

    public int ownerMaterialSlotIndex = 0;

    public void Initialize(Unit unit)
    {
        boxCollider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    private void Awake()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
    }

    private void OnMouseDown()
    {
        if (IsReadyForSelection() && IsMyUnit())
        {
            SelectUnit(true, Input.GetKey(KeyCode.LeftShift));
        }
    }

    public void SelectUnit(bool singleClick, bool holdingShift)
    {
        // If this is a drag select, select the unit
        if (!singleClick)
        {
            if (!Globals.CURRENTLY_SELECTED_UNITS.Contains(this))
            {
                Select();
            }
        }
        else
        {
            // Single click, no shift
            if (!holdingShift)
            {
                //If we are single clicking without shift, wipe the selection before selecting the clicked unit
                List<UnitManager> selectedUnits = new List<UnitManager>(Globals.CURRENTLY_SELECTED_UNITS);
                foreach (UnitManager unit in selectedUnits)
                {
                    unit.DeselectUnit();
                }
                Select();
            }
            // Single click with shift
            else
            {
                // If the unit is not currently selected, select it
                if (!Globals.CURRENTLY_SELECTED_UNITS.Contains(this))
                {
                    Select();
                }
                // If it is selected, deselect it
                else
                {
                    DeselectUnit();
                }
            }
        }
    }

    private void Select()
    {
        EventManager.TriggerEvent("SelectUnit", Unit);

        Globals.CURRENTLY_SELECTED_UNITS.Add(this);
        selectionIndicator.SetActive(true);

        if (healthBar == null)
        {
            healthBar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            healthBar.transform.SetParent(canvasTransform);
            Healthbar h = healthBar.GetComponent<Healthbar>();
            Rect boundingBox = Utils.GetBoundingBoxOnScreen(
                transform.Find("Mesh").GetComponent<Renderer>().bounds,
                Camera.main
            );
            h.InitializeHealthBar(transform, boundingBox.height);
            h.SetPosition();
        }

        //play sound
        contextualSource.PlayOneShot(Unit.Data.onSelectSound);
        selected = true;
    }

    public void DeselectUnit()
    {
        // Do not deselect a unit if it is not selected
        if (!Globals.CURRENTLY_SELECTED_UNITS.Contains(this)) return;

        Globals.CURRENTLY_SELECTED_UNITS.Remove(this);
        selectionIndicator.SetActive(false);

        Destroy(healthBar);
        healthBar = null;
        EventManager.TriggerEvent("DeselectUnit", Unit);

        selected = false;
    }

    protected virtual bool IsReadyForSelection()
    {
        return true;
    }

    private bool IsMyUnit()
    {
        return Unit.Owner == GameManager.instance.gamePlayerParameters.myPlayerId;
    }

    public void SetOwnerMaterial(int owner)
    {
        Color playerColor = GameManager.instance.gamePlayerParameters.players[owner].color;
        Material[] materials = transform.Find("Mesh").GetComponent<Renderer>().materials;
        materials[ownerMaterialSlotIndex].color = playerColor;
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials;
    }
}
