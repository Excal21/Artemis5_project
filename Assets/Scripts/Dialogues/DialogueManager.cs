using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [Header("DialogueLoader script")]
    [SerializeField] private DialogueLoader dialogueLoader;

    [Header("Opening")]
    [SerializeField] private GameObject openingPanel;
    [SerializeField] private GameObject openingTMPBackground;
    [SerializeField] private TextMeshProUGUI openingTMP;
    [SerializeField] private Image pressAnyKeyToContinueImage;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [Header("Centered dialogue box")]
    [SerializeField] private GameObject centerDialogueBox;
    [SerializeField] private TextMeshProUGUI centerMessageTMP;
    [Header("Left sided dialogue box")]
    [SerializeField] private GameObject leftDialogueBox;
    [SerializeField] private Image leftActorImage;
    [SerializeField] private TextMeshProUGUI leftActorNameTMP;
    [SerializeField] private TextMeshProUGUI leftMessageTMP;
    [Header("Right sided dialogue box")]
    [SerializeField] private GameObject rightDialogueBox;
    [SerializeField] private Image rightActorImage;
    [SerializeField] private TextMeshProUGUI rightActorNameTMP;
    [SerializeField] private TextMeshProUGUI rightMessageTMP;

    [Header("Buttons")]
    [SerializeField] GameObject buttonMainMenu;
    [SerializeField] GameObject buttonStartSector;

    [Header("EffectHandler script")]
    [SerializeField] private EffectHandler effectHandler;

    private List<Dialogue> dialogues;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool isDialogueFinished = false;

    private Vector2 originalOpeningTMPBackgroundSize;
    private Vector2 originalLeftDialogueBoxSize;
    private Vector2 originalRightDialogueBoxSize;
    private Vector2 originalCenterDialogueBoxSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (dialogueLoader == null)
        {
            Debug.LogError("Nem található a DialogueLoader script!");
            return;
        }

        /*
        if(openingPanel == null || openingTMPBackground == null || openingTMP == null || dialoguePanel == null || centerDialogueBox == null || centerMessageTMP == null || leftDialogueBox == null || leftActorImage == null || leftActorNameTMP == null || leftMessageTMP == null || rightDialogueBox == null || rightActorImage == null || rightActorNameTMP == null || rightMessageTMP == null || buttonMainMenu == null || buttonStartSector == null || pressAnyKeyToContinueImage == null)
        {
            Debug.LogError("Nem minden szükséges elem lett hozzárendelve a Unity Inspectorban!");
            return;
        }
        */

        dialogueLoader.LoadDialogueFromJSON();

        openingPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        buttonMainMenu.SetActive(false);
        buttonStartSector.SetActive(false);
        //pressAnyKeyToContinueImage.gameObject.SetActive(false);

        originalOpeningTMPBackgroundSize = openingTMPBackground.GetComponent<RectTransform>().sizeDelta;
        openingTMPBackground.GetComponent<RectTransform>().sizeDelta = new Vector2(originalOpeningTMPBackgroundSize.x, 0);
        openingTMPBackground.SetActive(true);

        originalLeftDialogueBoxSize = leftDialogueBox.GetComponent<RectTransform>().sizeDelta;
        leftDialogueBox.GetComponent<RectTransform>().sizeDelta = new Vector2(originalLeftDialogueBoxSize.x, 0);

        originalRightDialogueBoxSize = rightDialogueBox.GetComponent<RectTransform>().sizeDelta;
        rightDialogueBox.GetComponent<RectTransform>().sizeDelta = new Vector2(originalRightDialogueBoxSize.x, 0);
        
        originalCenterDialogueBoxSize = centerDialogueBox.GetComponent<RectTransform>().sizeDelta;
        centerDialogueBox.GetComponent<RectTransform>().sizeDelta = new Vector2(originalCenterDialogueBoxSize.x, 0);

        StartCoroutine(StartWithDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (isDialogueFinished) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (pressAnyKeyToContinueImage.gameObject.activeSelf)
            {
                pressAnyKeyToContinueImage.gameObject.SetActive(false);
                StartCoroutine(SwitchToDialoguePanel());
            }
            else
            {
                DisplayNextDialogue();
            }
        }
    }

    private IEnumerator StartWithDelay()
    {
        // A FadeIn miatt kell a késleltetés
        yield return new WaitForSeconds(1f);

        if(!(dialogueLoader.GetOpeningText() == ""))
        {
            StartCoroutine(ExpandOpeningBackground(() =>
            {
                StartCoroutine(TypeText(openingTMP, dialogueLoader.GetOpeningText(), () =>
                {
                    pressAnyKeyToContinueImage.gameObject.SetActive(true);
                }));
            }));
        }
        else
        {
            StartCoroutine(SwitchToDialoguePanel());
        }
    }

    private IEnumerator ExpandOpeningBackground(System.Action onComplete)
    {
        RectTransform openingBackgroundRectTransform = openingTMPBackground.GetComponent<RectTransform>();

        float elapsedTime = 0f;
        float duration = 0.25f; // default - 0.5f
        Vector2 originalSize = originalOpeningTMPBackgroundSize;

        while (elapsedTime < duration)
        {
            openingBackgroundRectTransform.sizeDelta = new Vector2(originalSize.x, Mathf.Lerp(0, originalSize.y, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        openingBackgroundRectTransform.sizeDelta = originalSize;
        onComplete?.Invoke();
    }

    private IEnumerator SwitchToDialoguePanel()
    {
        openingPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        StartCoroutine(ExpandDialogueBox(() =>
        {
            dialogues = dialogueLoader.GetDialogues();
            StartCoroutine(DisplayDialogue());
        }));
    }

    private IEnumerator TypeText(TextMeshProUGUI textComponent, string text, System.Action onComplete)
    {
        isTyping = true;
        textComponent.text = "";
        foreach (char letter in text.ToCharArray())
        {
            if (skipTyping)
            {
                textComponent.text = text;
                break;
            }
            textComponent.text += letter;
            yield return new WaitForSeconds(0.01f);
        }
        isTyping = false;
        skipTyping = false;
        onComplete?.Invoke();
    }

    private IEnumerator ExpandDialogueBox(System.Action onComplete)
    {
        RectTransform leftRectTransform = leftDialogueBox.GetComponent<RectTransform>();
        RectTransform rightRectTransform = rightDialogueBox.GetComponent<RectTransform>();
        RectTransform centerRectTransform = centerDialogueBox.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        float duration = 0.25f; // default - 0.5f

        while (elapsedTime < duration)
        {
            leftRectTransform.sizeDelta = new Vector2(originalLeftDialogueBoxSize.x, Mathf.Lerp(0, originalLeftDialogueBoxSize.y, elapsedTime / duration));
            rightRectTransform.sizeDelta = new Vector2(originalRightDialogueBoxSize.x, Mathf.Lerp(0, originalRightDialogueBoxSize.y, elapsedTime / duration));
            centerRectTransform.sizeDelta = new Vector2(originalCenterDialogueBoxSize.x, Mathf.Lerp(0, originalCenterDialogueBoxSize.y, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        leftRectTransform.sizeDelta = originalLeftDialogueBoxSize;
        rightRectTransform.sizeDelta = originalRightDialogueBoxSize;
        centerRectTransform.sizeDelta = originalCenterDialogueBoxSize;
        onComplete?.Invoke();
    }

    private IEnumerator DisplayDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            Dialogue dialogue = dialogues[currentDialogueIndex];

            if (dialogue.side == "left")
            {
                leftDialogueBox.SetActive(true);
                rightDialogueBox.SetActive(false);
                centerDialogueBox.SetActive(false);

                leftActorNameTMP.text = dialogue.characterName;
                leftActorImage.sprite = dialogueLoader.GetCharacterSprite(dialogue.characterAvatar);
                yield return StartCoroutine(TypeText(leftMessageTMP, dialogue.text, null));
            }
            else if (dialogue.side == "right")
            {
                leftDialogueBox.SetActive(false);
                rightDialogueBox.SetActive(true);
                centerDialogueBox.SetActive(false);

                rightActorNameTMP.text = dialogue.characterName;
                rightActorImage.sprite = dialogueLoader.GetCharacterSprite(dialogue.characterAvatar);
                yield return StartCoroutine(TypeText(rightMessageTMP, dialogue.text, null));
            }
            else if (dialogue.side == "center" || string.IsNullOrEmpty(dialogue.characterName))
            {
                leftDialogueBox.SetActive(false);
                rightDialogueBox.SetActive(false);
                centerDialogueBox.SetActive(true);

                yield return StartCoroutine(TypeText(centerMessageTMP, dialogue.text, null));
            }
        }
        else
        {
            isDialogueFinished = true;

            yield return new WaitForSeconds(1f);

            //Ha a jelenet neve Cutscene_Ending, akkor lassan legyen FadeOut, majd térjen vissza a MainMenu-be.
            if(SceneManager.GetActiveScene().name == "Cutscene_Ending")
            {
                effectHandler.StartFadeOutWithDurationAndLoadScene("MainMenu", 3.5f); // Lassabb fadeout effect
            }
            else
            {
                buttonMainMenu.SetActive(true);
                buttonStartSector.SetActive(true);
            }
        }
    }

    private void DisplayNextDialogue()
    {
        if (dialogues == null || currentDialogueIndex >= dialogues.Count)
        {
            return;
        }

        currentDialogueIndex++;
        StartCoroutine(DisplayDialogue());
    }
}
