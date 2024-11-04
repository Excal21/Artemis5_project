using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeInEffect : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.16f; // Fade időtartam másodpercben
    [SerializeField] private int steps = 20; // A fade lépéseinek száma
    private Image fadeImage;

    void Start()
    {
        fadeImage = GetComponent<Image>();
        
        fadeImage.color = new Color(0, 0, 0, 1);
        
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        float stepDuration = fadeDuration / steps;

        for (int i = 0; i < steps; i++)
        {
            elapsedTime += stepDuration;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return new WaitForSeconds(stepDuration); // Várakozás a következő lépésig
        }

        /*
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        */

        fadeImage.color = new Color(0, 0, 0, 0);

        fadeImage.enabled = false;
    }
}
