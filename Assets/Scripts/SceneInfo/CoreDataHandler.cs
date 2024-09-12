using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreDataHandler : MonoBehaviour
{
    public static CoreDataHandler instance;
    private MapData mapData;

    public string Scene => mapData.sceneName;
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
}
