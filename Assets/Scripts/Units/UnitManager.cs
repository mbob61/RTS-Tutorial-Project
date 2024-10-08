using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;
    private Transform canvasTransform;
    protected GameObject healthbar;
    protected BoxCollider boxCollider;
    public virtual Unit Unit { get; set; }
    private bool selected = false;
    public bool IsSelected { get => selected; }
    private int selectIndex = -1;
    public int SelectIndex { get => selectIndex; }

    public AudioSource contextualSource;

    public int ownerMaterialSlotIndex = 0;

    public void Initialize(Unit unit)
    {
        boxCollider = GetComponent<BoxCollider>();
        Unit = unit;
    }

    private void Update()
    {
        if (selected && Input.GetKeyDown(KeyCode.L))
        {
            if (Globals.CanBuy(Unit.GetLevelUpCost()))
            {
                Unit.LevelUp();
            } else
            {
                Debug.LogError("Can't buy the upgrade!");
            }
        }
    }

    private void Awake()
    {
        canvasTransform = GameObject.Find("Canvas").transform;
    }

    private void OnMouseDown()
    {
        //if (IsReadyForSelection() && IsMyUnit())
        //{
        //    SelectUnit(true, Input.GetKey(KeyCode.LeftShift));
        //}

        if (IsReadyForSelection())
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

    public virtual void Select()
    {
        EventManager.TriggerEvent("SelectUnit", Unit);

        Globals.CURRENTLY_SELECTED_UNITS.Add(this);
        selectionIndicator.SetActive(true);

        if (healthbar == null)
        {
            healthbar = GameObject.Instantiate(Resources.Load("Prefabs/UI/Healthbar")) as GameObject;
            healthbar.transform.SetParent(canvasTransform);
            Healthbar h = healthbar.GetComponent<Healthbar>();
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
        selectIndex = Globals.CURRENTLY_SELECTED_UNITS.Count - 1;
    }

    public void DeselectUnit()
    {
        // Do not deselect a unit if it is not selected
        if (!Globals.CURRENTLY_SELECTED_UNITS.Contains(this)) return;

        Globals.CURRENTLY_SELECTED_UNITS.Remove(this);
        selectionIndicator.SetActive(false);

        Destroy(healthbar);
        healthbar = null;
        EventManager.TriggerEvent("DeselectUnit", Unit);

        selected = false;
        selectIndex = -1;
    }

    protected virtual bool IsReadyForSelection()
    {
        return true;
    }

    private bool IsMyUnit()
    {
        return Unit.Owner == GameManager.instance.gamePlayersParameters.myPlayerId;
    }

    public void SetOwnerMaterial(int owner)
    {
        Color playerColor = GameManager.instance.gamePlayersParameters.players[owner].color;
        Material[] materials = transform.Find("Mesh").GetComponent<Renderer>().materials;
        materials[ownerMaterialSlotIndex].color = playerColor;
        transform.Find("Mesh").GetComponent<Renderer>().materials = materials;
    }

    public void Attack(Transform target)
    {
        UnitManager um = target.GetComponent<UnitManager>();
        if (um == null) return;
        um.TakeDamage(Unit.AttackDamage);
    }

    public void TakeDamage(int damage)
    {
        Unit.CurrentHP -= damage;
        UpdateHealthBar();
        if (Unit.CurrentHP <= 0) Die();
    }

    public void Die()
    {
        if (selected)
        {
            DeselectUnit();
        }
        Destroy(gameObject);
    }

    protected virtual void UpdateHealthBar()
    {
        if (!healthbar) return;
        Transform fill = healthbar.transform.Find("Bar");
        fill.GetComponent<UnityEngine.UI.Image>().fillAmount = Unit.CurrentHP / (float)Unit.MaxHP;
    }
}
