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
    private List<CharacterModel> models;
    
    /// <summary>
    /// Creates a new character model with no features
    /// </summary>
    /// <returns>The spawned character model</returns>    
    public CharacterModel SpawnCharacterModel()
    {
        CharacterModel model = new GameObject("CharacterModel").AddComponent<CharacterModel>();
        model.SpawnBody(featurePack.body);
        model.AddFeature(featurePack.eyes[0], featurePack.eyeRange.defaultSetting, "eyes");
        models.Add(model);
        return model;
    }
}