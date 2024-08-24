using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    public static void LoadGameData()
    {
        // Load the building data
        Globals.AVAILABLE_BUILDINGS_DATA = Resources.LoadAll<BuildingData>("ScriptableObjects/Units/Buildings");

        // Load game parameters
        GameParameters[] gameParametersList = Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        foreach (GameParameters parameters in gameParametersList)
        {
            parameters.LoadFromFile();
        }

        //ResourceData resources = Resources.Load<ResourceData>("ScriptableObjects/Resources") as ResourceData;
        //foreach (ResourceValue r in resources.initialResources)
        //{
        //    Globals.AVAILABLE_RESOURCES.Add(r.code, new GameResource(r.code, r.amount));
        //}

        //// Load the resources
        //ResourceData resources = Resources.Load<ResourceData>("ScriptableObjects/Resources");
        //resources.LoadFromFile();

        //Dictionary<string, GameResource> tempResourcesDictionary = new Dictionary<string, GameResource>();
        //ResourceData loadfromobj = Resources.Load<ResourceData>("ScriptableObjects/Resources") as ResourceData;
        //foreach (ResourceValue r in loadfromobj.initialResources)
        //{
        //    tempResourcesDictionary.Add(r.code, new GameResource(r.code, r.amount));
        //}
        //Globals.AVAILABLE_RESOURCES = tempResourcesDictionary;
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
