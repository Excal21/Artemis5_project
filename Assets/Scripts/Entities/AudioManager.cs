using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;
    public AudioClip shootSound;

    private float soundCooldown = 0.2f; // Például fél másodperc
    private float lastPlayTime;

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
}
