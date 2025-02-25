using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class HandleSettings : MonoBehaviour
{
    #region Változók
    [Header("Settings UI Elements")]
    [Header("Resolution Dropdowns")]
    [SerializeField] private TextMeshProUGUI    resolutionTextTitle;
    [SerializeField] private TMP_Dropdown       resolutionDropdown;
    [Header("Fullscreen")]
    [SerializeField] private TextMeshProUGUI    fullScreenTitle;
    [SerializeField] private Toggle             fullscreenToggle;
    [Header("Vsync")]
    [SerializeField] private TextMeshProUGUI    vsyncTitle;
    [SerializeField] private Toggle             vsyncToggle;
    [Header("Volume Settings")]
    [SerializeField] private TextMeshProUGUI    volumeTextTitle;
    [SerializeField] private Slider             volumeSlider;
    [SerializeField] private TextMeshProUGUI    volumeText;
    [Header("Controls Button")]
    [SerializeField] private Button             controlsButton;
    [Header("Legacy movement")]
    [SerializeField] private Toggle             legacyMovementToggle;
    [Header("Sensitivity Settings")]
    [SerializeField] private Slider             sensitivitySlider;
    [SerializeField] private TextMeshProUGUI    sensitivityText;
    [Header("Calibration Button")]
    [SerializeField] private Button             calibrationButton;
    [Header("Apply Buttons")]
    [SerializeField] private Button             applyButton;
    [SerializeField] private Button             applyControlsButton;
    
    /*
    [Header("Scene Manager")]
    [SerializeField] private HandleScenes       sceneManager;
    */

    private Resolution[] resolutions;

    // Temporary settings storage
    private float tempVolume;
    private int tempResolutionIndex;
    private bool tempIsFullscreen;
    private int tempVsyncCount;
    private bool tempLegacyMovement;
    private float tempSensitivity;
    private float tempCalibration;

    #endregion

    #region Awake és Start
    void Awake()
    {
        #if UNITY_ANDROID// || UNITY_EDITOR
            resolutionTextTitle.gameObject.SetActive(false);
            resolutionDropdown.gameObject.SetActive(false);
            vsyncTitle.gameObject.SetActive(false);
            vsyncToggle.gameObject.SetActive(false);
            controlsButton.gameObject.SetActive(true);

            fullScreenTitle.rectTransform.anchoredPosition = new Vector2(fullScreenTitle.rectTransform.anchoredPosition.x, 150);
            fullscreenToggle.GetComponent<RectTransform>().anchoredPosition = new Vector2(fullscreenToggle.GetComponent<RectTransform>().anchoredPosition.x, 150);

            volumeTextTitle.rectTransform.anchoredPosition = new Vector2(volumeTextTitle.rectTransform.anchoredPosition.x, 0);
            volumeSlider.GetComponent<RectTransform>().anchoredPosition = new Vector2(volumeSlider.GetComponent<RectTransform>().anchoredPosition.x, 0);
            // A volumeText-et nem kell átrendezni, mert az a volumeSlider gyereke, így azzal együtt mozog.            
        #endif
    }

    void Start()
    {
        applyButton.interactable = false;
		applyControlsButton.interactable = false;
        
        // Load initial settings - if they don't exist, set default values in second parameter
        tempVolume = PlayerPrefs.GetFloat("masterVolume", 1f) * 100;
        tempIsFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        tempVsyncCount = PlayerPrefs.GetInt("vsync", 0);
		
		#if UNITY_ANDROID
        tempLegacyMovement = PlayerPrefs.GetInt("legacymovement", 0) == 1;
        tempSensitivity = PlayerPrefs.GetFloat("sensitivity", 1.0f);
        tempCalibration = PlayerPrefs.GetFloat("calibration", -0.75f);
		#endif

        // Initialize volume
        volumeSlider.value = tempVolume;
        setVolume(tempVolume);

		#if UNITY_ANDROID
        // Initialize sensitivity
        sensitivitySlider.value = tempSensitivity;
        setSensitivity(tempSensitivity);

        legacyMovementToggle.isOn = tempLegacyMovement;
        setLegacyMovement(tempLegacyMovement);
		applyControlsButton.interactable = false;
		#endif

        // Initialize resolution dropdown
        //resolutions = Screen.resolutions;

        // Filter out duplicate resolutions
        HashSet<Resolution> uniqueResolutions = new HashSet<Resolution>(Screen.resolutions.Where(i => i.refreshRateRatio.value >= 59 && i.refreshRateRatio.value < 70).ToHashSet());
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

        //Player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    #endregion

    #region Setterek
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

    public void setSensitivity(float sensitivity)
    {
        tempSensitivity = Mathf.Round(sensitivity * 10) / 10;
        PlayerPrefs.SetFloat("sensitivity", tempSensitivity);
        sensitivityText.text = tempSensitivity.ToString(CultureInfo.InvariantCulture);
        setApplyControlsButton();
    }

    public void setCalibration(float VerticalCalibrationOffset)
    {
        tempCalibration = VerticalCalibrationOffset;
        PlayerPrefs.SetFloat("calibration", VerticalCalibrationOffset);

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                player.VerticalCalibrationOffset = tempCalibration;
            }
            else
            {
                Debug.LogError("Player GameObject found but Player Component not found!");
            }
        }
		/*
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
		*/
    }

    public void setLegacyMovement(bool isLegacyMovement)
    {
        tempLegacyMovement = isLegacyMovement;
        PlayerPrefs.SetInt("legacymovement", isLegacyMovement ? 1 : 0);

        // Disable sensitivity slider when legacy movement is enabled
        sensitivitySlider.interactable = !isLegacyMovement;
        calibrationButton.interactable = !isLegacyMovement;
        setApplyControlsButton();
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
	private void setApplyControlsButton()
    {
        applyControlsButton.interactable = true;
    }
    #endregion

    #region Egyéb metódusok
    public void Calibrate()
    {
        // Játékos által beállítható függőleges nyugalmi érték
        tempCalibration = Input.acceleration.y;
    }
    #endregion

    #region apply és reset
    public void applySettings()
    {
        AudioListener.volume = tempVolume / 100;
        PlayerPrefs.SetFloat("masterVolume", tempVolume / 100);

        #if !UNITY_ANDROID
        Resolution resolution = resolutions[tempResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, tempIsFullscreen);
        PlayerPrefs.SetInt("resolution", tempResolutionIndex);
        #endif

        PlayerPrefs.SetInt("fullscreen", tempIsFullscreen ? 1 : 0);

        QualitySettings.vSyncCount = tempVsyncCount;
        PlayerPrefs.SetInt("vsync", tempVsyncCount);

        PlayerPrefs.Save();

        applyButton.interactable = false;
    }
	
	public void applyControlsSettings()
	{
        PlayerPrefs.SetFloat("sensitivity", tempSensitivity);
        PlayerPrefs.SetInt("legacymovement", tempLegacyMovement ? 1 : 0);
		PlayerPrefs.SetFloat("calibration", tempCalibration);

        // Find the Player GameObject by its tag and set the sensitivity speed and calibration offset
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                player.SensitivitySpeed = sensitivitySlider.value;
                player.VerticalCalibrationOffset = tempCalibration;
                player.Legacymovement = tempLegacyMovement;
            }
            else
            {
                Debug.LogError("Player GameObject found but Player Component not found!");
            }
        }
		/*
        else
        {
            Debug.LogError("Player GameObject not found!");
        }
		*/
		
		PlayerPrefs.Save();
		
		applyControlsButton.interactable = false;
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

        #if !UNITY_ANDROID
        Screen.SetResolution(resolutions[tempResolutionIndex].width, resolutions[tempResolutionIndex].height, tempIsFullscreen);
        #endif

        QualitySettings.vSyncCount = tempVsyncCount;

        PlayerPrefs.SetFloat("masterVolume", tempVolume / 100);
        PlayerPrefs.SetInt("resolution", tempResolutionIndex);
        PlayerPrefs.SetInt("fullscreen", tempIsFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("vsync", tempVsyncCount);

        PlayerPrefs.Save();

        applyButton.interactable = false;
    }
	
	public void resetControlsSettings()
    {
        tempSensitivity = 1.0f;
		tempCalibration = -0.75f;
		tempLegacyMovement = false;
		
		sensitivitySlider.value = tempSensitivity;
        sensitivityText.text = tempSensitivity.ToString(CultureInfo.InvariantCulture);
        
        legacyMovementToggle.isOn = tempLegacyMovement;

        PlayerPrefs.SetFloat("sensitivity", tempSensitivity);               // Default sensitivity value
        PlayerPrefs.SetFloat("calibration", tempCalibration);               // Default calibration value
        PlayerPrefs.SetInt("legacymovement", tempLegacyMovement ? 1 : 0);   // Default legacy movement value

        // Find the Player GameObject by its tag and set the sensitivity speed and calibration offset
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            Player player = playerObject.GetComponent<Player>();
            if (player != null)
            {
                player.SensitivitySpeed = 1.0f;
                player.VerticalCalibrationOffset = -0.75f;
                player.Legacymovement = false;
            }
        }

        PlayerPrefs.Save();

        applyControlsButton.interactable = false;
    }
    #endregion
}