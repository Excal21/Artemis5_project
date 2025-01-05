using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System;

public class HandleSettings : MonoBehaviour
{
    [Header("Settings UI Elements")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI volumeText;
    [SerializeField] private Button applyButton;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle vsyncToggle;

    [Header("Resolution Dropdowns")]
    //FIXME: Lehetne privát és SerializeField?
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
        tempVolume = PlayerPrefs.GetFloat("masterVolume", 1f) * 100;
        tempIsFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        tempVsyncCount = PlayerPrefs.GetInt("vsync", 0);
        
        // Initialize volume
        volumeSlider.value = tempVolume;
        setVolume(tempVolume);

        // Initialize resolution dropdown
        //resolutions = Screen.resolutions;

        // Filter out duplicate resolutions
        HashSet<Resolution> uniqueResolutions = new HashSet<Resolution>(Screen.resolutions.Where(i => i.refreshRateRatio.value >= 59).ToHashSet());
        //resolutions = new Resolution[uniqueResolutions.Count];
        //resolutions.ToList().ForEach(i => uniqueResolutions.Add(i));
        
        //resolutions = new Resolution[uniqueResolutions.Count];
        resolutions = uniqueResolutions.ToArray<Resolution>();

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
           string option = string.Format("{0} x {1}", resolutions[i].width, resolutions[i].height);
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
        AudioListener.volume = tempVolume / 100; // Scale volume to 0-1
        volumeText.text = Mathf.RoundToInt(tempVolume).ToString();
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
        AudioListener.volume = tempVolume / 100;
        PlayerPrefs.SetFloat("masterVolume", tempVolume / 100);

        Resolution resolution = resolutions[tempResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, tempIsFullscreen);
        PlayerPrefs.SetInt("fullscreen", tempIsFullscreen ? 1 : 0);

        QualitySettings.vSyncCount = tempVsyncCount;
        PlayerPrefs.SetInt("vsync", tempVsyncCount);

        PlayerPrefs.SetInt("resolution", tempResolutionIndex);

        PlayerPrefs.Save();

        applyButton.interactable = false;
    }

    public void resetSettings()
    {
        tempVolume = 50f;
        tempResolutionIndex = resolutions.Length - 1; // (1920x1080 felbontás)
        tempIsFullscreen = true;
        tempVsyncCount = 0;

        volumeSlider.value = tempVolume;
        volumeText.text = Mathf.RoundToInt(tempVolume).ToString();

        resolutionDropdown.value = tempResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = tempIsFullscreen;
        vsyncToggle.isOn = tempVsyncCount == 1;

        AudioListener.volume = tempVolume / 100;
        Screen.SetResolution(resolutions[tempResolutionIndex].width, resolutions[tempResolutionIndex].height, tempIsFullscreen);
        QualitySettings.vSyncCount = tempVsyncCount;

        // PlayerPrefs mentése, hogy az értékek tartósak legyenek
        PlayerPrefs.SetFloat("masterVolume", tempVolume / 100);
        PlayerPrefs.SetInt("resolution", tempResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", tempIsFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("vsync", tempVsyncCount);
        PlayerPrefs.Save();

        // Apply gomb inaktiválása, mivel nincs alkalmazandó változtatás
        applyButton.interactable = false;
    }
}
