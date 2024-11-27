using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [Header("DialogueLoader script")]
    [SerializeField] private DialogueLoader dialogueLoader;

    [Header("Opening")]
    [SerializeField] private GameObject openingPanel;
    [SerializeField] private TextMeshProUGUI openingTMP;
    [SerializeField] private TextMeshProUGUI pressAnyKeyToContinueTMP;

    [Header("Dialogue")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Image actorImage;
    [SerializeField] private TextMeshProUGUI actorNameTMP;
    [SerializeField] private TextMeshProUGUI messageTMP;

    [Header("Buttons")]
    [SerializeField] GameObject buttonMainMenu;
    [SerializeField] GameObject buttonStartSector;

    private List<Dialogue> dialogues;
    private int currentDialogueIndex = 0;
    private bool isTyping = false;
    private bool skipTyping = false;
    private bool isDialogueFinished = false;

    private Vector2 originalDialogueBoxSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (dialogueLoader == null)
        {
            Debug.LogError("Nem található a DialogueLoader script!");
            return;
        }

        if(openingPanel == null || openingTMP == null || dialoguePanel == null || dialogueBox == null || actorImage == null || actorNameTMP == null || messageTMP == null)
        {
            Debug.LogError("Nem minden szükséges elem lett hozzárendelve a Unity Inspectorban!");
            return;
        }

        dialogueLoader.LoadDialogueFromJSON();

        openingPanel.SetActive(true);
        dialoguePanel.SetActive(false);
        buttonMainMenu.SetActive(false);
        buttonStartSector.SetActive(false);
        pressAnyKeyToContinueTMP.gameObject.SetActive(false);

        originalDialogueBoxSize = dialogueBox.GetComponent<RectTransform>().sizeDelta;
        dialogueBox.GetComponent<RectTransform>().sizeDelta = new Vector2(originalDialogueBoxSize.x, 0);

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
            else if (pressAnyKeyToContinueTMP.gameObject.activeSelf)
            {
                pressAnyKeyToContinueTMP.gameObject.SetActive(false);
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

        StartCoroutine(TypeText(openingTMP, dialogueLoader.GetOpeningText(), () =>
        {
            pressAnyKeyToContinueTMP.gameObject.SetActive(true);
        }));
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
        RectTransform rectTransform = dialogueBox.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        float duration = 0.25f; // default - 0.5f

        while (elapsedTime < duration)
        {
            rectTransform.sizeDelta = new Vector2(originalDialogueBoxSize.x, Mathf.Lerp(0, originalDialogueBoxSize.y, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rectTransform.sizeDelta = originalDialogueBoxSize;
        onComplete?.Invoke();
    }

    private IEnumerator DisplayDialogue()
    {
        if (currentDialogueIndex < dialogues.Count)
        {
            Dialogue dialogue = dialogues[currentDialogueIndex];

            actorNameTMP.text = dialogue.characterName;
            actorImage.sprite = dialogueLoader.GetCharacterSprite(dialogue.characterAvatar);

            yield return StartCoroutine(TypeText(messageTMP, dialogue.text, null));
        }
        else
        {
            isDialogueFinished = true;

            yield return new WaitForSeconds(1f);

            buttonMainMenu.SetActive(true);
            buttonStartSector.SetActive(true);
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
