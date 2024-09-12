using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoreBooter : MonoBehaviour
{

    public static CoreBooter instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadMap("Map1");
    }

    public void LoadMap(string mapReference)
    {
        MapData data = Resources.Load<MapData>($"ScriptableObjects/Maps/{mapReference}");
        CoreDataHandler.instance.SetMapData(data);
        string s = data.sceneName;
        SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive).completed += (_) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(s));
            SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
        };
    }
}
