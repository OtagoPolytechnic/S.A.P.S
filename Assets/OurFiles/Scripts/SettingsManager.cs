using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.IO;

[Serializable]
public class SettingUI
{
    public string key;                 // Name of setting
    public Slider slider;              // Reference to slider
    public Toggle toggle;              // Reference to toggle
    public TextMeshProUGUI valueLabel; // Optional numeric value
   
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
    [Header("Setting Ranges")]
    private const float MinSnapAngle = 15f;
    private const float MaxSnapAngle = 180f;

    private const float MinSnapDelay = 0.05f;
    private const float MaxSnapDelay = 1.0f;

    private const float MinSmoothTurnSpeed = 30f;
    private const float MaxSmoothTurnSpeed = 360f;

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
        UpdateUIVisibility(); // initial visibility
    }

    public void SaveSettings()
    {
        if (currentSettings == null) currentSettings = GetDefaultSettings();

        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                if (s.key == "snapAngle")
                    currentSettings.snapAngle = Mathf.Clamp(s.slider.value, MinSnapAngle, MaxSnapAngle);

                if (s.key == "snapDelay")
                    currentSettings.snapDelay = Mathf.Clamp(s.slider.value, MinSnapDelay, MaxSnapDelay);

                if (s.key == "smoothTurnSpeed")
                    currentSettings.smoothTurnSpeed = Mathf.Clamp(s.slider.value, MinSmoothTurnSpeed, MaxSmoothTurnSpeed);

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

        ApplySettingsToPlayer();
       
    }

    private void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                currentSettings = JsonUtility.FromJson<SettingsData>(json);

                
                currentSettings.snapAngle = Mathf.Clamp(currentSettings.snapAngle, MinSnapAngle, MaxSnapAngle);
                currentSettings.snapDelay = Mathf.Clamp(currentSettings.snapDelay, MinSnapDelay, MaxSnapDelay);
                currentSettings.smoothTurnSpeed = Mathf.Clamp(currentSettings.smoothTurnSpeed, MinSmoothTurnSpeed, MaxSmoothTurnSpeed);

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


    public void ResetSettings()
    {
        currentSettings = GetDefaultSettings();
        ApplySettingsToUI();
        SaveSettings();
    }

    private SettingsData GetDefaultSettings()
    {
        return new SettingsData
        {
            smoothTurning = false,
            snapAngle = 100f,
            snapDelay = 0.2f,
            smoothTurnSpeed = 90f
        };
    }


    private void ApplySettingsToUI()
    {
        if (currentSettings == null) return;

        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                if (s.key == "snapAngle")
                {
                    s.slider.minValue = MinSnapAngle;
                    s.slider.maxValue = MaxSnapAngle;
                    s.slider.value = Mathf.Clamp(currentSettings.snapAngle, MinSnapAngle, MaxSnapAngle);
                }

                if (s.key == "snapDelay")
                {
                    s.slider.minValue = MinSnapDelay;
                    s.slider.maxValue = MaxSnapDelay;
                    s.slider.value = Mathf.Clamp(currentSettings.snapDelay, MinSnapDelay, MaxSnapDelay);
                }

                if (s.key == "smoothTurnSpeed")
                {
                    s.slider.minValue = MinSmoothTurnSpeed;
                    s.slider.maxValue = MaxSmoothTurnSpeed;
                    s.slider.value = Mathf.Clamp(currentSettings.smoothTurnSpeed, MinSmoothTurnSpeed, MaxSmoothTurnSpeed);
                }


                UpdateSliderLabel(s);
            }

            if (s.toggle != null)
            {
                if (s.key == "smoothTurning") s.toggle.isOn = currentSettings.smoothTurning;
            }
        }
    }

    private void ApplySettingsToPlayer()
    {
        if (currentSettings == null)
        {
            Debug.LogWarning("No settings data available. Settings not applied.");
            return;
        }

        if (PlayerReferences.Instance == null)
        {
            Debug.LogWarning("PlayerReferences.Instance not found. Settings not applied.");
            return;
        }

        //  Apply smooth turning
        if (PlayerReferences.Instance.RightControllerInput != null)
            PlayerReferences.Instance.RightControllerInput.smoothTurnEnabled = currentSettings.smoothTurning;
        else
            Debug.LogWarning("RightControllerInput is not assigned in PlayerReferences.");

        // Apply snap turn
        if (PlayerReferences.Instance.SnapTurn != null)
            PlayerReferences.Instance.SnapTurn.turnAmount = currentSettings.snapAngle;
            PlayerReferences.Instance.SnapTurn.debounceTime = currentSettings.snapDelay
        else
            Debug.LogWarning("SnapTurnProvider is not assigned in PlayerReferences.");

        //  Apply smooth turn speed
        if (PlayerReferences.Instance.SmoothTurn != null)
            PlayerReferences.Instance.SmoothTurn.turnSpeed = currentSettings.smoothTurnSpeed;
        else
            Debug.LogWarning("ContinuousTurnProvider is not assigned in PlayerReferences.");
    }



    private void HookupUIEvents()
    {
        foreach (var s in settingsUI)
        {
            if (s.slider != null)
            {
                s.slider.onValueChanged.AddListener((_) =>
                {
                    UpdateSliderLabel(s);
                    SaveSettings();
                });
            }
            else if (s.toggle != null)
            {
                s.toggle.onValueChanged.AddListener((_) =>
                {
                    SaveSettings();
                    UpdateUIVisibility();
                });
            }
        }
    }

    private void UpdateSliderLabel(SettingUI s)
    {
        if (s.valueLabel != null && s.slider != null)
            s.valueLabel.text = s.slider.value.ToString("F2");
    }

    private void UpdateUIVisibility()
    {
        bool smoothTurningOn = currentSettings != null && currentSettings.smoothTurning;

        foreach (var s in settingsUI)
        {
            switch (s.key)
            {
                case "smoothTurnSpeed":
                    // Show only if smooth turning is ON
                    bool showSmooth = smoothTurningOn;
                    if (s.slider != null)
                    {
                        s.slider.gameObject.SetActive(showSmooth);
                        s.slider.interactable = showSmooth;
                    }
                    if (s.valueLabel != null)
                    {
                        s.valueLabel.gameObject.SetActive(showSmooth);
                    }
                        break;

                case "snapAngle":
                case "snapDelay":
                    // Show only if smooth turning is OFF
                    bool showSnap = !smoothTurningOn;
                    if (s.slider != null)
                    {
                        s.slider.gameObject.SetActive(showSnap);
                        s.slider.interactable = showSnap;
                    }
                    if (s.valueLabel != null)
                    {
                        
                        s.valueLabel.gameObject.SetActive(showSnap);
                    }
                    break;
            }
        }
    }

}
