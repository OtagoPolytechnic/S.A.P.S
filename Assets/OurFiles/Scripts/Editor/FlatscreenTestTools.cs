using UnityEditor;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

/// <summary>
/// Used to test XR gameplay without requiring a headset
/// </summary>
public class FlatscreenTestTools : EditorWindow
{
    private XRDeviceSimulator simulator;
    private bool useXRSimulator;

    [MenuItem("Tools/Flatscreen Test Tools")]
    static void ShowEditorWindow()
    {
        GetWindow<FlatscreenTestTools>("Flatscreen Test Tools");
    }

    void OnGUI()
    {
        useXRSimulator = EditorGUILayout.Toggle("Use XR Device Simulator", useXRSimulator);

        if (!Application.isPlaying)
        {
            return;
        }

        if (simulator == null)
        {
            simulator = Instantiate(AssetDatabase.LoadAssetAtPath<XRDeviceSimulator>(
                "Assets/Samples/XR Interaction Toolkit/3.0.7/XR Device Simulator/XR Device Simulator.prefab"
            ));
            DontDestroyOnLoad(simulator);
        }

        if (simulator.gameObject.activeSelf != useXRSimulator)
        {
            simulator.gameObject.SetActive(useXRSimulator);
        }
    }
}
