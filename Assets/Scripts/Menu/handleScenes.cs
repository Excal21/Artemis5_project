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
            Debug.LogError("Scene not found in build settings: " + sceneName);
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
            Debug.LogError("Scene index out of range: " + sceneIndex);
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
}
