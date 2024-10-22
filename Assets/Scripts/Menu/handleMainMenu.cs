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
    private Color highlightedColor = Color.yellow;
    private Color clickedColor = new Color(0.5f, 0, 0.5f);  // Lila
    private Color selectedColor = Color.yellow;

    private Button lastSelectedButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //An array of all the buttons with the tag 'button'
        GameObject[] taggedButtons = GameObject.FindGameObjectsWithTag("Button");
        for (int i = 0; i < taggedButtons.Length; i++)
        {
            // Adding the current 'taggedButtons' to the 'buttons' list
            buttons.Add(taggedButtons[i].GetComponent<Button>());
        }
        // Add event listeners to buttons
        foreach (Button button in buttons)
        {
            AddEventTrigger(button);
        }

        // Select the first button
        if (buttons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
            SetButtonColor(buttons[0], selectedColor);
            lastSelectedButton = buttons[0];
        }
        else
        {
            Debug.LogError("Buttons are not assigned in the inspector.");
        }

        // Ensure each panel has at least one selected button
        EnsurePanelSelection();

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
        // Check if the currently selected GameObject is null (e.g., when the player uses the mouse)
        if (EventSystem.current.currentSelectedGameObject == null && buttons != null && buttons.Count > 0)
        {
            // Select the first button again
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }

        // Ensure the active panel has a selected button
        EnsureActivePanelSelection();

        // Update button colors based on selection
        UpdateButtonColors();
    }

    // Method to add event triggers to buttons
    void AddEventTrigger(Button button)
    {
        if (button != null)
        {
            EventTrigger trigger = button.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

            // Pointer Enter (hover) - change to yellow
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;

            entryEnter.callback.AddListener((eventData) =>
            {
                SetButtonColor(button, highlightedColor);
                EventSystem.current.SetSelectedGameObject(button.gameObject);
            });
            trigger.triggers.Add(entryEnter);

            // Pointer Exit - change back to white
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) =>
            {
                SetButtonColor(button, normalColor);
            });
            trigger.triggers.Add(entryExit);

            // Pointer Down (on click or hold) - change to purple and execute function
            EventTrigger.Entry entryClick = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            entryClick.callback.AddListener((eventData) =>
            {
                SetButtonColor(button, clickedColor);
                ExecuteButtonClick(button);
                SetButtonColor(button, normalColor);
            });
            trigger.triggers.Add(entryClick);

            // Add PointerDown event to handle button press when held down
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((eventData) =>
            {
                SetButtonColor(button, clickedColor);
                ExecuteButtonClick(button);
                SetButtonColor(button, normalColor);
            });
            trigger.triggers.Add(entryDown);
        }
    }

    // Method to set button text color
    void SetButtonColor(Button button, Color color)
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
    }

    // Ensure each panel has at least one selected button
    void EnsurePanelSelection()
    {
        // Find all panels in the scene
        GameObject[] panels = GameObject.FindGameObjectsWithTag("Panel");
        foreach (GameObject panel in panels)
        {
            // Find all buttons in the current panel
            Button[] panelButtons = panel.GetComponentsInChildren<Button>();
            if (panelButtons.Length > 0)
            {
                // Select the first button in the panel
                EventSystem.current.SetSelectedGameObject(panelButtons[0].gameObject);
                SetButtonColor(panelButtons[0], selectedColor);
            }
        }
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
                    SetButtonColor(panelButtons[0], selectedColor);
                }
            }
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
                    SetButtonColor(lastSelectedButton, normalColor);
                }

                // Set the color of the currently selected button
                SetButtonColor(currentSelectedButton, selectedColor);
                lastSelectedButton = currentSelectedButton;
            }
        }
    }
    public void exitGame()
    {
        Debug.Log("Exit Game!");
        Application.Quit();
    }
}