using UnityEngine;
using UnityEditor;

// base written by joshii

/// <summary>
/// Use to test character creator
/// </summary>
public class CharacterCreatorDebug : EditorWindow
{
    // I used this as a base
    // CrowdDebug

    [MenuItem("Tools/Character Creator")]
    public static void ShowEditorWindow()
    {
        GetWindow<CharacterCreatorDebug>("Character Creator");
    }

    public void OnGUI()
    {
        CharacterCreator characterCreator = FindFirstObjectByType<CharacterCreator>();
        if (characterCreator == null)
        {
            Debug.Log("Did not find a creator in scene. Making a new one.");
            GameObject creatorObject = new GameObject("CharacterCreator");
            characterCreator = creatorObject.AddComponent<CharacterCreator>();
        }
    }
}