using UnityEngine;
using System;

public class EffectHandler : MonoBehaviour
{
    [SerializeField] private FadeInEffect fadeInEffect;
    [SerializeField] private FadeOutEffect fadeOutEffect;
    [SerializeField] private HandleScenes handleScenes;

    void Start()
    {
        if (fadeInEffect == null)
        {
            Debug.LogError("FadeInEffect nincs hozzárendelve!");
            return;
        }

        if (fadeOutEffect == null)
        {
            Debug.LogError("FadeOutEffect nincs hozzárendelve!");
            return;
        }

        if (handleScenes == null)
        {
            Debug.LogError("HandleScenes nincs hozzárendelve!");
            return;
        }
        
        fadeInEffect.StartFadeIn();
    }

    public void StartFadeWithAction(Action onFadeComplete)
    {
        if (fadeOutEffect != null)
        {
            fadeOutEffect.OnFadeComplete += () =>
            {
                onFadeComplete?.Invoke();
                fadeOutEffect.OnFadeComplete -= onFadeComplete;
            };

            fadeOutEffect.StartFadeOut();
        }
        else
        {
            Debug.LogError("FadeOutEffect nincs hozzárendelve!");
        }
    }

    public void StartFadeOutAndLoadScene(string sceneName)
    {
        StartFadeWithAction(() =>
        {
            handleScenes.LoadScene(sceneName);
        });
    }

    public void StartFadeOutAndReloadCurrentScene()
    {
        StartFadeWithAction(() =>
        {
            handleScenes.ReloadCurrentScene();
        });
    }

    public void StartFadeOutAndQuit()
    {
        StartFadeWithAction(() =>
        {
            handleScenes.ExitGame();
        });
    }
}