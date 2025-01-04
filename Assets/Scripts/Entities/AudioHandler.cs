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

    private float soundCooldown = 0.2f; // Például fél másodperc
    private float lastPlayTime;
    private Coroutine fadeInCoroutine;

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
        switch (music)
        {
            case Music.LEVEL1:
                musicSource.clip = level1Music;
                break;
 
            case Music.MAINMENU:
                musicSource.clip = mainMenuMusic;
                break;
        }

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        float targetVolume = musicSource.volume; // Eredeti hangerő az Inspectorban beállított érték
        musicSource.volume = 0; // Kezdő hangerő 0
        musicSource.loop = true;
        musicSource.Play();

        // Indítsd el a fade-in-t
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeInCoroutine = StartCoroutine(FadeInMusic(5, targetVolume));
    }
    public void StopMusic()
    {
        musicSource.Stop();
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

        musicSource.volume = targetVolume-0.2f; // Biztosítsd, hogy pontosan a célhangerőt érje el
    }


}
