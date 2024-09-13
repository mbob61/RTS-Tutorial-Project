using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreDataHandler : MonoBehaviour
{
    public static CoreDataHandler instance;
    private MapData mapData;
    private string gameUID;

    public string GameUID => gameUID;
    public string Scene => mapData != null ? mapData.sceneName : null;
    public float MapSize => mapData.mapSize;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void SetMapData(MapData d)
    {
        mapData = d;
    }

    public void SetGameUID(MapData d)
    {
        gameUID = $"{d.sceneName}__{System.Guid.NewGuid().ToString()}";
    }
}
