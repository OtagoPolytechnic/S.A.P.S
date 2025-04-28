using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

    /// <summary>
    /// List of the features that must have a unique combination to define a unique NPC
    /// </summary>
    private enum UniqueFeatures {
        EYES,
        MOUTH,
        SNOZ,
    }

    /// <summary>
    /// Primitive information about a CharacterModel.Feature before generating one
    /// </summary>
    private struct Feature
    {
        public float angle;
        public float height;
        public int objectIndex;
    }

    private int[] targetFeatureIndexes;

    void Start()
    {
        targetFeatureIndexes = GetRandomFeatureIndexes();
    }

    /// <summary>
    /// Creates a new character model with default features
    /// </summary>
    public CharacterModel SpawnDefaultModel(Transform parent = null)
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
        return model;
    }

    /// <summary>
    /// Creates a new character model with random features. Avoids looking like the target NPC.
    /// </summary>
    public CharacterModel SpawnNPCModel(Transform parent)
    {
        CharacterModel model = new(featurePack.bodyMargins);
        model.SpawnBody(featurePack.bodyMesh, parent);
        int[] featureIndexes;
        // generate random features and do not match the same combo as 
        do
        {
            featureIndexes = GetRandomFeatureIndexes();
        } while (featureIndexes.Equals(targetFeatureIndexes));

        // maybe use the random placements?

        model.eyes = model.AddFeature(featurePack.eyes[featureIndexes[(int)UniqueFeatures.EYES]], GetRandomPlacement(featurePack.eyeRange, true));
        model.mouth = model.AddFeature(featurePack.mouths[featureIndexes[(int)UniqueFeatures.MOUTH]], GetRandomPlacement(featurePack.mouthRange));
        model.snoz = model.AddFeature(featurePack.snozzes[featureIndexes[(int)UniqueFeatures.SNOZ]], GetRandomPlacement(featurePack.snozRange));

        return model;
    }

    /// <summary>
    /// Creates a new character model with random features, which are defined when CharacterCreator initializes
    /// </summary>
    /// <param name="parent"></param>
    public CharacterModel SpawnTargetModel(Transform parent)
    {
        CharacterModel model = new(featurePack.bodyMargins);
        model.SpawnBody(featurePack.bodyMesh, parent);
        model.eyes = model.AddFeature(featurePack.eyes[targetFeatureIndexes[(int)UniqueFeatures.EYES]], GetRandomPlacement(featurePack.eyeRange, true));
        model.mouth = model.AddFeature(featurePack.mouths[targetFeatureIndexes[(int)UniqueFeatures.MOUTH]], GetRandomPlacement(featurePack.mouthRange));
        model.snoz = model.AddFeature(featurePack.snozzes[targetFeatureIndexes[(int)UniqueFeatures.SNOZ]], GetRandomPlacement(featurePack.snozRange));
        return model;
    }

    /// <summary>
    /// Returns an array of indexes mapped to the lists of feature objects
    /// e.g. [1, 5, 3] => eye 1, mouth 5, snoz 3
    /// </summary>
    /// <returns></returns>
    int[] GetRandomFeatureIndexes()
    {
        int length = Enum.GetValues(typeof(UniqueFeatures)).Length;
        int[] featureIndexes = new int[length];
        featureIndexes[(int)UniqueFeatures.EYES] = Random.Range(0, featurePack.eyes.Length-1);
        featureIndexes[(int)UniqueFeatures.MOUTH] = Random.Range(0, featurePack.mouths.Length-1);
        featureIndexes[(int)UniqueFeatures.SNOZ] = Random.Range(0, featurePack.snozzes.Length-1);
        return featureIndexes;
    }

    /// <summary>
    /// Finds the array of feature objects from FeaturePack for the identified unique feature
    /// </summary>
    GameObject[] UniqueFeatureToObjectArray(UniqueFeatures identifier)
    {
        return identifier switch
        {
            UniqueFeatures.EYES => FeaturePack.eyes,
            UniqueFeatures.MOUTH => FeaturePack.mouths,
            UniqueFeatures.SNOZ => FeaturePack.snozzes,
            _ => FeaturePack.accessories,
        };
    }

    /// <summary>
    /// Randomize angle and height of feature placement within a given range
    /// </summary>
    CharacterModel.Feature.PlacementSetting GetRandomPlacement(CharacterModel.Feature.PlacementRange range, bool toMirror = false, bool toProtrude = false)
    {
        return new()
        {
            angle = Mathf.Lerp(range.angleMin, range.angleMax, Random.value),
            height = Mathf.Lerp(range.heightMin, range.heightMax, Random.value),
            mirroring = toMirror,
            protruding = toProtrude,
        };
    }
}
