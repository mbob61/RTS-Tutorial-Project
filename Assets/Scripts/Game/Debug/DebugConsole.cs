using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DebugConsole : MonoBehaviour
{
    private bool showConsole = false;
    private string _consoleInput;

    enum DisplayType
    {
        None,
        Help,
        Autocomplete
    }

    private DisplayType displayType = DisplayType.None;

    private void Awake()
    {
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

        new DebugCommand("?", "Lists all available debug commands.", "?", () =>
        {
            displayType = DisplayType.Help;
        });
    }

    private void OnEnable()
    {
        EventManager.AddListener("<Input>ShowDebugConsole", OnShowDebugConsole);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("<Input>ShowDebugConsole", OnShowDebugConsole);
    }

    private void OnShowDebugConsole()
    {
        showConsole = true;
    }

    private void OnGUI()
    {
        if (showConsole)
        {
            // add fake boxes in the background to increase the opacity
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "");

            // show main input field
            string newInput = GUI.TextField(new Rect(0, 0, Screen.width, 24), _consoleInput);

            // show log area
            float y = 24;
            GUI.Box(new Rect(0, y, Screen.width, Screen.height - 24), "");
            if (displayType == DisplayType.Help)
                ShowHelp(y);
            else if (displayType == DisplayType.Autocomplete)
                ShowAutocomplete(y, newInput);

            // reset display state to "none" if input changes
            if (displayType != DisplayType.None && _consoleInput.Length != newInput.Length)
                displayType = DisplayType.None;

            // update input variable
            _consoleInput = newInput;

            // check for special keys
            Event e = Event.current;
            if (e.isKey)
            {
                if (e.keyCode == KeyCode.Return && _consoleInput.Length > 0)
                {
                    _OnReturn();
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    showConsole = false;
                }
                else if (e.keyCode == KeyCode.Tab)
                {
                    displayType = DisplayType.Autocomplete;
                }
            }
        }
    }

    private void _OnReturn()
    {
        _HandleConsoleInput();
        _consoleInput = "";
    }

    private void _HandleConsoleInput()
    {
        // parse input
        string[] inputParts = _consoleInput.Split(" ");
        string mainKeyword = inputParts[0];
        // check against available commands
        DebugCommandBase command;
        if (DebugCommandBase.DebugCommands.TryGetValue(mainKeyword.ToLower(), out command))
        {
            // try to invoke command if it exists
            if (command is DebugCommand dc)
                dc.Invoke();
            else
            {
                if (inputParts.Length < 2)
                {
                    Debug.LogError("Missing parameter!");
                    return;
                }
                if (command is DebugCommand<int> dcInt)
                {
                    int i;
                    if (int.TryParse(inputParts[1], out i))
                        dcInt.Invoke(i);
                }
            }
        }
    }

    private void ShowHelp(float y)
    {
        foreach (DebugCommandBase command in DebugCommandBase.DebugCommands.Values)
        {
            GUI.Label(new Rect(2, y, Screen.width, 20), $"{command.Format} - {command.Description}");
            y += 16;
        }
    }

    private void ShowAutocomplete(float y, string newInput)
    {
        IEnumerable<string> autocompleteCommands =DebugCommandBase.DebugCommands.Keys
            .Where(k => k.StartsWith(newInput.ToLower()));

        foreach (string k in autocompleteCommands)
        {
            DebugCommandBase c = DebugCommandBase.DebugCommands[k];
            GUI.Label(new Rect(2, y, Screen.width, 20), $"{c.Format} - {c.Description}");
            y += 16;
        }
    }
}