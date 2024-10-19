using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandleMainMenu : MonoBehaviour
{
    public Button newGameButton; // Reference to the "New Game" button
    public Button continueButton; // Reference to the "Continue" button
    public Button optionsButton; // Reference to the "Options" button
    public Button creditsButton; // Reference to the "Credits" button
    public Button quitButton; // Reference to the "Quit" button

    public Toggle wireframeToggle; // Reference to the wireframe toggle button
    public Text wireframeToggleLabel; // Reference to the label of the wireframe toggle button

    private bool isWireframeEnabled = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Select the "New Game" button when the game starts
        if (newGameButton != null)
        {
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        }
        else
        {
            Debug.LogError("New Game button is not assigned in the inspector.");
        }

        // Add event listeners to buttons
        AddEventTrigger(newGameButton);
        AddEventTrigger(continueButton);
        AddEventTrigger(optionsButton);
        AddEventTrigger(creditsButton);
        AddEventTrigger(quitButton);

        // Add event listener to the wireframe toggle
        if (wireframeToggle != null)
        {
            wireframeToggle.onValueChanged.AddListener(delegate { ToggleWireframeMode(wireframeToggle.isOn); });
        }
        else
        {
            Debug.LogError("Wireframe toggle is not assigned in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the currently selected GameObject is null (e.g., when the player uses the mouse)
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            // Select the "New Game" button again
            EventSystem.current.SetSelectedGameObject(newGameButton.gameObject);
        }
    }

    // Method to add event triggers to buttons
    void AddEventTrigger(Button button)
    {
        if (button != null)
        {
            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { EventSystem.current.SetSelectedGameObject(button.gameObject); });
            trigger.triggers.Add(entry);
        }
    }

    // Method to toggle wireframe mode
    void ToggleWireframeMode(bool isEnabled)
    {
        isWireframeEnabled = isEnabled;

        // Change the label text of the toggle button
        if (wireframeToggleLabel != null)
        {
            wireframeToggleLabel.text = isWireframeEnabled ? "DEBUG - Toggle wireframe OFF" : "DEBUG - Toggle wireframe ON";
        }

        // Toggle wireframe mode for all UI elements
        foreach (Graphic graphic in FindObjectsOfType<Graphic>())
        {
            graphic.material = isWireframeEnabled ? new Material(Shader.Find("UI/Default")) { color = Color.white } : null;
        }

        foreach (TextMeshProUGUI tmp in FindObjectsOfType<TextMeshProUGUI>())
        {
            tmp.fontMaterial = isWireframeEnabled ? new Material(Shader.Find("UI/Default")) { color = Color.white } : null;
        }
    }
}
