using UnityEngine;
using UnityEditor;
using System;

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
    static void ShowEditorWaindow()
    {
        GetWindow<CharacterCreatorDebug>("Character Creator");
    }

    void OnGUI()
    {
        CharacterCreator characterCreator = FindFirstObjectByType<CharacterCreator>();
        if (characterCreator == null)
        {
            Debug.Log("Did not find a creator in scene. Making a new one.");
            characterCreator = SpawnNewCreator();
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
        if (GUILayout.Button("Destroy model"))
        {
            DestroyImmediate(model.gameObject);
            return;
        }

        GUILayout.Space(10);
        GUILayout.Label("Edit character");
        EditCharacterMenu(characterCreator);
    }

    CharacterCreator SpawnNewCreator()
    {
        CharacterCreator characterCreator;
        GameObject creatorObject = new("CharacterCreator");
        characterCreator = creatorObject.AddComponent<CharacterCreator>();
        characterCreator.FeaturePack = AssetDatabase.LoadAssetAtPath<CharacterFeaturePackSO>(
            "Assets/OurFiles/Scripts/NPC/CharacterCreator/DefaultFeaturePack.asset");
        return characterCreator;
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
        foreach (CharacterModel.Feature feature in model.Features)
        {
            GUILayout.Space(5);
            switch (feature.name)
    {
                case "eyes":
                    EditEyes(feature, creator.FeaturePack);
                    break;
                default:
                    EditFeature(feature, creator.FeaturePack);
                    break;
            }

        }
    }
    }

    void EditFeature(CharacterModel.Feature feature)
    {
        GUILayout.Label(feature.gameObject.name);
        CharacterModel.Feature.PlacementSetting placement = feature.Placement;
        placement.angle = EditorGUILayout.Slider(
            new GUIContent("angle"),
            feature.Placement.angle,
            0,
            2 * Mathf.PI
        );
        placement.height = EditorGUILayout.Slider(
            new GUIContent("height"), 
            feature.Placement.height, 
            0, 
            1
        );
        placement.mirroring = EditorGUILayout.Toggle(
            new GUIContent("mirroring"), 
            feature.Placement.mirroring
        );
        feature.Placement = placement;
    }

    void EditEyes(CharacterModel.Feature eye, CharacterFeaturePackSO featurePack)
    {
        GUILayout.Label("Eyes");
        CharacterModel.Feature.PlacementSetting placement = eye.Placement;
        CharacterModel.Feature.PlacementRange range = featurePack.eyeRange;
        placement.angle = EditorGUILayout.Slider(
            new GUIContent("spacing"),
            placement.angle,
            range.angleMin,
            range.angleMax
        );
        placement.height = EditorGUILayout.Slider(
            new GUIContent("height"),
            placement.height,
            range.heightMin,
            range.heightMax
        );
        eye.Placement = placement;
    }
}