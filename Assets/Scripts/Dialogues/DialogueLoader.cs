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
        HashSet<string> characterAvatars = new HashSet<string>();
        foreach (var dialogue in dialogueData.dialogues)
        {
            if (!string.IsNullOrEmpty(dialogue.characterAvatar))
            {
                characterAvatars.Add(dialogue.characterAvatar);
            }
        }

        // Load sprites for the extracted character names
        foreach (string characterName in characterAvatars)
        {
            if (characterName == "")
            {
                characterSprites.Add(characterName, defaultSprite);
            }
            else
            {
                // Remove the .png extension if it exists
                string theCharacterName = characterName.Replace(".png", "");
                string spritePath = $"Textures/Characters/{theCharacterName}";
                Sprite sprite = Resources.Load<Sprite>(spritePath);
                if (sprite != null)
                {
                    characterSprites.Add(theCharacterName, sprite);
                }
                else
                {
                    Debug.LogWarning($"DialogueLoader -> LoadCharacterSprites() -> Sprite not found for character: \"{theCharacterName}\" at path: {spritePath}");
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
        string theCharacterName = characterAvatar.Replace(".png", "");
        if (characterSprites.TryGetValue(theCharacterName, out Sprite sprite))
        {
            return sprite;
        }
        else
        {
            Debug.LogWarning($"GetDefaultSprite() -> Sprite not found for character: {theCharacterName} - Using default sprite instead.");
            
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
