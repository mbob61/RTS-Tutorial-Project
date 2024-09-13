using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
public class MainMenuManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject menuSceneSelectionPrefab;
    public GameObject playerPrefab;

    [Header("UI")]
    public Transform newGameScrollView;
    public Image newGameDetailMapCapture;
    public TextMeshProUGUI newGameDetailInfoText;
    public Transform newGamePlayersList;

    private MapData selectedMap;
    private Dictionary<int, PlayerData> playersData;
    private List<Color> availableColors;
    private List<bool> activePlayers;

    private static readonly Color[] playerColors = new Color[]
    {
        Color.red,
        Color.blue,
        Color.yellow,
        Color.green,
        Color.cyan,
        Color.magenta,
        Color.white,
        Color.gray,
    };

    private void Start()
    {
        PopulateMapsList();
    }

    #region New Game
    private void PopulateMapsList()
    {
        MapData[] maps = Resources.LoadAll<MapData>("ScriptableObjects/Maps");
        Transform t; Sprite s;
        foreach (MapData map in maps)
        {
            GameObject g = Instantiate(menuSceneSelectionPrefab, newGameScrollView);
            t = g.transform;
            s = Resources.Load<Sprite>($"MapCaptures/{map.sceneName}");
            //t.Find("Image").GetComponent<Image>().sprite = s;
            t.Find("Name").GetComponent<TextMeshProUGUI>().text = map.mapName;
            t.Find("Desc").GetComponent<TextMeshProUGUI>().text = $"{map.GetMapSizeType()} map\nmax {map.maxPlayers} players";
            //AddSceneSelectionListener(g.GetComponent<Button>(), map, s);
            AddSceneSelectionListener(g.GetComponent<Button>(), map);
        }
    }

    private void SelectMap(MapData map)
    {
        availableColors = new List<Color>(playerColors);
        selectedMap = map;
        string name;

        foreach (Transform child in newGamePlayersList)
        {
            Destroy(child.gameObject);
        }

        //newGameDetailMapCapture.sprite = mapSprite;
        newGameDetailInfoText.text = $"{map.mapName} <size=20>({map.mapSize}x{map.mapSize})</size>";
        playersData = new Dictionary<int, PlayerData>(map.maxPlayers);

        for (int i = 0; i < map.maxPlayers; i++)
        {
            name = i == 0 ? "Player" : $"Enemy {i}";
            playersData[i] = new PlayerData(name, playerColors[i]);

            Transform player = Instantiate(playerPrefab, newGamePlayersList).transform;

            player.Find("Name").GetComponent<TMP_InputField>().text = name;
            Image colorSprite = player.Find("Color").GetComponent<Image>();
            colorSprite.color = playersData[i].color;

            Transform colorsPicker = player.Find("Color/ColorPicker");
            Transform picker;

            player.Find("Color").GetComponent<Button>().onClick.AddListener(() =>
            {
                for (int j = 0; j < playerColors.Length; j++)
                {
                    picker = colorsPicker.GetChild(j);
                    picker.GetComponent<Button>().interactable = availableColors.Contains(playerColors[j]);
                }
                colorsPicker.gameObject.SetActive(true);
            });

            for (int j = 0; j < playerColors.Length; j++)
            {
                picker = colorsPicker.GetChild(j);
                picker.GetComponent<Image>().color = playerColors[j];
                AddScenePickPlayerColorListener(colorsPicker, colorSprite, picker.GetComponent<Button>(),i, j);
            }

            _AddScenePickPlayerInputListener(
                player.Find("Name").GetComponent<TMP_InputField>(),
                i);

            //activePlayers.Add(false);
        }
    }

    private void AddSceneSelectionListener(Button b, MapData map)
    {
        b.onClick.AddListener(() => SelectMap(map));
    }

    private void SetPlayerColor(
        Color color, int i, Image colorSprite, bool autoAdd = true)
    {
        if (autoAdd && playersData[i].color != null)
        {
            availableColors.Add(playersData[i].color);
        }
        availableColors.Remove(color);
        playersData[i].color = color;
        colorSprite.color = color;
    }

    private void _SetPlayerName(string value, int i)
    {
        playersData[i].name = value;
    }

    private void _AddScenePickPlayerInputListener(TMP_InputField f, int i)
    {
        f.onValueChanged.AddListener((string value) => _SetPlayerName(value, i));
    }

    private void TogglePlayer(Transform t, int i)
    {
        // ...
        // check for player colors duplicates
        Color c = playersData[i].color;
        if (isActiveAndEnabled)
        {
            if (availableColors.Contains(c))
            {
                availableColors.Remove(c);
            }
            else
            {
                SetPlayerColor(
                    availableColors[0], i,
                t.Find("Color/Content").GetComponent<Image>(),
                    false);
            }
        }
        else
        {
            if (!availableColors.Contains(c))
                availableColors.Add(c);
        }
    }

    private void AddScenePickPlayerColorListener(
        Transform colorsPicker, Image colorSprite,
        Button b, int i, int c)
    {
        b.onClick.AddListener(() => {
            SetPlayerColor(playerColors[c], i, colorSprite);
            colorsPicker.gameObject.SetActive(false);
        });
    }

    public void StartNewGame()
    {
        CoreDataHandler.instance.SetGameUID(selectedMap);

        // save player parameters for this map
        // from the menu setup
        GamePlayerParameters p = new GamePlayerParameters();
        p.players = playersData
            .Where((KeyValuePair<int, PlayerData> p) => activePlayers[p.Key])
            .Select((KeyValuePair<int, PlayerData> p) => p.Value)
            .ToArray();
        p.myPlayerId = 0;
        p.SaveToFile($"Games/{CoreDataHandler.instance.GameUID}/PlayerParameters", true);

        CoreBooter.instance.LoadMap(selectedMap.sceneName);
    }
    #endregion
}
