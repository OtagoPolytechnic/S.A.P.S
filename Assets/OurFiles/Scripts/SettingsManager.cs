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
    }

    public void SaveSettings()
    {
        if (currentSettings == null) currentSettings = new SettingsData();

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

        string json = JsonUtility.ToJson(currentSettings, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Settings saved to " + savePath);
    }

    public void LoadSettings()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            currentSettings = JsonUtility.FromJson<SettingsData>(json);
            Debug.Log("Settings loaded from " + savePath);
        }
        else
        {
            currentSettings = new SettingsData
            {
                smoothTurning = true,
                snapAngle = 30f,
                snapDelay = 0.5f,
                smoothTurnSpeed = 90f
            };
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
        PlayerReferences.Instance.RightControllerInput.smoothTurnEnabled = currentSettings.smoothTurning;

        PlayerReferences.Instance.SnapTurn.delayTime = currentSettings.snapDelay;
        PlayerReferences.Instance.SnapTurn.turnAmount = currentSettings.snapAngle;

        PlayerReferences.Instance.SmoothTurn.turnSpeed = currentSettings.smoothTurnSpeed;
    }

}
