using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameParameters : BinarySerializableScriptableObject
{
    public abstract string GetParametersName();

    [SerializeField] protected List<string> fieldsToShowInGame;
    
    public bool ShowsField(string fieldName)
    {
        if (fieldsToShowInGame == null)
        {
            return false;
        }
        return fieldsToShowInGame.Contains(fieldName);
    }

    public void ToggleShowField(string fieldName)
    {
        if (fieldsToShowInGame == null)
        {
            fieldsToShowInGame = new List<string>();
        }

        if (ShowsField(fieldName))
        {
            fieldsToShowInGame.Remove(fieldName);
        } else
        {
            fieldsToShowInGame.Add(fieldName);
        }
    }

    public List<string> FieldsToShowInGame => fieldsToShowInGame;
}
