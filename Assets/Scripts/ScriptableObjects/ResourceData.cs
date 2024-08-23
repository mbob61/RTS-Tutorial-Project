using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "Scriptable Objects/Resources", order = 5)]
public class ResourceData : JSONSerializableScriptableObject
{
    public List<ResourceValue> initialResources;

}
