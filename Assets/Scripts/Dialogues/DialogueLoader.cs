using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

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

        StartCoroutine(LoadDialogueFromJSON());
    }

    public IEnumerator LoadDialogueFromJSON()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        string jsonContent;

        // Android esetén UnityWebRequest használata
        if (filePath.Contains("://") || filePath.Contains(":///"))
        {
            UnityWebRequest request = UnityWebRequest.Get(filePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Nem sikerült betölteni a JSON fájlt: {filePath}, Hiba: {request.error}");
                yield break;
            }

            jsonContent = request.downloadHandler.text;
        }
        else
        {
            // Más platformokon közvetlenül olvasható a fájl
            if (File.Exists(filePath))
            {
                jsonContent = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError($"Nem található a JSON fájl: {filePath}");
                yield break;
            }
        }

        // Parse JSON
        dialogueData = JsonUtility.FromJson<DialogueData>(jsonContent);

        // Az openingText értékének beállítása
        openingText = dialogueData.opening;

        // Load character sprites
        LoadCharacterSprites();

        //Debug.Log($"Opening text: {openingText}");
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
                    Debug.LogWarning($"Sprite not found for character: \"{theCharacterName}\" at path: {spritePath}");
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
