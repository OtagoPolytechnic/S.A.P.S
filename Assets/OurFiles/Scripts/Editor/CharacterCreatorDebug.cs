using UnityEngine;
using UnityEditor;
using System;

// base written by joshii

/// <summary>
/// Editor window to quickly spawn a CharacterCreator, generate a model,
/// and tweak core body/feature settings for iteration.
/// </summary>
public class CharacterCreatorDebug : EditorWindow
{
    // I used this as a base
    // CrowdDebug

    private static CharacterModel model;
    private Vector2 scrollPos;

    /// <summary>Opens the Character Creator debug window.</summary>
    [MenuItem("Tools/Character Creator")]
    static void ShowEditorWaindow()
    {
        GetWindow<CharacterCreatorDebug>("Character Creator");
    }

    /// <summary>
    /// Ensures a CharacterCreator exists, lets you spawn/destroy a test model,
    /// and draws simple controls to edit it.
    /// </summary>
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
                // Destroying the body will also destroy the attached features
                DestroyImmediate(model.body);
            }
            model = characterCreator.SpawnNPCModel(characterCreator.gameObject.transform, NPCType.Passerby); // This NPCType might want to be a changable thing to configure it in the future
        }

        GUILayout.Space(10);
        if (model == null)
        {
            GUILayout.Label("Spawn a character to edit");
            return;
        }
        if (GUILayout.Button("Destroy model"))
        {
            DestroyImmediate(model.body);
            model = null;
            return;
        }

        GUILayout.Space(10);
        GUILayout.Label("Edit character");
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        EditCharacterMenu(characterCreator);
        GUILayout.EndScrollView();
    }
    
    /// <summary>
    /// Creates a temporary CharacterCreator GameObject with a default FeaturePack.
    /// </summary>
    CharacterCreator SpawnNewCreator()
    {
        CharacterCreator characterCreator;
        GameObject creatorObject = new("CharacterCreator");
        characterCreator = creatorObject.AddComponent<CharacterCreator>();
        characterCreator.FeaturePack = AssetDatabase.LoadAssetAtPath<CharacterFeaturePackSO>(
            "Assets/OurFiles/Scripts/NPC/CharacterCreator/DefaultFeaturePack.asset");
        return characterCreator;
    }

    /// <summary>
    /// Draws body sliders and per-feature editors using ranges/options from the creator.
    /// </summary>
    /// <param name="creator">Provides min/max ranges and prefab options.</param>
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
        foreach (CharacterModel.Feature feature in model.features)
        {
            GUILayout.Space(5);

            if (feature == model.eyes)
            {
                EditFeature(feature, creator.FeaturePack.eyes, creator.FeaturePack.eyeRange, false, "Eyes");
                continue;
            }
            if (feature == model.snoz)
            {
                EditFeature(feature, creator.FeaturePack.snozzes, creator.FeaturePack.snozRange, false, "Snoz");
                continue;
            }
            if (feature == model.mouth)
            {
                EditFeature(feature, creator.FeaturePack.mouths, creator.FeaturePack.mouthRange, false, "Mouth");
                continue;
            }
            // EditFeature(feature, creator.FeaturePack.accessories, creator.FeaturePack.accessoryRange);
        }
    }

    /// <summary>
    /// Lets you pick a prefab for the feature and adjust its placement.
    /// </summary>
    /// <param name="feature">Feature instance on the model to modify.</param>
    /// <param name="objectOptions">Available prefabs for this feature type.</param>
    /// <param name="range">Allowed angle/height bounds.</param>
    /// <param name="canRemove">If true, shows a remove button.</param>
    /// <param name="name">Section label.</param>
    void EditFeature(CharacterModel.Feature feature, GameObject[] objectOptions, CharacterModel.Feature.PlacementRange range, bool canRemove = false, string name = "Feature")
    {
        GUILayout.Label(name);
        EditPlacement(feature, range);
        int objectIndex = Array.IndexOf(objectOptions, feature.FeaturePrefab);
        objectIndex = EditorGUILayout.IntSlider(
            "style",
            objectIndex,
            0,
            objectOptions.Length - 1
        );
        feature.FeaturePrefab = objectOptions[objectIndex];
        if (canRemove)
        {
            // don't want to show button at all if feature cannot be removed
            if (GUILayout.Button("Remove feature"))
            {
                model.RemoveFeature(feature);
            }
        }
    }

    /// <summary>
    /// Edits angle and height within the given placement range.
    /// </summary>
    /// <param name="feature">Feature whose placement is being edited.</param>
    /// <param name="range">Min/max bounds for angle and height.</param>
    void EditPlacement(CharacterModel.Feature feature, CharacterModel.Feature.PlacementRange range)
    {
        CharacterModel.Feature.PlacementSetting placement = feature.Placement;
        placement.angle = EditorGUILayout.Slider(
            new GUIContent("angle"),
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
        feature.Placement = placement;
    }
}
