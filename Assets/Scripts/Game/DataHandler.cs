using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public static void LoadGameData()
    {
        // Load the building data
        Globals.AVAILABLE_BUILDINGS_DATA = Resources.LoadAll<BuildingData>("ScriptableObjects/Units/Buildings");

        // load game parameters
        GameParameters[] gameParametersList = Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        foreach (GameParameters parameters in gameParametersList)
        {
            if (parameters is GamePlayerParameters pp)
                pp.LoadFromFile($"Games/{CoreDataHandler.instance.GameUID}/PlayerParameters");
            else
                parameters.LoadFromFile();
        }

        CharacterData[] characterData = Resources.LoadAll<CharacterData>("ScriptableObjects/Units/Character") as CharacterData[];
        foreach(CharacterData d in characterData)
        {
            Globals.CHARACTER_DATA[d.code] = d;
        }
        
    }

    public static void SaveGameData()
    {
        // save game parameters
        GameParameters[] gameParametersList = Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        foreach (GameParameters parameters in gameParametersList)
        {
            parameters.SaveToFile();
        }
        //ResourceData res = Resources.Load<ResourceData>("ScriptableObjects/Resources");
        //res.SaveToFile();
    }
}
