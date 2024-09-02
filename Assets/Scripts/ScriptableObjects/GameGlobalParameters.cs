using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Global Parameters", menuName = "Scriptable Objects/Game Global Parameters", order = 10)]
public class GameGlobalParameters : GameParameters
{
    [Header("Day and Night")]
    public bool enableDayAndNightCycle;
    public float dayLengthInSeconds;
    public float dayInitialRatio;

    [Header("Initialization")]
    public BuildingData initialBuilding;

    [Header("Resource Production")]
    public int baseGoldProduction;
    public int bonusGoldProductionPerBuilding;
    public float goldBonusRange;
    public float woodProductionRange;
    public float stoneProductionRange;

    public AnimationCurve experienceEvolutionCurve;
    public AnimationCurve productionMultiplierCurve;
    public AnimationCurve attackDamageMultiplierCurve;
    public AnimationCurve attackRangeMultiplierCurve;

    public override string GetParametersName() => "Global";

    public delegate int ResourceProductionFunction(float distance);

    public bool testingDebugConsole = false;

    [HideInInspector]
    public ResourceProductionFunction woodProductionFunction = (float distance) =>
    {
        return Mathf.CeilToInt(10 * 1f / distance);
    };

    [HideInInspector]
    public ResourceProductionFunction stoneProductionFunction = (float distance) =>
    {
        return Mathf.CeilToInt(2 * 1f / distance);
    };

    public int UnitMaxLevel()
    {
        Keyframe[] keys = experienceEvolutionCurve.keys;
        return (int)keys.Select(k => k.time).Max();
    }

}
