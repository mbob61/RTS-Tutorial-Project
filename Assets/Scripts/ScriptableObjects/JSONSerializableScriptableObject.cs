using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JSONSerializableScriptableObject : ScriptableObject
{
    #if UNITY_EDITOR
        private static string scriptableObjectsDataDirectory = "ScriptableObjects_Dev";
    #else
        private static string scriptableObjectsDataDirectory = "ScriptableObjects";
    #endif

    //private static string scriptsbleObjectsDataDirectory = "ScriptableObjects";

    public void SaveToFile()
    {
        string dirPath = Path.Combine(Application.persistentDataPath, scriptableObjectsDataDirectory);
        string filePath = Path.Combine(dirPath, $"{name}.json");

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Dispose();
        }

        string json = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, json);
    }

    public void LoadFromFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, scriptableObjectsDataDirectory, $"{name}.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning($"File \"{filePath}\" not found! Getting default values.", this);
            return;
        }

        string json = File.ReadAllText(filePath);
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
