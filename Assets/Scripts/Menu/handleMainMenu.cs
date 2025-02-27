using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class HandleMainMenu : MonoBehaviour
{
    #region Változók
    private List<GameObject> uIElements = new List<GameObject>();      // Reference to the array of UI elements in the menu
    
    private Color normalColor = Color.white;
    private Color clickedColor = new Color(128f / 255f, 0f / 255f, 255f / 255f);        // Lila
    private Color selectedColor = Color.yellow;
    private Color disabledColor = Color.gray;

    private GameObject currentSelectedPanel;
    private GameObject currentSelectedObject;
    private GameObject lastSelectedObject;
    private float originalBeepVolume;

    private bool isPointerDown = false;

    //Ha kell, akkor a jeleneten létre kell hozni egy TextMeshProUGUI objektumot, majd a Canvas-t kijelölve
    //a TextMeshProUGUI objektumot a debugOutput változóhoz hozzá kell rendelni.
    //Ezt majd publicra kell állítani, hogy az inspectorban látható legyen, illetve a null-t kell kitörölni.
    public TextMeshProUGUI debugOutput = null;
    #endregion

    #region Start, Awake és Update
    void Awake()
    {
		#if UNITY_ANDROID
        Application.targetFrameRate = 60;
		#endif
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalBeepVolume = AudioHandler.instance.BeepSource.volume;
        AudioHandler.instance.BeepSource.volume = 0;

        AudioHandler.instance.PlayMusic(AudioHandler.Music.MAINMENU);

        Time.timeScale = 1;

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
        //DEBUG_CheckUIElementsStates();

        //...mert EventTrigger nincs Enterre és NumPad Enterre.
        checkEnterPressed();

        //TMP_Dropdown esetén a nyilak használatakor a Scrollbar pozícióját frissíteni kell.
        checkArrowsInput();

        //Ha kattintásra nincs kiválasztva gomb, slider vagy dropdown, akkor legyen kiválasztva.
        checkMouseInput();

        // UIElemek színének frissítése a kiválasztás alapján
        UpdateUIElementColors();
    }
    #endregion

    #region Egér Események Kezelése
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
            EnsureActivePanelSelection();
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
    #endregion

    #region UI objektum végrehajtása
    // Általános eseménykezelő minden UI elemhez
    private void ExecuteElementEvent(GameObject uiElement)
    {
        if (uiElement.TryGetComponent(out Selectable selectable) && !selectable.interactable) return;

        if (uiElement.TryGetComponent(out Button button))
        {
            SetButtonTextColor(button, clickedColor);
            AudioHandler.instance.PlayMenuBeep();
            AudioHandler.instance.BeepSource.volume = originalBeepVolume;
            button.onClick.Invoke();
            SetButtonTextColor(button, normalColor);
        }
        else if (uiElement.TryGetComponent(out Slider slider))
        {
            isPointerDown = true;
            SetSliderHandleColor(slider, clickedColor);
            AudioHandler.instance.PlayMenuBeep();
            AudioHandler.instance.BeepSource.volume = originalBeepVolume;
            slider.onValueChanged.Invoke(slider.value);
            SetSliderHandleColor(slider, normalColor);
        }
        else if (uiElement.TryGetComponent(out TMP_Dropdown dropdown))
        {
            SetDropdownBackgroundColor(dropdown, clickedColor);
            dropdown.onValueChanged.AddListener((value) =>
            {
                SetDropdownBackgroundColor(dropdown, selectedColor);
                AudioHandler.instance.PlayMenuBeep();
                AudioHandler.instance.BeepSource.volume = originalBeepVolume;
                currentSelectedObject = dropdown.gameObject;
                EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
            });
            dropdown.onValueChanged.Invoke(dropdown.value);
        }
        else if (uiElement.TryGetComponent(out Toggle toggle))
        {
            if(uiElement.CompareTag("DropdownItem"))
            {
                // Dropdown Toggle
                SetDropdownToggleBackgroundColor(toggle, clickedColor);
                
                TMP_Dropdown tmp_dropdown = toggle.GetComponentInParent<TMP_Dropdown>();
                tmp_dropdown.onValueChanged.AddListener((value) =>
                {
                    SetDropdownBackgroundColor(tmp_dropdown, selectedColor);
                    currentSelectedObject = tmp_dropdown.gameObject;
                    EventSystem.current.SetSelectedGameObject(tmp_dropdown.gameObject);
                });
                tmp_dropdown.onValueChanged.Invoke(tmp_dropdown.value);

                SetDropdownToggleBackgroundColor(toggle, normalColor);
            }
            else
            {
                // Normál Toggle
                SetToggleBackgroundColor(toggle, clickedColor);
                AudioHandler.instance.PlayMenuBeep();
                AudioHandler.instance.BeepSource.volume = originalBeepVolume;
                toggle.onValueChanged.Invoke(toggle.isOn);
                SetToggleBackgroundColor(toggle, normalColor);
            }
        }
        else
        {
            Debug.LogError("Unsupported UI element on PointerDown.");
        }
        isPointerDown = false;
        /*
                else if (uiElement.GetComponentInChildren<TMP_Dropdown>() is TMP_Dropdown childDropdown)
                {
                    Debug.Log("Dropdown child clicked: " + childDropdown.name);
                    SetDropdownItemTextColor(childDropdown, clickedColor);
                    childDropdown.onValueChanged.Invoke(childDropdown.value);
                    SetDropdownItemTextColor(childDropdown, normalColor);
                }
        */
    }
    #endregion

    #region Bemenetek ellenőrzése
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
            // Ha nincs kiválasztott elem, biztosítjuk, hogy az aktív panel kiválasztásra kerüljön
            EnsureActivePanelSelection();
        }
    }

    private void checkArrowsInput()
    {
        if
        (
            Input.GetKeyDown(KeyCode.UpArrow)   || Input.GetKeyDown(KeyCode.DownArrow)  ||
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.PageUp)    || Input.GetKeyDown(KeyCode.PageDown)   ||
            Input.GetKey(KeyCode.UpArrow)       || Input.GetKey(KeyCode.DownArrow)      ||
            Input.GetKey(KeyCode.LeftArrow)     || Input.GetKey(KeyCode.RightArrow)     ||
            Input.GetKey(KeyCode.PageUp)        || Input.GetKey(KeyCode.PageDown)
        )
        {
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

            if (currentSelected != null && currentSelected.CompareTag("DropdownItem"))
            {
                AdjustDropdownScroll(currentSelected);
            }
        }
    }
    #endregion

    #region Legördülő menü kezelése
    private void AdjustDropdownScroll(GameObject selectedItem)
    {
        // Keressük meg a TMP_Dropdown komponenst a teljes szülői hierarchiában
        TMP_Dropdown dropdown = selectedItem.GetComponentInParent<TMP_Dropdown>();

        if (dropdown == null)
        {
            Debug.LogWarning("No dropdown found in the hierarchy of the selected item.");
            return;
        }

        // Ha van Scrollbar, beállítjuk a pozíciót
        Scrollbar currentScrollbar = dropdown.GetComponentInChildren<Scrollbar>();
        if (currentScrollbar != null)
        {
            int selectedIndex = selectedItem.transform.GetSiblingIndex();
            float step = 1f / (dropdown.options.Count - 1);
            currentScrollbar.value = 1 - (step * (selectedIndex-2));
        }
    }

/*
void AdjustDropdownScroll(TMP_Dropdown dropdown)
{
    // Keressük meg a Scrollbar komponenst a Dropdown hierarchy-jában
    Scrollbar scrollbar = dropdown.GetComponentInChildren<Scrollbar>();
    if (scrollbar == null)
    {
        Debug.LogWarning("No scrollbar found in dropdown.");
        return;
    }

    // Eseménykezelő a Dropdown kiválasztott indexének figyelésére
    dropdown.onValueChanged.AddListener((value) =>
    {
        int totalOptions = dropdown.options.Count;
        int visibleItems = Mathf.Min(4, totalOptions); // Példa szerint 4 látható elem egy időben
        float scrollValue = (float)value / (totalOptions - visibleItems);

        // Állítsuk be a scrollbar pozícióját
        scrollbar.value = 1 - scrollValue;
    });
}
*/
    #endregion

    #region Festések
    void SetButtonTextColor(Button button, Color color)
    {
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = color;
        }
    }

    void SetSliderHandleColor(Slider slider, Color selectedColor)
    {
        if (slider != null)
        {
            Image handle = slider.transform.Find("Handle Slide Area/Handle").GetComponent<Image>();
            if (handle != null)
            {
                handle.color = selectedColor;
            }
        }
    }
    
    void SetDropdownBackgroundColor(TMP_Dropdown dropdown, Color selectedColor)
    {
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
    
    void SetToggleBackgroundColor(Toggle toggle, Color selectedColor)
    {
        if (toggle != null)
        {
            Image background = toggle.transform.Find("Background").GetComponent<Image>();
            if (background != null)
            {
                background.color = selectedColor;
            }
        }
    }

    void SetDropdownToggleBackgroundColor(Toggle toggle, Color selectedColor)
    {
        if (toggle != null)
        {
            Image background = toggle.transform.Find("Item Background").GetComponent<Image>();
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

                    AudioHandler.instance.PlayMenuBeep();
                    AudioHandler.instance.BeepSource.volume = originalBeepVolume;
                }
            }
        }
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
    #endregion

    #region UI Elemek Kezelése
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
                    if (selectable.interactable && selectable.gameObject.activeInHierarchy)
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
    #endregion

    #region Kilépés
    public void exitGame()
    {
        Application.Quit();
    }
    #endregion

    #region FOR DEBUGGING!
    //FOR DEBUGGING!
    void DEBUG_CheckUIElementsStates()
    {
        if(debugOutput != null)
        {
            string debugText = "!DEBUG!:";

            //debugText += "\n";

            debugText += "Time.timeScale: " + (Time.timeScale == 0 ? ColoredString("0", Color.red) : ColoredString("1", Color.green)) + "\n";

            GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            List<GameObject> gameObjectsPanels = new List<GameObject>();

            foreach (GameObject go in allGameObjects)
            {
                if (go.CompareTag("Panel")  && !go.name.Contains("Confirm") && !go.name.Contains("Credits") && !go.name.Contains("Main") && !go.name.Contains("Star"))
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
    #endregion
}