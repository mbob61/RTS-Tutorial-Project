using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : UnitManager
{
    private Building building;
    public override Unit Unit
    {
        get { return building; }
        set { building = value is Building ? (Building)value : null; }
    }

    [SerializeField] private LayerMask terrainLayer;
    [SerializeField] private AudioSource ambientSource;
    private int numberOfCollisions = 0;

    private void Start()
    {
        //ambientSource.PlayOneShot(((BuildingData)building.Data).ambientSound);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain") return;
        numberOfCollisions++;
        CheckPlacementValidity();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain") return;
        numberOfCollisions--;
        CheckPlacementValidity();
    }

    public bool CheckPlacementValidity()
    {
        if (building == null || building.HasFixedPlacementStatus) return false;

        bool validPlacement = HasValidPlacement();
        if (!validPlacement)
        {
            building.SetMaterials(BuildingPlacementStatus.INVALID);
        } else
        {
            building.SetMaterials(BuildingPlacementStatus.VALID);
        }
        return validPlacement;
    }

    public bool HasValidPlacement()
    {
        if (numberOfCollisions > 0) return false;

        // get 4 bottom corner positions
        Vector3 p = transform.position;
        Vector3 c = boxCollider.center;
        Vector3 e = boxCollider.size / 2f;
        float bottomHeight = c.y - e.y + 0.5f;
        Vector3[] bottomCorners = new Vector3[]
        {
        new Vector3(c.x - e.x, bottomHeight, c.z - e.z),
        new Vector3(c.x - e.x, bottomHeight, c.z + e.z),
        new Vector3(c.x + e.x, bottomHeight, c.z - e.z),
        new Vector3(c.x + e.x, bottomHeight, c.z + e.z)
        };
        // cast a small ray beneath the corner to check for a close ground
        // (if at least two are not valid, then placement is invalid)
        int invalidCornersCount = 0;
        foreach (Vector3 corner in bottomCorners)
        {
            if (!Physics.Raycast(p + corner, Vector3.up * -1f, 2f, terrainLayer))
            {
                invalidCornersCount++;

            }
        }
        return invalidCornersCount < 3;
    }

    protected override bool IsReadyForSelection()
    {
        return building.HasFixedPlacementStatus;
    }

    //protected override void UpdateHealthBar()
    //{
    //    if (!healthbar) return;
    //    Transform fill = healthbar.transform.Find("Bar");

    //    // if in consutrction: show current construction ratio
    //    if (isActiveAndEnabled && !building.IsAlive)
    //    {
    //        fill.GetComponent<UnityEngine.UI.Image>().fillAmount = building.ConstructionRatio;
    //    }
    //    // else show health ratio as usual
    //    else
    //    {
    //        fill.GetComponent<UnityEngine.UI.Image>().fillAmount = Unit.CurrentHP / (float)Unit.MaxHP;
    //    }

    //}


    protected override void UpdateHealthBar()
    {
        if (!healthbar) return;
        Transform fill = healthbar.transform.Find("Bar");

        // if in construction: show current construction HP
        // else, show current health
        int hp = (isActiveAndEnabled && !building.IsAlive)
            ? building.ConstructionHP
            : Unit.MaxHP;
        fill.GetComponent<UnityEngine.UI.Image>().fillAmount = hp / (float)Unit.MaxHP;
    }

    public bool Build(int buildPower)
    {
        building.SetConstructionHP(building.ConstructionHP + buildPower);
        UpdateHealthBar();
        return building.IsAlive;
    }
}
