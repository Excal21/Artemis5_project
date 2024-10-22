using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandleMainMenu : MonoBehaviour
{
    public List<Button> buttons;                            // Reference to the array of buttons in the main menu
    public TextMeshProUGUI buildNumberTMP;                  // Reference to the TextMeshProUGUI component that displays the build number
    
    private Color normalColor = Color.white;
    private Color clickedColor = new Color(128f / 255f, 0f / 255f, 255f / 255f);  // Lila
    private Color selectedColor = Color.yellow;

    private Button lastSelectedButton;

    public TextMeshProUGUI debugTextForButtons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Find all buttons in the scene and add them to the list.
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allGameObjects)
        {
            if (go.CompareTag("Button"))
            {
                buttons.Add(go.GetComponent<Button>());
            }
        }

        // Add event listeners to buttons.
        foreach (Button button in buttons)
        {
            AddEventTrigger(button);
        }

        EnsureActivePanelSelection();

        // Calculate the number of days since September 8, 2024
        DateTime startDate = new DateTime(2024, 9, 8);
        TimeSpan elapsedTime = DateTime.Now - startDate;
        int elapsedDays = (int)elapsedTime.TotalDays;

        // Set the build number text
        if (buildNumberTMP != null)
        {
            buildNumberTMP.text = $"(build {elapsedDays})";
        }
        else
        {
            Debug.LogError("Build number TMP is not assigned in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        DEBUGcheckButtonsStates();

        //...mert EventTrigger nincs Enterre és NumPad Enterre.
        checkEnterPressed();

        checkMouseInput();

        // Update button colors based on selection
        UpdateButtonColors();
    }

    // Method to add event triggers to buttons
    void AddEventTrigger(Button button)
    {
        if (button != null)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

            //Egérkurzor a gombon.
            // Pointer Enter (hover) - change to yellow
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;

            entryEnter.callback.AddListener((eventData) =>
            {
                EventSystem.current.SetSelectedGameObject(button.gameObject);
                SetButtonTextColor(button, selectedColor);
            });
            trigger.triggers.Add(entryEnter);

            // Pointer Down (on click or hold) - change to purple and execute function
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerDown;

            entryClick.callback.AddListener((eventData) =>
            {
                SetButtonTextColor(button, clickedColor);
                ExecuteButtonClick(button);
                SetButtonTextColor(button, normalColor);
            });
            trigger.triggers.Add(entryClick);

            /*
            //Egérkurzor lelép a gombról.
            // Pointer Exit - change back to white
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;

            entryExit.callback.AddListener((eventData) =>
            {
                Debug.Log("Pointer Exit");
            });
            trigger.triggers.Add(entryExit);
            */
        }
    }

    // Update button colors based on selection
    void UpdateButtonColors()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            Button currentSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            if (currentSelectedButton != null && currentSelectedButton != lastSelectedButton)
            {
                // Reset the color of the last selected button
                if (lastSelectedButton != null)
                {
                    SetButtonTextColor(lastSelectedButton, normalColor);
                }

                // Set the color of the currently selected button
                SetButtonTextColor(currentSelectedButton, selectedColor);
                lastSelectedButton = currentSelectedButton;
            }
        }
    }

    // Method to set button text color
    void SetButtonTextColor(Button button, Color color)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = color;
        }
    }

    void ExecuteButtonClick(Button button)
    {
        button.onClick.Invoke();  // Invokes the assigned click event
        EnsureActivePanelSelection();
    }

    void DEBUGcheckButtonsStates()
    {
        string debugText = "!DEBUG!:\n";

        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        List<GameObject> gameObjectsPanels = new List<GameObject>();

        foreach (GameObject go in allGameObjects)
        {
            if (go.CompareTag("Panel"))
            {
                gameObjectsPanels.Add(go);
            }
        }
        foreach (GameObject panel in gameObjectsPanels)
        {
            Button[] panelButtons = panel.GetComponentsInChildren<Button>();
            TextMeshProUGUI text;

            debugText += panel.name + ":\t" + (panel.activeInHierarchy? "Active" : "Not Active") + "\n";

            foreach (Button button in panelButtons)
            {
                text = button.GetComponentInChildren<TextMeshProUGUI>();

                debugText += "    " + button.name
                + ":\t" + (EventSystem.current.currentSelectedGameObject == button.gameObject? "Active" : "Not Active")
                + "\n    " + ColorToString(text.color) + "\n";
            }
        }
        debugText += lastSelectedButton != null? "Last Selected Button: " + lastSelectedButton.name + "\n": "Last Selected Button: null\n";
        debugTextForButtons.text = debugText;
    }

    private string ColorToString(Color color)
    {
        if (ColorUtility.ToHtmlStringRGB(color) == "FFFF00")
            return "<color=yellow>Citrom</color>";
        else if (color == Color.white)
            return "<color=white>Fehér</color>";
        else if (color == new Color(128f / 255f, 0f / 255f, 255f / 255f)) // Példa a lila színre (RGB kód)
            return "<color=#800080>Lila</color>"; // Hex kód formátumban
        else
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{ColorUtility.ToHtmlStringRGB(color)}</color>"; // Kódolt szín kiírása
    }

    // Ensure the active panel has a selected button
    void EnsureActivePanelSelection()
    {
        // Find the currently active panel
        GameObject activePanel = GetActivePanel();
        if (activePanel != null)
        {
            // Find all buttons in the active panel
            Button[] panelButtons = activePanel.GetComponentsInChildren<Button>();
            if (panelButtons.Length > 0)
            {
                // Check if any button is currently selected
                bool anyButtonSelected = false;
                foreach (Button button in panelButtons)
                {
                    if (EventSystem.current.currentSelectedGameObject == button.gameObject)
                    {
                        anyButtonSelected = true;
                        break;
                    }
                }

                // If no button is selected, select the first button
                if (!anyButtonSelected)
                {
                    EventSystem.current.SetSelectedGameObject(panelButtons[0].gameObject);
                    SetButtonTextColor(panelButtons[0], selectedColor);
                }
            }
        }
        else
        {
            Debug.LogError("No active panel found!");
        }
    }

    // Get the currently active panel
    GameObject GetActivePanel()
    {
        // Find all panels in the scene
        GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
        foreach (GameObject panel in panels)
        {
            if (panel.activeInHierarchy)
            {
                return panel;
            }
        }
        return null;
    }

    private void checkMouseInput()
    {
        if
        (
            (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) &&
            //?. - ha a currentSelectedGameObject null, akkor nem hajtódik végre a GetComponent<Button>().
            EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null
        )
        {
            EnsureActivePanelSelection();
        }
    }

    private void checkEnterPressed()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (EventSystem.current.currentSelectedGameObject != null)
            {
                Button currentSelectedButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                if (currentSelectedButton != null)
                {
                    ExecuteButtonClick(currentSelectedButton);
                }
                else
                {
                    EnsureActivePanelSelection();
                }
            }
        }
    }

    public void exitGame()
    {
        Debug.Log("Exit Game!");
        Application.Quit();
    }
}