using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class loadPrefs : MonoBehaviour
{
    [SerializeField] private bool canUse = false;
    [SerializeField] private handleSettings settings;

    [SerializeField] private TMP_Text volumeText = null;
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private TMP_Dropdown resolutionDropdown = null;

    [SerializeField] private Toggle fullscreenToggle = null;

    [SerializeField] private Toggle vsyncToggle = null;

    private void Start()
    {
        if (canUse)
        {
            StartCoroutine(LoadSettingsWithDelay());
        }
    }

    private IEnumerator LoadSettingsWithDelay()
    {
        yield return new WaitUntil(() => resolutionDropdown.options.Count > 0);
        LoadSettings();
    }


    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            float localVolume = PlayerPrefs.GetFloat("masterVolume");

            volumeText.text = Mathf.RoundToInt(localVolume).ToString();
            volumeSlider.value = localVolume;
            AudioListener.volume = localVolume;
        }
        else
        {
            settings.setVolume(50);
        }

        if (PlayerPrefs.HasKey("fullscreen"))
        {
            bool localFullscreen = PlayerPrefs.GetInt("fullscreen") == 1;

            fullscreenToggle.isOn = localFullscreen;
            Screen.fullScreen = localFullscreen;
        }
        else
        {
            settings.setFullscreen(true);
        }

        if (PlayerPrefs.HasKey("vsync"))
        {
            bool localVsync = PlayerPrefs.GetInt("vsync") == 1;

            QualitySettings.vSyncCount = localVsync ? 1 : 0;
            vsyncToggle.isOn = localVsync;
        }
        else
        {
            settings.setVsync(true);
        }

        if (PlayerPrefs.HasKey("resolution"))
        {
            int localResolution = PlayerPrefs.GetInt("resolution");
            resolutionDropdown.value = localResolution < resolutionDropdown.options.Count ? localResolution : 0;

            resolutionDropdown.captionText.GetComponent<TMP_Text>().text = resolutionDropdown.options[resolutionDropdown.value].text;
        }
        else
        {
            settings.setResolution(0);
        }

        resolutionDropdown.RefreshShownValue();
    }
}