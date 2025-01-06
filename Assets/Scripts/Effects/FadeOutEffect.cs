using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FadeOutEffect : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.16f; // Fade időtartam másodpercben
    [SerializeField] private int steps = 20; // A fade lépéseinek száma
    private Image fadeImage;

    public event Action OnFadeComplete;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.enabled = false;
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void StartFadeOut(float? customFadeDuration = null)
    {
        fadeImage.enabled = true;
        StartCoroutine(FadeOut(customFadeDuration));
    }

    private IEnumerator FadeOut(float? customFadeDuration)
    {
        if (fadeImage == null)
        {
            Debug.LogError("FadeImage is null!");
            yield break;
        }

        float duration = customFadeDuration ?? fadeDuration;
        float elapsedTime = 0f;
        float stepDuration = duration / steps;

        for (int i = 0; i < steps; i++)
        {
            elapsedTime += stepDuration;
            float alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSecondsRealtime(stepDuration); //WaitForSeconds helyett WaitForSecondsRealtime kell, hogy a Time.timeScale ne befolyásolja
        }

        fadeImage.color = new Color(0, 0, 0, 1);
        OnFadeComplete?.Invoke();
    }
}