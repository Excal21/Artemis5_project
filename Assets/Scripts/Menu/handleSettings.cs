using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class handleSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Button applyButton;

    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    // Temporary settings storage
    private float tempVolume;
    private int tempResolutionIndex;
    private bool tempIsFullscreen;
    private int tempVsyncCount;

    void Start()
    {
        applyButton.interactable = false;
        
        // Load initial settings
        tempVolume = PlayerPrefs.GetFloat("masterVolume", 1f);
        tempIsFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        tempVsyncCount = PlayerPrefs.GetInt("vsync", 1);
        
        // Initialize volume
        volumeSlider.value = tempVolume;
        setVolume(tempVolume);

        // Initialize resolution dropdown
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        tempResolutionIndex = currentResolutionIndex;
        resolutionDropdown.value = tempResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Update fullscreen and vsync settings
        Screen.fullScreen = tempIsFullscreen;
        QualitySettings.vSyncCount = tempVsyncCount;
    }

    public void setResolution(int resolutionIndex)
    {
        tempResolutionIndex = resolutionIndex;
        setApplyButton();
    }

    public void setVolume(float volume)
    {
        tempVolume = volume;
        volumeText.text = Mathf.RoundToInt(volume).ToString();
        setApplyButton();
    }

    public void setFullscreen(bool isFullscreen)
    {
        tempIsFullscreen = isFullscreen;
        setApplyButton();
    }

    public void setVsync(bool isVsync)
    {
        tempVsyncCount = isVsync ? 1 : 0;
        setApplyButton();
    }

    private void setApplyButton()
    {
        applyButton.interactable = true;
    }

    public void applySettings()
    {
        AudioListener.volume = tempVolume;
        PlayerPrefs.SetFloat("masterVolume", tempVolume);

        Resolution resolution = resolutions[tempResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, tempIsFullscreen);
        PlayerPrefs.SetInt("fullscreen", tempIsFullscreen ? 1 : 0);

        QualitySettings.vSyncCount = tempVsyncCount;
        PlayerPrefs.SetInt("vsync", tempVsyncCount);

        PlayerPrefs.SetInt("resolution", tempResolutionIndex);

        PlayerPrefs.Save();

        applyButton.interactable = false;
    }
}
