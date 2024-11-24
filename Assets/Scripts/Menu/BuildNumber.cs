#if UNITY_EDITOR
    using UnityEditor;
#endif

using System;
using UnityEngine;
using TMPro;

public class BuildNumber : MonoBehaviour
{
    [Header("Build Number")]
    [SerializeField] private TextMeshProUGUI buildNumberTMP = null; // Reference to the TextMeshProUGUI component that displays the build number

    #if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void SetBuildNumber()
    {
        // Calculate the number of days since September 8, 2024
        DateTime startDate = new DateTime(2024, 9, 8);
        TimeSpan elapsedTime = DateTime.Now - startDate;
        int elapsedDays = (int)elapsedTime.TotalDays;

        // Set the build number in PlayerSettings
        PlayerSettings.bundleVersion = $"{elapsedDays}";
        Debug.Log($"Build number set to {elapsedDays}");
    }
    #endif

    private void Start()
    {
        // Display the build number at runtime
        #if UNITY_EDITOR
        string buildVersion = PlayerSettings.bundleVersion;
        #else
        string buildVersion = Application.version;
        #endif

        if (buildNumberTMP != null)
        {
            buildNumberTMP.text = $"(build {buildVersion})";
        }
        else
        {
            Debug.LogError("Build number TMP is not assigned in the inspector.");
        }
    }
}
