using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    private bool _isDraggingMouseBox = false;
    private Vector3 _dragStartPosition = Vector3.zero;

    Ray ray;
    RaycastHit raycastHit;

    private void Update()
    {
        // Start creating the drag box
        if (Input.GetMouseButtonDown(0))
        {
            _isDraggingMouseBox = true;
            _dragStartPosition = Input.mousePosition;
        }
        
        // Finish creating the drag box
        if (Input.GetMouseButtonUp(0))
        {
            _isDraggingMouseBox = false;
        }

        // If the user is dragging and they aren't in the start position of the box
        if (_isDraggingMouseBox && Input.mousePosition != _dragStartPosition)
        {
            SelectUnitsInsideBox();
        }

        // If we have some units selected and we click the ground, deselect them all
        if (Globals.CURRENTLY_SELECTED_UNITS.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                DeselectAllUnits();
            }

            if (Input.GetMouseButtonDown(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, 1000f))
                {
                    if (raycastHit.transform.tag == "Terrain" && !HoldingShift())
                    {
                        DeselectAllUnits();
                    }
                }
            }
        }
    }

    private void SelectUnitsInsideBox()
    {
        // Create the bounds for  the selection box
        // Get all units with the "Unit" tag.
        // If the unit is inside the box, select it
        // If not, deselect it
        Bounds selectionBoxBounds = Utils.GetViewportBounds(Camera.main, _dragStartPosition, Input.mousePosition);
        GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
        bool unitInsideSelectionBox;

        foreach (GameObject unit in units)
        {
            unitInsideSelectionBox = selectionBoxBounds.Contains(Camera.main.WorldToViewportPoint(unit.transform.position));
            if (unitInsideSelectionBox)
            {
                unit.GetComponent<UnitManager>().SelectUnit(false, false);
            } else
            {
                if (!HoldingShift())
                {
                    unit.GetComponent<UnitManager>().DeselectUnit();
                }
            }
        }
    }

    private void DeselectAllUnits()
    {
        List<UnitManager> selectedUnits = new List<UnitManager>(Globals.CURRENTLY_SELECTED_UNITS);
        foreach(UnitManager unit in selectedUnits)
        {
            unit.DeselectUnit();
        }
    }

    private bool HoldingShift()
    {
        return Input.GetKey(KeyCode.LeftShift);
    }

    private void OnGUI()
    {
        if (_isDraggingMouseBox)
        {
            Rect rect = Utils.GetScreenRect(_dragStartPosition, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.5f, 1f, 0.4f, 0.2f));
            Utils.DrawScreenRectBorder(rect, 1, new Color(0.5f, 1f, 0.4f));
        }
    }
}
