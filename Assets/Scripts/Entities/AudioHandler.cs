using System.Collections;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance;

    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioClip shootSound;
    public AudioClip level1Music;
    public AudioClip level2Music;
    public AudioClip level3Music;
    public AudioClip mainMenuMusic;

    private float soundCooldown = 0.2f;
    private float lastPlayTime;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;

    public enum Music
    {
        MAINMENU,
        LEVEL1,
        LEVEL2,
        LEVEL3
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayShootSound()
    {
        if (Time.time >= lastPlayTime + soundCooldown)
        {
            audioSource.PlayOneShot(shootSound);
            lastPlayTime = Time.time;
        }
    }

    public void PlayMusic(Music music)
    {
        // Ha van aktív fade-out, állítsuk le
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
            fadeOutCoroutine = null;
        }

        switch (music)
        {
            case Music.LEVEL1:
                musicSource.clip = level1Music;
                break;
            case Music.LEVEL2:
                musicSource.clip = level2Music;
                break;
            case Music.MAINMENU:
                musicSource.clip = mainMenuMusic;
                break;
        }

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        float targetVolume = 1f; // Maximum hangerő
        musicSource.volume = 0;
        musicSource.loop = true;
        musicSource.Play();

        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeInCoroutine = StartCoroutine(FadeInMusic(5, targetVolume));
    }

    public void StopMusic()
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(FadeOutMusic(4)); // 4 másodperces fade-out
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    private IEnumerator FadeInMusic(float duration, float targetVolume)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume;
    }

    private IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop(); // Leállítjuk a lejátszást a fade-out végén
    }
}
