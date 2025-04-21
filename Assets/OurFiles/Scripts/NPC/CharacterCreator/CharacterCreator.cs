using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// Creates variation in character objects
/// </summary>
public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private CharacterFeaturePackSO featurePack;

    public CharacterFeaturePackSO FeaturePack
    {
        get => featurePack; set => featurePack = value;
    }
    private List<CharacterModel> models = new();
    
    /// <summary>
    /// Creates a new character model with no features
    /// </summary>
    /// <returns>The spawned character model</returns>
    public CharacterModel SpawnCharacterModel(Transform parent = null)
    {
        CharacterModel model = new(featurePack.bodyMargins);
        if (parent == null)
        {
            parent = transform;
        }
        model.SpawnBody(featurePack.bodyMesh, parent);
        model.eyes = model.AddFeature(featurePack.eyes[0], featurePack.eyeRange.defaultSetting);
        model.mouth = model.AddFeature(featurePack.mouths[0], featurePack.mouthRange.defaultSetting);
        model.snoz = model.AddFeature(featurePack.snozzes[0], featurePack.snozRange.defaultSetting);
        models.Add(model);
        return model;
    }
}
