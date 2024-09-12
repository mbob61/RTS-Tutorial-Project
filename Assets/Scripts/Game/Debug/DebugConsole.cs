using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    enum DisplayType
    {
        History,
        Help,
        Autocomplete
    }
    enum LogType
    {
        Command,
        Output,
        Error,
    }
    private static Dictionary<LogType, Color> logColors = new Dictionary<LogType, Color>()
    {
        { LogType.Output, Color.cyan },
        { LogType.Error, Color.red },
    };

    struct HistoryItem
    {
        public string message;
        public LogType type;
    }

    private static GUIStyle _logStyle;

    private bool _showConsole = false;
    private string _consoleInput;

    private DisplayType _displayType;

    private const int MAX_HISTORY_LENGTH = 5;
    private int _historyCurrentIndex;
    private Queue<HistoryItem> _history = new Queue<HistoryItem>();
    private HistoryItem[] _browsableHistory
        => _history.Where(i => i.type == LogType.Command).ToArray();

    private void Awake()
    {
        new DebugCommand("?", "Lists all available debug commands.", "?", () =>
        {
            _displayType = DisplayType.Help;
        });

        new DebugCommand("toggle_fov", "Toggles the FOV parameter on/off", "toggle_fov", () =>
        {
            print("toggle fov acrivated");
            bool testingBool = !GameManager.instance.gameGlobalParameters.testingDebugConsole;
            GameManager.instance.gameGlobalParameters.testingDebugConsole = testingBool;
        });

        new DebugCommand<int>("add_gold", "Adds a given amount of gold to the current player.", "add_gold <amount>", (x) =>
        {
            Globals.AVAILABLE_RESOURCES[GameManager.instance.gamePlayersParameters.myPlayerId][InGameResource.Gold].AddAmount(x);
            EventManager.TriggerEvent("UpdateResourceTexts");
        });

        new DebugCommand<string, int>("instantiate_characters",
            "Instantiates multiple instances of a character unit(by reference code), using a Poisson disc sampling for random positioning.",
            "instantiate_characters <code> <amount>", (code, amount) =>
        {
               CharacterData d = Globals.CHARACTER_DATA[code];
               int owner = GameManager.instance.gamePlayersParameters.myPlayerId;
               List<Vector3> positions = Utils.SamplePositions(  amount, 1.5f, Vector2.one * 15, Utils.MiddleOfScreenPointToWorld());

               foreach(Vector3 pos in positions)
               {
                   Character c = new Character(d, owner);
                   c.ComputeProduction();
                   c.Transform.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(pos);
               }
        });

        new DebugCommand<int>(
          "set_unit_formation_type",
          "Sets the unit formation type (by index).",
          "set_unit_formation_type <formation_index>", (x) =>
          {
              Globals.UNIT_FORMATION_TYPE = (UnitFormationType)x;
              EventManager.TriggerEvent("UpdateUnitFormationType");
          });

        new DebugCommand<int>(
          "set_construction_hp",
          "Sets the selected unit construction hp.",
          "set_construction_hp <hp>", (x) =>
          {
              if (Globals.CURRENTLY_SELECTED_UNITS.Count == 0) return;
              Building b = (Building)Globals.CURRENTLY_SELECTED_UNITS[0].GetComponent<BuildingManager>().Unit;
              if (b == null) return;
              b.SetConstructionHP(x);
          });

        _displayType = DisplayType.History;
    }

    private void OnEnable()
    {
        EventManager.AddListener("<Input>ShowDebugConsole", _OnShowDebugConsole);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("<Input>ShowDebugConsole", _OnShowDebugConsole);
    }

    private void _OnShowDebugConsole()
    {
        _showConsole = true;
        EventManager.TriggerEvent("PausedGame");
    }

    private void OnGUI()
    {
        if (_logStyle == null)
        {
            _logStyle = new GUIStyle(GUI.skin.label);
            _logStyle.fontSize = 12;
        }

        if (_showConsole)
        {
            // add fake boxes in the background to increase the opacity
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            // show main input field
            string newInput = GUI.TextField(new Rect(0, 0, Screen.width, 24), _consoleInput);

            // show log area
            float y = 24;
            GUI.Box(new Rect(0, y, Screen.width, Screen.height - 24), "");
            if (_displayType == DisplayType.Help)
                _ShowHelp(y);
            else if (_displayType == DisplayType.Autocomplete)
                _ShowAutocomplete(y, newInput);
            else
                _ShowHistory(y);

            // reset display state to "history" if input changes
            if (_displayType != DisplayType.History && _consoleInput.Length != newInput.Length)
                _displayType = DisplayType.History;

            // update input variable
            _consoleInput = newInput;

            // check for special keys
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode == KeyCode.Tab)
                    _displayType = DisplayType.Autocomplete;
                else if (e.keyCode == KeyCode.Return && _consoleInput.Length > 0)
                    _OnReturn();
                else if (e.keyCode == KeyCode.Escape)
                {
                    _showConsole = false;
                    EventManager.TriggerEvent("ResumedGame");
                }
                else if (e.keyCode == KeyCode.UpArrow)
                {
                    _historyCurrentIndex--;
                    if (_historyCurrentIndex < 0)
                    {
                        _historyCurrentIndex = 0;
                    }

                    if (_browsableHistory.Length > 0)
                    {

                        _consoleInput = _browsableHistory[_historyCurrentIndex].message;
                    }
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    HistoryItem[] browsableHistory = _browsableHistory;
                    if (_historyCurrentIndex == browsableHistory.Length) return;

                    _historyCurrentIndex++;
                    if (_historyCurrentIndex == browsableHistory.Length)
                    {
                        _consoleInput = "";
                    }
                    else
                    {
                        if (browsableHistory.Length > 0)
                        {
                            _consoleInput = browsableHistory[_historyCurrentIndex].message;
                        }
                    }
                }
            }
        }
    }

    private void _ShowHelp(float y)
    {
        foreach (DebugCommandBase command in DebugCommandBase.DebugCommands.Values)
        {
            GUI.Label(
                new Rect(2, y, Screen.width, 20),
                $"{command.Format} - {command.Description}",
                _logStyle
            );
            y += 16;
        }
    }

    private void _ShowAutocomplete(float y, string newInput)
    {
        IEnumerable<string> autocompleteCommands =
                            DebugCommandBase.DebugCommands.Keys
                            .Where(k => k.StartsWith(newInput.ToLower()));
        foreach (string k in autocompleteCommands)
        {
            DebugCommandBase c = DebugCommandBase.DebugCommands[k];
            GUI.Label(
                new Rect(2, y, Screen.width, 20),
                $"{c.Format} - {c.Description}",
                _logStyle
            );
            y += 16;
        }
    }

    private void _ShowHistory(float y)
    {
        foreach (HistoryItem x in _history)
        {
            Color c = GUI.color;
            Color c2;
            if (logColors.TryGetValue(x.type, out c2))
                GUI.color = c2;
            GUI.Label(new Rect(2, y, Screen.width, 20), x.message, _logStyle);
            y += 16;
            GUI.color = c;
        }
    }

    private void _OnReturn()
    {
        _HandleConsoleInput();
        _consoleInput = "";
        _historyCurrentIndex = _browsableHistory.Length;
    }

    private void _HandleConsoleInput()
    {
        // parse input
        string[] inputParts = _consoleInput.Split(' ');
        string mainKeyword = inputParts[0];
        // check against available commands
        DebugCommandBase command;
        if (DebugCommandBase.DebugCommands.TryGetValue(mainKeyword.ToLower(), out command))
        {
            _LogMessage(_consoleInput);

            // try to invoke command if it exists
            if (command is DebugCommand dc)
                dc.Invoke();
            else
            {
                if (inputParts.Length < 2)
                {
                    _LogError("Missing parameter!");
                    return;
                }

                if (command is DebugCommand<string> dcString)
                {
                    dcString.Invoke(inputParts[1]);
                }
                else if (command is DebugCommand<int> dcInt)
                {
                    int i;
                    if (int.TryParse(inputParts[1], out i))
                        dcInt.Invoke(i);
                    else
                    {
                        _LogError($"'{command.Id}' requires an int parameter!");
                        return;
                    }
                }
                else if (command is DebugCommand<float> dcFloat)
                {
                    float f;
                    if (float.TryParse(inputParts[1], out f))
                        dcFloat.Invoke(f);
                    else
                    {
                        _LogError($"'{command.Id}' requires a float parameter!");
                        return;
                    }
                }
                else if (command is DebugCommand<string, int> dcStringInt)
                {
                    int i;
                    if (int.TryParse(inputParts[2], out i))
                        dcStringInt.Invoke(inputParts[1], i);
                    else
                    {
                        _LogError($"'{command.Id}' requires a string and an int parameter!");
                        return;
                    }
                }
            }
        }
        else
        {
            _LogError("Unkwown command!");
        }
    }

    private void _LogOutput(string msg)
    {
        _LogMessage($">> {msg}", LogType.Output);
    }

    private void _LogError(string msg)
    {
        _LogMessage(msg, LogType.Error);
    }

    private void _LogMessage(string msg, LogType t = LogType.Command)
    {
        _history.Enqueue(new HistoryItem { message = msg, type = t });
        if (_history.Count > MAX_HISTORY_LENGTH)
            _history.Dequeue();
    }
}