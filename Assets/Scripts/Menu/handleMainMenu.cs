using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandleMainMenu : MonoBehaviour
{
    [SerializeField] private List<GameObject> uIElements = new List<GameObject>();      // Reference to the array of UI elements in the menu
    
    [SerializeField] private TextMeshProUGUI buildNumberTMP = null;                     // Reference to the TextMeshProUGUI component that displays the build number
    
    private Color normalColor = Color.white;
    private Color clickedColor = new Color(128f / 255f, 0f / 255f, 255f / 255f);        // Lila
    private Color selectedColor = Color.yellow;
    private Color disabledColor = Color.gray;

    private GameObject lastSelectedObject;

    //Ha kell, akkor a jeleneten létre kell hozni egy TextMeshProUGUI objektumot, majd a Canvas-t kijelölve
    //a TextMeshProUGUI objektumot a debugOutput változóhoz hozzá kell rendelni.
    //Ezt majd publicra kell állítani, hogy az inspectorban látható legyen, illetve a null-t kell kitörölni.
    public TextMeshProUGUI debugOutput = null;

    private void SetBuildNumber()
    {
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
            buildNumberTMP.text = "(build -1)";
            Debug.LogError("Build number TMP is not assigned in the inspector.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetBuildNumber();

        //Find all UI elements in the scene and add them to the list.
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject go in allGameObjects)
        {
            if (go.CompareTag("Button") || go.CompareTag("Toggle") || go.CompareTag("Slider") || go.CompareTag("Dropdown"))
            {
                uIElements.Add(go);
            }
        }

        // Add event listeners to UI elements and set their colors if they are disabled.
        foreach (GameObject uIElement in uIElements)
        {
            AddEventTrigger(uIElement);
            SetDisabledColor(uIElement);
        }

        EnsureActivePanelSelection();
    }

    // Update is called once per frame
    void Update()
    {
        DEBUGcheckButtonsStates();

        //...mert EventTrigger nincs Enterre és NumPad Enterre.
        checkEnterPressed();

        checkMouseInput();

        // Update UIElements colors based on selection
        UpdateUIElementColors();
    }

    // Method to add event triggers to buttons
    void AddEventTrigger(GameObject uiElement)
    {
        if (uiElement != null)
        {
            EventTrigger trigger = uiElement.GetComponent<EventTrigger>() ?? uiElement.AddComponent<EventTrigger>();

            //Egérkurzor a játékobjektumon.
            // Pointer Enter (hover/select) - change to yellow
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;

            entryEnter.callback.AddListener((eventData) =>
            {
                switch (uiElement.tag)
                {
                    case "Button":
                        EventSystem.current.SetSelectedGameObject(uiElement);
                        SetButtonTextColor(uiElement.GetComponent<Button>(), selectedColor);
                        break;
                    case "Slider":
                        EventSystem.current.SetSelectedGameObject(uiElement);
                        SetSliderHandleColor(uiElement.GetComponent<Slider>(), selectedColor);
                        break;
                    case "Dropdown":
                        EventSystem.current.SetSelectedGameObject(uiElement);
                        SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), selectedColor);
                        break;
                    case "Toggle":
                        EventSystem.current.SetSelectedGameObject(uiElement);
                        SetToggleBackgroundColor(uiElement.GetComponent<Toggle>(), selectedColor);
                        break;
                    default:
                        break;
                }
            });
            trigger.triggers.Add(entryEnter);

            // Pointer Down (on click or hold) - change to purple and execute function
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerDown;

            entryClick.callback.AddListener((eventData) =>
            {
                
                switch (uiElement.tag)
                {
                    case "Button":
                        SetButtonTextColor(uiElement.GetComponent<Button>(), clickedColor);
                        ExecuteButtonClick(uiElement.GetComponent<Button>());
                        SetButtonTextColor(uiElement.GetComponent<Button>(), normalColor);
                        break;
                    case "Slider":
                        SetSliderHandleColor(uiElement.GetComponent<Slider>(), clickedColor);
                        ExecuteSliderClick(uiElement.GetComponent<Slider>());
                        SetSliderHandleColor(uiElement.GetComponent<Slider>(), normalColor);
                        break;
                    case "Dropdown":
                        SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), clickedColor);
                        ExecuteDropdownClick(uiElement.GetComponent<TMP_Dropdown>());
                        //SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), clickedColor);
                        break;
                    default:
                        break;
                }
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

    // Method to set button text color
    void SetButtonTextColor(Button button, Color color)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = color;
        }
    }

    // Method to set slider handle color
    void SetSliderHandleColor(Slider slider, Color selectedColor)
    {
        // Set the color of the slider handle
        if (slider != null)
        {
            Image handle = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
            if (handle != null)
            {
                handle.color = selectedColor;
            }
        }
    }
    
    // Method to set dropdown background color
    void SetDropdownBackgroundColor(TMP_Dropdown dropdown, Color selectedColor)
    {
        // Set the color of the dropdown background
        if (dropdown != null)
        {
            //Image background = dropdown.transform.Find("Template/Viewport/Content").GetComponent<Image>();
            Image background = dropdown.GetComponent<Image>();
            if (background != null)
            {
                background.color = selectedColor;
            }
            else
            {
                Debug.LogError("SetDropdownBackgroundColor has reference to a dropdown but it does not have a background! (null)");
            }
        }
        else
        {
            Debug.LogError("SetDropdownBackgroundColor received a null reference to a dropdown!");
        }
    }
    
    // Method to set toggle background color
    void SetToggleBackgroundColor(Toggle toggle, Color selectedColor)
    {
        // Set the color of the toggle background
        if (toggle != null)
        {
            Image background = toggle.transform.Find("Background").GetComponent<Image>();
            if (background != null)
            {
                background.color = selectedColor;
            }
        }
    }

    public void SetDisabledColor(GameObject uiElement)
    {
        // Ellenőrizzük, hogy a GameObject le van-e tiltva (enabled = false)
        var behaviour = uiElement.GetComponent<Behaviour>();
        switch (uiElement.tag)
        {
            case "Button":
                if(uiElement.GetComponent<Button>().enabled == false)
                    SetButtonTextColor(uiElement.GetComponent<Button>(), disabledColor);
                else
                    SetButtonTextColor(uiElement.GetComponent<Button>(), normalColor);
                break;
            case "Slider":
                if (uiElement.GetComponent<Slider>().enabled == false)
                    SetSliderHandleColor(uiElement.GetComponent<Slider>(), disabledColor);
                else
                    SetSliderHandleColor(uiElement.GetComponent<Slider>(), normalColor);
                break;
            case "Dropdown":
                if (uiElement.GetComponent<TMP_Dropdown>().enabled == false)
                    SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), disabledColor);
                else
                    SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), normalColor);
                break;
            case "Toggle":
                if (uiElement.GetComponent<Toggle>().enabled == false)
                    SetToggleBackgroundColor(uiElement.GetComponent<Toggle>(), disabledColor);
                else    
                    SetToggleBackgroundColor(uiElement.GetComponent<Toggle>(), normalColor);
                break;
            default:
                break;
        }
    }


    // Update colors based on selection
    void UpdateUIElementColors()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {

            var selectedGameObject = EventSystem.current.currentSelectedGameObject;
            
            // Ellenőrizzük, hogy van-e kiválasztott elem, és hogy az előzővel megegyezik-e
            if (selectedGameObject != lastSelectedObject)
            {
                // Előző kiválasztott elem színének visszaállítása
                if (lastSelectedObject != null)
                {
                    var lastGraphic = GetRelevantGraphic(lastSelectedObject);
                    if (lastGraphic != null)
                        lastGraphic.color = normalColor;
                }

                // Az aktuális kiválasztott elem színének beállítása
                var selectedGraphic = GetRelevantGraphic(selectedGameObject);
                if (selectedGraphic != null)
                {
                    selectedGraphic.color = selectedColor;
                    lastSelectedObject = selectedGameObject;
                }
            }
        }
    }

    // Megfelelő grafikai elem visszaadása
    Graphic GetRelevantGraphic(GameObject uiElement)
    {
        if (uiElement.TryGetComponent(out Button button))
            return button.GetComponentInChildren<TextMeshProUGUI>();

        if (uiElement.TryGetComponent(out Toggle toggle))
            return toggle.transform.Find("Background")?.GetComponent<Image>();

        if (uiElement.TryGetComponent(out Slider slider))
            return slider.transform.Find("Handle Slide Area/Handle")?.GetComponent<Image>();

        if (uiElement.TryGetComponent(out TMP_Dropdown dropdown))
            return dropdown?.GetComponent<Image>();

        return null;
    }



    void ExecuteButtonClick(Button button)
    {
        button.onClick.Invoke();  // Invokes the assigned click event
        EnsureActivePanelSelection();
    }

    void ExecuteSliderClick(Slider slider)
    {
        slider.onValueChanged.Invoke(slider.value);  // Invokes the assigned value change event
        EnsureActivePanelSelection();
    }

    void ExecuteDropdownClick(TMP_Dropdown dropdown)
    {
        Debug.Log("ExecuteDropdownClick");
        dropdown.onValueChanged.Invoke(dropdown.value);  // Invokes the assigned value change event
        EnsureActivePanelSelection();
    }

    void ExecuteToggleClick(Toggle toggle)
    {
        toggle.onValueChanged.Invoke(toggle.isOn);  // Invokes the assigned value change event
        EnsureActivePanelSelection();
    }

    // Ensure the active panel has a selected button
    void EnsureActivePanelSelection()
    {
        // Az aktív panel lekérdezése
        GameObject activePanel = GetActivePanel();
        if (activePanel != null)
        {
            // Kiválasztott UI elem keresése közvetlenül
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            // Ha nincs kijelölve elem, a panel első UI eleme legyen az aktív
            if (currentSelected == null || currentSelected.transform.IsChildOf(activePanel.transform) == false)
            {
                // Első UI elem megkeresése
                SelectFirstUIElement(activePanel);
            }
        }
        else
        {
            Debug.LogError("No active panel found!");
        }
    }

    // Az aktív panel első UI elemének kiválasztása
    void SelectFirstUIElement(GameObject panel)
    {
        // Próbálunk közvetlenül keresni egy gombot, toggle-t, slidert vagy dropdown-t
        Selectable firstElement = panel.GetComponentInChildren<Button>() as Selectable ??
                                panel.GetComponentInChildren<Toggle>() as Selectable ??
                                panel.GetComponentInChildren<Slider>() as Selectable ??
                                panel.GetComponentInChildren<TMP_Dropdown>() as Selectable;

        if (firstElement != null)
        {
            EventSystem.current.SetSelectedGameObject(firstElement.gameObject);
            SetButtonTextColor(firstElement.GetComponent<Button>(), selectedColor);
        }
        else
        {
            Debug.LogError("No selectable UI elements found in the active panel!");
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
            //?. - ha a currentSelectedGameObject null, akkor nem hajtódik végre a GetComponent<___>().
            EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null ||
            EventSystem.current.currentSelectedGameObject?.GetComponent<Slider>() == null ||
            EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_Dropdown>() == null
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
                GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                switch (currentSelectedGameObject.tag)
                {
                    case "Button":
                        ExecuteButtonClick(currentSelectedGameObject.GetComponent<Button>());
                        break;
                    case "Slider":
                        ExecuteSliderClick(currentSelectedGameObject.GetComponent<Slider>());
                        break;
                    case "Dropdown":
                        ExecuteDropdownClick(currentSelectedGameObject.GetComponent<TMP_Dropdown>());
                        break;
                    case "Toggle":
                        ExecuteToggleClick(currentSelectedGameObject.GetComponent<Toggle>());
                        break;
                    default:
                        EnsureActivePanelSelection();
                        break;
                }
            }
        }
    }

    public void exitGame()
    {
        Debug.Log("Exit Game!");
        Application.Quit();
    }

    //DEBUG!
    void DEBUGcheckButtonsStates()
    {
        if(debugOutput != null)
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

            /*
            void PrintGameObjectHierarchy(GameObject go, int indentLevel = 0)
            {
                // Az aktuális objektum neve és típusa
                string componentType = "GameObject";
                if (go.GetComponent<Toggle>() != null) componentType = "Toggle";
                else if (go.GetComponent<Slider>() != null) componentType = "Slider";
                else if (go.GetComponent<TMP_Dropdown>() != null) componentType = "TMP_Dropdown";
                else if (go.GetComponent<Image>() != null) componentType = "Image";
                else if (go.GetComponent<TextMeshProUGUI>() != null) componentType = "TextMeshProUGUI";

                // Kiírás a debugText változóba megfelelő tabulációval
                debugText += new string('\t', indentLevel) + go.name + " (" + componentType + ")\n";

                // Végigmegyünk az összes gyereken, és rekurzív módon meghívjuk a függvényt
                foreach (Transform child in go.transform)
                {
                    PrintGameObjectHierarchy(child.gameObject, indentLevel + 1);
                }
            }
            foreach (GameObject panel in gameObjectsPanels)
            {
                Transform[] panelTransforms = panel.GetComponentsInChildren<Transform>();
                if (panel.name.Contains("Options"))
                {
                    debugText += "\nOptionsPanel GameObjects: (" + panel.name + ")\n";
                    foreach (Transform trans in panelTransforms)
                    {
                        GameObject go = trans.gameObject;
                        
                        // Csak akkor írjuk ki, ha nincs TextMeshPro vagy Button komponens rajta
                        if (go.GetComponent<TextMeshProUGUI>() == null && go.GetComponent<Button>() == null)
                        {
                            PrintGameObjectHierarchy(go);
                        }
                    }
                    debugText += "\n\n\n\n\n";
                }
            }
            */

            foreach (GameObject panel in gameObjectsPanels)
            {            
                Button[] panelButtons = panel.GetComponentsInChildren<Button>();
                Toggle[] panelToggles = panel.GetComponentsInChildren<Toggle>();
                Slider[] panelSliders = panel.GetComponentsInChildren<Slider>();
                TMP_Dropdown[] panelDropdowns =panel.GetComponentsInChildren<TMP_Dropdown>();

                TextMeshProUGUI text;

                debugText += panel.name + ":\t" + (panel.activeInHierarchy? ColoredString("Active", Color.green) : ColoredString("Not Active", Color.blue)) + "\n";

                foreach (Button button in panelButtons)
                {
                    text = button.GetComponentInChildren<TextMeshProUGUI>();

                    debugText += "    " + button.name
                    + ":\t" + (button.enabled? ColoredString("Enabled", Color.green) : ColoredString("Disabled", Color.red))
                    + "\t"  + (EventSystem.current.currentSelectedGameObject == button.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                    + "\n    " + ColorToString(text.color) + "\n";
                }
                foreach (Toggle toggle in panelToggles)
                {
                    Transform backgroundTransform = toggle.transform.Find("Background");
                    if (backgroundTransform != null)
                    {
                        Image background = backgroundTransform.GetComponent<Image>();
                        if (background != null)
                        {
                            debugText += "    " + toggle.name
                                + ":\t" + (EventSystem.current.currentSelectedGameObject == toggle.gameObject ? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                                + "\n    " + ColorToString(background.color) + "\n";
                        }
                        else
                        {
                            debugText += "    " + toggle.name + ": " + ColoredString("background = null!", Color.red) + "\n";
                        }
                    }
                    else
                    {
                        debugText += "    " + toggle.name + ": " + ColoredString("BackgroundTransform = null!", Color.red) + "\n";
                    }
                }
                foreach (Slider slider in panelSliders)
                {
                    Image handle = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();

                    debugText += "    " + slider.name
                    + ":\t" + (EventSystem.current.currentSelectedGameObject == slider.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                    + "\n    " + ColorToString(handle.color) + "\n";
                }
                foreach (TMP_Dropdown dropdown in panelDropdowns)
                {
                    if (dropdown.transform.IsChildOf(panel.transform))
                    {
                        //Image background = dropdown.transform.Find("Template/Viewport/Content").GetComponent<Image>();
                        Image background = dropdown.GetComponent<Image>();

                        debugText += "    " + dropdown.name
                        + ":\t" + (EventSystem.current.currentSelectedGameObject == dropdown.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                        + "\n    " + ColorToString(background.color) + "\n";
                    }
                }

                debugText += "\n";
            }
            debugText += lastSelectedObject != null? "Last Selected GameObject: " + lastSelectedObject.name + "\n": "Last Selected Gameobject: null\n";
            debugOutput.text = debugText;
        }
    }

    private string ColoredString(string text, Color color)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{text}</color>";
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
}