using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Parameters", menuName = "Scriptable Objects/Parameters")]
public class GameParameters : ScriptableObject
{
    public bool enableDayAndNightCycle;
    public float dayLengthInSeconds;
    public float dayInitialRatio;
}
