using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class DialogueLoader : MonoBehaviour
{
    [SerializeField] private string jsonFileName;
    private string openingText;
    private DialogueData dialogueData;
    private Dictionary<string, Sprite> characterSprites;

    private Sprite defaultSprite;

    void Start()
    {
        defaultSprite = Resources.Load<Sprite>("Textures/Others/black_box");
        if (defaultSprite == null)
        {
            Debug.LogError("Nem található a 'black_box.png' sprite a 'Resources/Textures/Others/' mappában!");
        }
    }

    public void LoadDialogueFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            dialogueData = JsonUtility.FromJson<DialogueData>(jsonContent);

            // Az openingText értékének beállítása
            openingText = dialogueData.opening;

            // Load character sprites
            LoadCharacterSprites();

            /*
            Debug.Log($"Opening text: {openingText}");
            Debug.Log("Dialógusok betöltve:");
            foreach (var dialogue in dialogueData.dialogues)
            {
                Debug.Log($"[{dialogue.characterName}] {dialogue.text}");
            }
            */
        }
        else
        {
            Debug.LogError($"Nem található a JSON fájl: {filePath}");
        }
    }

    private void LoadCharacterSprites()
    {
        characterSprites = new Dictionary<string, Sprite>();

        // Extract character names from dialogues
        HashSet<string> characterNames = new HashSet<string>();
        foreach (var dialogue in dialogueData.dialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.characterAvatar))
            {
                characterNames.Add(dialogue.characterAvatar);
            }
        }

        // Load sprites for the extracted character names
        foreach (string characterName in characterNames)
        {
            if (characterName == "none")
            {
                characterSprites.Add(characterName, defaultSprite);
            }
            else
            {
                Sprite sprite = Resources.Load<Sprite>($"Textures/Characters/{characterName}");
                if (sprite != null)
                {
                    characterSprites.Add(characterName, sprite);
                }
                else
                {
                    Debug.LogWarning($"Sprite not found for character: {characterName}");
                }
            }
        }
    }

    public Sprite GetDefaultSprite()
    {
        return defaultSprite;
    } 

    public Sprite GetCharacterSprite(string characterAvatar)
    {
        if (characterSprites.TryGetValue(characterAvatar, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogWarning($"Sprite not found for character: {characterAvatar} - Using default sprite instead.");
            
            return defaultSprite;
        }
    }

    public string GetOpeningText()
    {
        return openingText;
    }

    public List<Dialogue> GetDialogues()
    {
        return dialogueData?.dialogues;
    }
}
