using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandleMainMenu : MonoBehaviour
{
    private handleSettings settingsHandler;

    [SerializeField] private List<GameObject> uIElements = new List<GameObject>();      // Reference to the array of UI elements in the menu
    
    [SerializeField] private TextMeshProUGUI buildNumberTMP = null;                     // Reference to the TextMeshProUGUI component that displays the build number
    
    private Color normalColor = Color.white;
    private Color clickedColor = new Color(128f / 255f, 0f / 255f, 255f / 255f);        // Lila
    private Color selectedColor = Color.yellow;
    private Color disabledColor = Color.gray;

    private GameObject currentSelectedPanel;
    private GameObject currentSelectedObject;
    private GameObject lastSelectedObject;

    private bool isPointerDown = false;

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
            if (go.CompareTag("Button") || go.CompareTag("Toggle") || go.CompareTag("Slider") || go.CompareTag("Dropdown") || go.CompareTag("DropdownItem"))
            {
                uIElements.Add(go);
            }
        }

        // Add event listeners to UI elements and set their colors if they are disabled.
        foreach (GameObject uIElement in uIElements)
        {
            AddEventTrigger(uIElement);
        }

        //Induláskor legyen egy elem kiválasztva.
        EnsureActivePanelSelection();
    }

    // Update is called once per frame
    void Update()
    {
        //DEBUGcheckButtonsStates();

        //...mert EventTrigger nincs Enterre és NumPad Enterre.
        checkEnterPressed();

        //Ha kattintásra nincs kiválasztva gomb, slider vagy dropdown, akkor legyen kiválasztva.
        checkMouseInput();

        // UIElemek színének frissítése a kiválasztás alapján
        UpdateUIElementColors();
    }

    // Method to add event triggers to buttons
    private void AddEventTrigger(GameObject uiElement)
    {
        if (uiElement == null) return;

        EventTrigger trigger = uiElement.GetComponent<EventTrigger>() ?? uiElement.AddComponent<EventTrigger>();


        // Pointer Enter (hover/select) - change to yellow
        EventTrigger.Entry entryEnter = new EventTrigger.Entry();
        entryEnter.eventID = EventTriggerType.PointerEnter;

        entryEnter.callback.AddListener((eventData) => 
        {
            if (isPointerDown) return;
            HandlePointerEnter(uiElement);
        });
        trigger.triggers.Add(entryEnter);


        // Pointer Down (on click or hold) - change to purple and execute function
        EventTrigger.Entry entryClick = new EventTrigger.Entry();
        entryClick.eventID = EventTriggerType.PointerDown;

        entryClick.callback.AddListener((eventData) => 
        {
            isPointerDown = true;
            ExecuteElementEvent(uiElement);
        });
        trigger.triggers.Add(entryClick);


        // Pointer Up (release after drag) - reset to normal color
        EventTrigger.Entry entryRelease = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entryRelease.callback.AddListener((eventData) =>
        {
            isPointerDown = false;
            if(uiElement.TryGetComponent(out Slider slider))
            {
                SetSliderHandleColor(slider, normalColor);
            }
        });
        trigger.triggers.Add(entryRelease);

        /*
        // Add Enter key event listener
        EventTrigger.Entry entryEnterKey = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerClick // This is for the UI element click
        };
        entryEnterKey.callback.AddListener((eventData) => HandleElementClick(uiElement));
        trigger.triggers.Add(entryEnterKey);
        */
    }

    /*
    // Handles click events for the current selected UI element
    private void HandleElementClick(GameObject uiElement)
    {
        Debug.Log("HandleElementClick");
        if (uiElement.TryGetComponent(out Selectable selectable) && !selectable.interactable)
        {
            Debug.Log("HandleElementClick -> EnsureActivePanelSelection will be called.");
            EnsureActivePanelSelection();
            return;
        }

        Debug.Log("HandleElementClick -> ExecuteElementEvent will be called.");
        ExecuteElementEvent(uiElement);
    }
    */

    // Általános eseménykezelő minden UI elemhez
    private void ExecuteElementEvent(GameObject uiElement)
    {
        if (uiElement.TryGetComponent(out Selectable selectable) && !selectable.interactable) return;

        if (uiElement.TryGetComponent(out Button button))
        {
            SetButtonTextColor(button, clickedColor);
            button.onClick.Invoke();
            SetButtonTextColor(button, normalColor);
        }
        else if (uiElement.TryGetComponent(out Slider slider))
        {
            isPointerDown = true;
            SetSliderHandleColor(slider, clickedColor);
            slider.onValueChanged.Invoke(slider.value);
            SetSliderHandleColor(slider, normalColor);
        }
        else if (uiElement.TryGetComponent(out TMP_Dropdown dropdown))
        {
            SetDropdownBackgroundColor(dropdown, clickedColor);
            dropdown.onValueChanged.AddListener((value) =>
            {
                SetDropdownBackgroundColor(dropdown, selectedColor);
                currentSelectedObject = dropdown.gameObject;
                EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
            });
            dropdown.onValueChanged.Invoke(dropdown.value);
        }
        else if (uiElement.TryGetComponent(out Toggle toggle))
        {
            SetToggleBackgroundColor(toggle, clickedColor);
            toggle.onValueChanged.Invoke(toggle.isOn);
            SetToggleBackgroundColor(toggle, normalColor);
        }
        else
        {
            Debug.LogError("Unsupported UI element on PointerDown.");
        }
        isPointerDown = false;
    }

    // Kurzor belépése esemény, amely színt vált
    private void HandlePointerEnter(GameObject uiElement)
    {
        if (uiElement.TryGetComponent(out Selectable selectable) && !selectable.interactable) return;

        currentSelectedObject = uiElement;
        EventSystem.current.SetSelectedGameObject(uiElement);

        if (uiElement.TryGetComponent(out Button button))
        {
            SetButtonTextColor(button, selectedColor);
        }
        else if (uiElement.TryGetComponent(out Slider slider))
        {
            SetSliderHandleColor(slider, selectedColor);
        }
        else if (uiElement.TryGetComponent(out TMP_Dropdown dropdown))
        {
            SetDropdownBackgroundColor(dropdown, selectedColor);
        }
        else if (uiElement.TryGetComponent(out Toggle toggle))
        {
            SetToggleBackgroundColor(toggle, selectedColor);
        }
        else
        {
            Debug.LogError("Unsupported UI element on PointerEnter.");
        }
    }

    //Ha kattintásra nincs kiválasztva gomb, slider vagy dropdown, akkor legyen kiválasztva.
    private void checkMouseInput()
    {
        if
        (
            (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2)) &&
            //?. - ha a currentSelectedGameObject null, akkor nem hajtódik végre a GetComponent<___>().
            (EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null &&
            EventSystem.current.currentSelectedGameObject?.GetComponent<Slider>() == null &&
            EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_Dropdown>() == null)

            //&& EventSystem.current.currentSelectedGameObject?.GetComponent<Toggle>() == null)

            /*
            EventSystem.current.currentSelectedGameObject?.GetComponent<Button>() == null ||
            EventSystem.current.currentSelectedGameObject?.GetComponent<Slider>() == null ||
            EventSystem.current.currentSelectedGameObject?.GetComponent<TMP_Dropdown>() == null
            // || EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>() == null
            */

        )
        {
            EnsureActivePanelSelection();
        }
        
        // Check if no mouse buttons are pressed
        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
        {
            isPointerDown = false;
        }
    }

    private void checkEnterPressed()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;

            if (currentSelectedGameObject != null)
            {
                ExecuteElementEvent(currentSelectedGameObject);
            }
            else
            {
                // Ha nincs kiválasztott elem, biztosítjuk, hogy az aktív panel kiválasztásra kerüljön
                EnsureActivePanelSelection();
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

    public void SetDisabledColor(GameObject activePanel)
    {
        if (activePanel == null) return;

        // Ellenőrizzük, hogy a GameObject le van-e tiltva (enabled = false)
        foreach(Transform uiElementTransform in activePanel.GetComponentsInChildren<Transform>())
        {
            GameObject uiElement = uiElementTransform.gameObject;
            /*
            switch (uiElement.tag)
            {
                case "Button":
                    Debug.Log("======Button: " + uiElement.name);
                    if(uiElement.GetComponent<Button>().interactable == false)
                    {
                        SetButtonTextColor(uiElement.GetComponent<Button>(), disabledColor);
                    }
                    else if (uiElement.GetComponent<Button>() != currentSelectedGameObject)
                    {
                        SetButtonTextColor(uiElement.GetComponent<Button>(), normalColor);
                    }
                    break;
                case "Slider":
                    if (uiElement.GetComponent<Slider>().interactable == false)
                    {
                        SetSliderHandleColor(uiElement.GetComponent<Slider>(), disabledColor);
                    }
                    break;
                case "Dropdown":
                    if (uiElement.GetComponent<TMP_Dropdown>().interactable == false)
                    {
                        SetDropdownBackgroundColor(uiElement.GetComponent<TMP_Dropdown>(), disabledColor);
                    }
                    break;
                case "Toggle":
                    if (uiElement.GetComponent<Toggle>().interactable == false)
                    {
                        SetToggleBackgroundColor(uiElement.GetComponent<Toggle>(), disabledColor);
                    }
                    break;
                default:
                    break;
            }
            */
            Graphic relevantGraphic = GetRelevantGraphic(uiElement);
        
            if (relevantGraphic == null) continue;

            Selectable selectable = uiElement.GetComponent<Selectable>();
            
            if (selectable != null && !selectable.interactable)
            {
                relevantGraphic.color = disabledColor;
            }
            else if (uiElement == currentSelectedObject)
            {
                relevantGraphic.color = selectedColor;
            }
            else
            {
                relevantGraphic.color = normalColor;
            }
        }
    }


    // Update colors based on selection
    void UpdateUIElementColors()
    {
        currentSelectedPanel = GetActivePanel();
        SetDisabledColor(currentSelectedPanel);
        if (EventSystem.current.currentSelectedGameObject != null)
        {

            currentSelectedObject = EventSystem.current.currentSelectedGameObject;
            
            // Ellenőrizzük, hogy van-e kiválasztott elem, és hogy az előzővel megegyezik-e
            if (currentSelectedObject != lastSelectedObject)
            {
                // Előző kiválasztott elem színének visszaállítása
                if (lastSelectedObject != null)
                {
                    var lastGraphic = GetRelevantGraphic(lastSelectedObject);
                    if (lastGraphic != null)
                        lastGraphic.color = normalColor;
                }

                // Az aktuális kiválasztott elem színének beállítása
                var selectedGraphic = GetRelevantGraphic(currentSelectedObject);
                if (selectedGraphic != null)
                {
                    selectedGraphic.color = selectedColor;
                    lastSelectedObject = currentSelectedObject;
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

    // Ensure the active panel has a selected button
    void EnsureActivePanelSelection()
    {
        // Az aktív panel lekérdezése
        currentSelectedPanel = GetActivePanel();
        if (currentSelectedPanel != null)
        {
            // Kiválasztott UI elem lekérdezése
            currentSelectedObject = EventSystem.current.currentSelectedGameObject;

            // Ha nincs kijelölve elem, a panel első interaktív UI eleme legyen az aktív
            if (currentSelectedObject == null || !currentSelectedObject.transform.IsChildOf(currentSelectedPanel.transform))
            {
                // Keresés a panel gyermekeiben
                foreach (var selectable in currentSelectedPanel.GetComponentsInChildren<Selectable>(true))
                {
                    if (selectable.interactable)
                    {
                        // Kiválasztjuk az első interaktív elemet
                        EventSystem.current.SetSelectedGameObject(selectable.gameObject);
                        SetElementColor(selectable); // Az elem színének beállítása
                        return; // Kilépünk, ha megtaláltuk az első interaktív elemet
                    }
                }

                Debug.LogError("No selectable UI elements found in the active panel!");
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

    // Szín beállítása a kiválasztott UI elemhez
    void SetElementColor(Selectable element)
    {
        if (element is Button button)
        {
            SetButtonTextColor(button, selectedColor);
        }
        else if (element is Slider slider)
        {
            SetSliderHandleColor(slider, selectedColor);
        }
        else if (element is TMP_Dropdown dropdown)
        {
            SetDropdownBackgroundColor(dropdown, selectedColor);
        }
        else if (element is Toggle toggle)
        {
            SetToggleBackgroundColor(toggle, selectedColor);
        }
        else
        {
            Debug.LogError("Unsupported UI element type.");
        }
    }

    public void exitGame()
    {
        Debug.Log("Exit Game!");
        Application.Quit();
    }

    //FOR DEBUGGING!
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

            //FOR DEBUGGING!
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

                debugText += panel.name + ": " + (panel.activeInHierarchy? ColoredString("Active", Color.green) : ColoredString("Not Active", Color.blue)) + "\n";

                foreach (Button button in panelButtons)
                {
                    text = button.GetComponentInChildren<TextMeshProUGUI>();

                    debugText += "    " + button.name
                    + ": \t" + (button.interactable? ColoredString("Interactable", Color.green) : ColoredString("Not interactable", Color.red))
                    + " \t"  + (EventSystem.current.currentSelectedGameObject == button.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
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
                                + ": \t" + (EventSystem.current.currentSelectedGameObject == toggle.gameObject ? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
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
                    + ": \t" + (EventSystem.current.currentSelectedGameObject == slider.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                    + "\n    " + ColorToString(handle.color) + "\n";
                }
                foreach (TMP_Dropdown dropdown in panelDropdowns)
                {
                    if (dropdown.transform.IsChildOf(panel.transform))
                    {
                        //Image background = dropdown.transform.Find("Template/Viewport/Content").GetComponent<Image>();
                        Image background = dropdown.GetComponent<Image>();

                        debugText += "    " + dropdown.name
                        + ": \t" + (EventSystem.current.currentSelectedGameObject == dropdown.gameObject? ColoredString("Selected", Color.green) : ColoredString("Not Selected", Color.blue))
                        + "\n    " + ColorToString(background.color) + "\n";
                    }
                }

                debugText += "\n";
            }
            debugText += currentSelectedPanel  != null? "Current Selected Panel: "      + currentSelectedPanel.name  + "\n": "Current Selected Panel: null\n";
            debugText += currentSelectedObject != null? "Current Selected GameObject: " + currentSelectedObject.name + "\n": "Current Selected Gameobject: null\n";
            debugText += lastSelectedObject    != null? "Last Selected GameObject: "    + lastSelectedObject.name    + "\n": "Last Selected Gameobject: null\n";
            debugText += "ispointerDown: " + (isPointerDown ? ColoredString("Yes", Color.green) : ColoredString("No", Color.red)) + "\n";
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