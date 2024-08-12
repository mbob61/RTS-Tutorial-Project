using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class UnitManager : MonoBehaviour
{
    [SerializeField] private GameObject selectionIndicator;

    private void OnMouseDown()
    {
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
            Select();
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
        Globals.CURRENTLY_SELECTED_UNITS.Add(this);
        selectionIndicator.SetActive(true);
    }

    public void DeselectUnit()
    {
        // Do not deselect a unit if it is not selected
        if (!Globals.CURRENTLY_SELECTED_UNITS.Contains(this)) return;

        Globals.CURRENTLY_SELECTED_UNITS.Remove(this);
        selectionIndicator.SetActive(false);
    }

    protected virtual bool IsReadyForSelection()
    {
        return true;
    }
}
