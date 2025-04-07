using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// base written by joshii

/// <summary>
/// Use to test character creator
/// </summary>
public class CharacterCreatorDebug : EditorWindow
{
    // I used this as a base
    // CrowdDebug

    private static CharacterModel model;

    [MenuItem("Tools/Character Creator")]
    public static void ShowEditorWaindow()
    {
        GetWindow<CharacterCreatorDebug>("Character Creator");
    }

    public void OnGUI()
    {
        CharacterCreator characterCreator = FindFirstObjectByType<CharacterCreator>();
        if (characterCreator == null)
        {
            Debug.Log("Did not find a creator in scene. Making a new one.");
            GameObject creatorObject = new("CharacterCreator");
            characterCreator = creatorObject.AddComponent<CharacterCreator>();
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Spawn new model"))
        {
            if (model != null)
            {
                DestroyImmediate(model.gameObject);
            }
            model = characterCreator.SpawnCharacterModel();
        }
        GUILayout.Space(10);
        if (model == null)
        {
            GUILayout.Label("Spawn a character to edit");
            return;
        }
        GUILayout.Label("Edit character");
        EditCharacterMenu(characterCreator);
    }

    void EditCharacterMenu(CharacterCreator creator)
    {
        model.Radius = EditorGUILayout.Slider(
            new GUIContent("Radius"),
            model.Radius,
            creator.FeaturePack.minRadius,
            creator.FeaturePack.maxRadius
        );
        model.Height = EditorGUILayout.Slider(
            new GUIContent("Height"),
            model.Height,
            creator.FeaturePack.minHeight,
            creator.FeaturePack.maxHeight
        );
    }
}