using System.Collections;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    public static AudioHandler instance;

    #region AudiHandler munkaváltozói
    private float soundCooldown = 0.2f;
    private float lastPlayTime;
    private float lastDialogBeepTime;
    private float lastMenuBeepTime;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;
    #endregion

    #region AudiHandler beállításai
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource beepSource;

    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip level1Music;
    [SerializeField] private AudioClip level2Music;
    [SerializeField] private AudioClip level3Music;
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip menuBeep;
    [SerializeField] private AudioClip dialogBeep;

    #endregion

    public AudioSource BeepSource { get => beepSource; }

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

    #region Rövid hangok metódusai
    public void PlayShootSound()
    {
        if (Time.time >= lastPlayTime + soundCooldown)
        {
            audioSource.PlayOneShot(shootSound);
            lastPlayTime = Time.time;
        }
    }

    public void PlayMenuBeep()
    {
        float menuBeepCooldown = 0.3f; // Egyedi időzítő a menuBeep-hez
        if (menuBeep != null && Time.time >= lastMenuBeepTime + menuBeepCooldown)
        {
            beepSource.PlayOneShot(menuBeep);
            lastMenuBeepTime = Time.time; // Frissítjük az utolsó menuBeep lejátszás idejét
        }
    }

    public void PlayDialogBeep()
    {
        if (dialogBeep != null && Time.time >= lastDialogBeepTime + 0.1f)
        {
            beepSource.PlayOneShot(dialogBeep);
            lastDialogBeepTime = Time.time;
        }
    }
    #endregion 

    #region Zenelejátszás metódusai
    public void PlayMusic(Music music)
    {
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
            case Music.LEVEL3:
                musicSource.clip = level3Music;
                break;
            case Music.MAINMENU:
                musicSource.clip = mainMenuMusic;
                break;
        }

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        float targetVolume = 1f; 
        musicSource.volume = 0;
        musicSource.loop = true;
        musicSource.Play();

        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
        }
        fadeInCoroutine = StartCoroutine(FadeInMusic(5, targetVolume));
    }

    public void StopMusic(float duration = 4)
    {
        if (fadeOutCoroutine != null)
        {
            StopCoroutine(fadeOutCoroutine);
        }
        fadeOutCoroutine = StartCoroutine(FadeOutMusic(duration));
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
        musicSource.Stop();
    }
    #endregion
}
