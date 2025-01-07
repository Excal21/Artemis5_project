using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
//SaveManager.cs
[System.Serializable]
public class SaveData
{
    public int Level1;
    public int Level2;
    public int Level3;
}

public class SaveManager : MonoBehaviour
{
    private string saveFolderPath;
    private string saveFileName = "savegame.json";

    [Header("Continue Button")]
    [SerializeField] private Button buttonContinue;
    [Header("Star Map Buttons")]
    [SerializeField] private Button buttonLevel1;
    [SerializeField] private Button buttonLevel2;
    [SerializeField] private Button buttonLevel3;
    [Header("Save Warning GameObject")]
    [SerializeField] private GameObject savingNotification;
    [Header("Effect Handler")]
    [SerializeField] private EffectHandler effectHandler; // Hozzáadjuk az EffectHandler mezőt

    private bool allLevelsUnlocked = false;

    void Awake()
    {
        saveFolderPath = Path.Combine(Application.dataPath, "Savegame");
        #if UNITY_ANDROID
            saveFolderPath = Path.Combine(Application.persistentDataPath, "Savegame");
        #else
            saveFolderPath = Path.Combine(Application.dataPath, "Savegame");
        #endif

        //Ha MainMenu jelenetben vagyunk, akkor nézzük meg, hogy létezik-e a mappa.
        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);

                // Létrehozunk egy alapértelmezett mentést
                CreateDefaultSave();
            }
            else
            {
                // Ha létezik a mappa, akkor nézzük meg, hogy van-e mentésünk.
                CheckSaveFile();
            }
        }
    }

    private void CheckSaveFile()
    {
        string saveFilePath = Path.Combine(saveFolderPath, saveFileName);
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (IsValidSaveData(saveData))
            {
                SetLevelButtons(saveData);
                buttonContinue.interactable = saveData.Level2 == 1 || saveData.Level3 == 1;
                if (buttonContinue.interactable)
                {
                    buttonContinue.onClick.AddListener(() => ContinueGame(saveData));
                }
            }
            else
            {
                Debug.LogWarning("Érvénytelen mentés fájl.");
                CreateDefaultSave();
            }
        }
        else
        {
            Debug.LogWarning("Nem található mentés fájl. Az összes szint feloldva.");
            CreateDefaultSave();
            UnlockAllLevels();
        }
    }

    private void CreateDefaultSave()
    {
        SaveData saveData = new SaveData
        {
            Level1 = 1,
            Level2 = 0,
            Level3 = 0
        };

        string json = JsonUtility.ToJson(saveData);
        string saveFilePath = Path.Combine(saveFolderPath, saveFileName);

        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Alapértelmezett mentés létrehozása sikertelen: " + e.Message);
        }

        SetLevelButtons(saveData);
        buttonContinue.interactable = false;
    }

    private bool IsValidSaveData(SaveData saveData)
    {
        if (saveData.Level1 == 1 && saveData.Level2 == 0 && saveData.Level3 == 0)
            return true;
        if (saveData.Level1 == 1 && saveData.Level2 == 1 && saveData.Level3 == 0)
            return true;
        if (saveData.Level1 == 1 && saveData.Level2 == 1 && saveData.Level3 == 1)
            return true;
        return false;
    }

    private void SetLevelButtons(SaveData saveData)
    {
        buttonLevel1.interactable = saveData.Level1 == 1;
        buttonLevel2.interactable = saveData.Level2 == 1;
        buttonLevel3.interactable = saveData.Level3 == 1;
    }

    private void ContinueGame(SaveData saveData)
    {
        string levelToLoad = saveData.Level3 == 1 ? "Cutscene_Intro_Level3" :
                             saveData.Level2 == 1 ? "Cutscene_Intro_Level2" :
                             "Cutscene_Intro_Level1";
        effectHandler.StartFadeOutAndLoadScene(levelToLoad); // Közvetlenül használjuk az EffectHandler-t
    }

    public void SaveGame()
    {
        if (allLevelsUnlocked)
        {
            return; // Ha az összes szint fel van oldva, nem mentünk
        }
        StartCoroutine(SaveGameCoroutine());
    }

    private IEnumerator SaveGameCoroutine()
    {
        savingNotification.SetActive(true);

        SaveData saveData = LoadSaveData();

        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName == "Level1")
        {
            saveData.Level2 = 1;
        }
        else if (currentSceneName == "Level2")
        {
            saveData.Level3 = 1;
        }
        /*
        else if (currentSceneName == "Level3")
        {
            saveData.Level3 = 1;
        }
        */

        string json = JsonUtility.ToJson(saveData);
        string saveFilePath = Path.Combine(saveFolderPath, saveFileName);

        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            savingNotification.SetActive(false);
            Debug.LogError("Mentés sikertelen: " + e.Message);
        }

        yield return new WaitForSeconds(1f);

        savingNotification.SetActive(false);
        yield return null;
    }

    private SaveData LoadSaveData()
    {
        string saveFilePath = Path.Combine(saveFolderPath, saveFileName);
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();
    }

    public void NewGameStarted()
    {
        savingNotification.SetActive(true);

        SaveData saveData = new SaveData
        {
            Level1 = 1,
            Level2 = 0,
            Level3 = 0
        };

        string json = JsonUtility.ToJson(saveData);
        string saveFilePath = Path.Combine(saveFolderPath, saveFileName);

        try
        {
            File.WriteAllText(saveFilePath, json);
        }
        catch (System.Exception e)
        {
            savingNotification.SetActive(false);
            Debug.LogError("Mentés sikertelen: " + e.Message);
        }
        savingNotification.SetActive(false);

        buttonContinue.interactable = false;
    }

    private void UnlockAllLevels()
    {
        buttonLevel1.interactable = true;
        buttonLevel2.interactable = true;
        buttonLevel3.interactable = true;
    }
}
