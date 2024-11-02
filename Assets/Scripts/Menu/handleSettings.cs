using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class handleSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Button applyButton;

    void Start()
    {
        applyButton.interactable = false;
        
        setVolume(volumeSlider.value);
    }

    public void setVolume(float volume)
    {
        volumeText.text = Mathf.RoundToInt(volume).ToString();
    }

    public void setFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void setVsync(bool isVsync)
    {
        QualitySettings.vSyncCount = isVsync ? 1 : 0;
    }

    public void setApplyButton()
    {
        applyButton.interactable = true;
    }

    /*
    public void applySettings()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
        PlayerPrefs.SetInt("fullscreen", Screen.fullScreen ? 1 : 0);
        PlayerPrefs.SetInt("vsync", QualitySettings.vSyncCount);
        PlayerPrefs.Save();
    }
    */
}
