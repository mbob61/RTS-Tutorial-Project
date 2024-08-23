using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Players Parameters", menuName = "Scriptable Objects/Game Players Parameters", order = 12)]
public class GamePlayerParameters : GameParameters
{
    public override string GetParametersName() => "Players";

    public PlayerData[] players;
    public int myPlayerId;
    
}
