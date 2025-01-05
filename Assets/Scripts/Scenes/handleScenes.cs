using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleScenes : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (IsSceneInBuildSettings(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene \"{sceneName}\" not found in build settings! Returning to Main Menu!");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadScene(int sceneIndex)
    {
        if (sceneIndex >= 0 && sceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError($"Scene {sceneIndex} index out of range! Returning to Main Menu!");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (scene == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
