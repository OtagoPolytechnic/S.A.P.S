using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class SettingUI
{
    public string key;          // Name of setting 
    public Slider slider;       // reference to slider
    public Toggle toggle;       // reference to toggle 
}

[Serializable]
public class SettingsData
{
    public bool smoothTurning;
    public float snapAngle;
    public float snapDelay;
    public float smoothTurnSpeed;
}

public class SettingsManager : MonoBehaviour
{
    [Header("All setting UI elements")]
    public List<SettingUI> settingsUI = new List<SettingUI>();

    private string savePath;
    private SettingsData currentSettings;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "settings.json");
    }

    void Start()
    {
        LoadSettings();
        ApplySettingsToUI();
        ApplySettingsToPlayer();
        HookupUIEvents();
    }

    /// <summary>
    /// Saves current UI state to JSON file.
    /// </summary>
    public void SaveSettings()
    {
        if (currentSettings == null) currentSettings = GetDefaultSettings();

        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                switch (s.key)
                {
                    case "snapAngle": currentSettings.snapAngle = s.slider.value; break;
                    case "snapDelay": currentSettings.snapDelay = s.slider.value; break;
                    case "smoothTurnSpeed": currentSettings.smoothTurnSpeed = s.slider.value; break;
                }
            }
            else if (s.toggle != null)
            {
                if (s.key == "smoothTurning") currentSettings.smoothTurning = s.toggle.isOn;
            }
        }

        try
        {
            string json = JsonUtility.ToJson(currentSettings, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Settings saved to " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save settings: " + e.Message);
        }

        ApplySettingsToPlayer(); // Apply immediately after saving
    }

    /// <summary>
    /// Loads settings from file or falls back to defaults.
    /// </summary>
    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                currentSettings = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log("Settings loaded from " + savePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed to load settings, using defaults. " + e.Message);
                currentSettings = GetDefaultSettings();
            }
        }
        else
        {
            currentSettings = GetDefaultSettings();
            Debug.Log("No settings file found. Using defaults.");
        }
    }

    private void ApplySettingsToUI()
    {
        if (currentSettings == null) return;

        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                switch (s.key)
                {
                    case "snapAngle": s.slider.value = currentSettings.snapAngle; break;
                    case "snapDelay": s.slider.value = currentSettings.snapDelay; break;
                    case "smoothTurnSpeed": s.slider.value = currentSettings.smoothTurnSpeed; break;
                }
            }
            else if (s.toggle != null)
            {
                if (s.key == "smoothTurning") s.toggle.isOn = currentSettings.smoothTurning;
            }
        }
    }

    private void ApplySettingsToPlayer()
    {
        if (PlayerReferences.Instance == null)
        {
            Debug.LogWarning("PlayerReferences.Instance not found. Settings not applied.");
            return;
        }

        if (PlayerReferences.Instance.RightControllerInput != null)
            PlayerReferences.Instance.RightControllerInput.smoothTurnEnabled = currentSettings.smoothTurning;
        else
            Debug.LogWarning("RightControllerInput not assigned in PlayerReferences.");

        if (PlayerReferences.Instance.SnapTurn != null)
        {
            PlayerReferences.Instance.SnapTurn.delayTime = currentSettings.snapDelay;
            PlayerReferences.Instance.SnapTurn.turnAmount = currentSettings.snapAngle;
        }
        else
            Debug.LogWarning("SnapTurn not assigned in PlayerReferences.");

        if (PlayerReferences.Instance.SmoothTurn != null)
            PlayerReferences.Instance.SmoothTurn.turnSpeed = currentSettings.smoothTurnSpeed;
        else
            Debug.LogWarning("SmoothTurn not assigned in PlayerReferences.");
    }


    private SettingsData GetDefaultSettings()
    {
        return new SettingsData
        {
            smoothTurning = true,
            snapAngle = 30f,
            snapDelay = 0.5f,
            smoothTurnSpeed = 90f
        };
    }

    /// <summary>
    /// Resets everything back to defaults.
    /// </summary>
    public void ResetSettings()
    {
        currentSettings = GetDefaultSettings();
        ApplySettingsToUI();
        SaveSettings();
    }

    /// <summary>
    /// Hooks up sliders and toggles to auto-save on change.
    /// </summary>
    private void HookupUIEvents()
    {
        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                s.slider.onValueChanged.AddListener((_) => SaveSettings());
            }
            else if (s.toggle != null)
            {
                s.toggle.onValueChanged.AddListener((_) => SaveSettings());
            }
        }
    }
}
