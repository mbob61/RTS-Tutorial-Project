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
        LoadMenu();
    }

    public void LoadMenu()
    {
        string prevScene = CoreDataHandler.instance.Scene;
        CoreDataHandler.instance.SetMapData(null);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive).completed += (_) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName("MainMenu"));

            if (prevScene != null)
            {
                Scene sc = SceneManager.GetSceneByName(prevScene);
                if (sc != null && sc.IsValid())
                    SceneManager.UnloadSceneAsync(sc);
            }
        };
    }

    public void LoadMap(string mapReference)
    {
        MapData d = Resources.Load<MapData>($"ScriptableObjects/Maps/{mapReference}");
        CoreDataHandler.instance.SetMapData(d);
        string s = d.sceneName;
        SceneManager.LoadSceneAsync(s, LoadSceneMode.Additive).completed += (_) =>
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(s));
            Scene sc = SceneManager.GetSceneByName("MainMenu");
            if (sc != null && sc.IsValid())
                SceneManager.UnloadSceneAsync(sc).completed += (_) =>
                {
                    SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
                };
        };
    }
}
