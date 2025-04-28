using System;
using UnityEngine;
using Random = UnityEngine.Random;

// base written by joshii

/// <summary>
/// Creates variation in character objects
/// </summary>
public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private CharacterFeaturePackSO featurePack;
    [SerializeField] private Transform lazySusan;
    [SerializeField] private int contractCardRenderLayer;

    public CharacterFeaturePackSO FeaturePack
    {
        get => featurePack; set => featurePack = value;
    }

    /// <summary>
    /// List of the features that must have a unique combination to define a unique NPC
    /// </summary>
    private enum UniqueFeatures
    {
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
    private CharacterModel targetModel;

    void Start()
    {
        targetModel = SpawnTargetModel(lazySusan, contractCardRenderLayer);
        targetModel.body.layer = contractCardRenderLayer;
    }

    /// <summary>
    /// Creates a new character model with random features. Avoids looking like the target NPC.
    /// </summary>
    public CharacterModel SpawnNPCModel(Transform parent)
    {
        CharacterModel model = new(featurePack.bodyMargins);
        model.SpawnBody(featurePack.bodyMesh, parent);

        // generate random features and do not match the same combo as the target
        int[] featureIndexes;
        do
        {
            featureIndexes = GetRandomFeatureIndexes();
        } while (featureIndexes.Equals(targetFeatureIndexes));

        AddFeatures(model, featureIndexes);

        return model;
    }

    /// <summary>
    /// Creates a new character model with random features, which are defined when CharacterCreator initializes
    /// </summary>
    public CharacterModel SpawnTargetModel(Transform parent, int layer = 0)
    {
        GameObject body;
        if (targetModel == null)
        {
            targetFeatureIndexes = GetRandomFeatureIndexes();
            targetModel = new(featurePack.bodyMargins);
            body = targetModel.SpawnBody(featurePack.bodyMesh, parent);
            AddFeatures(targetModel, targetFeatureIndexes);
        }
        else
        {
            body = Instantiate(targetModel.body, parent);
        }
        foreach (Transform child in body.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layer;
        }
        return targetModel;
    }

    /// <summary>
    /// Adds the unique features to a character model
    /// </summary>
    void AddFeatures(CharacterModel model, int[] featureIndexes)
    {
        model.eyes = model.AddFeature(featurePack.eyes[featureIndexes[(int)UniqueFeatures.EYES]], GetRandomPlacement(featurePack.eyeRange, true));
        model.mouth = model.AddFeature(featurePack.mouths[featureIndexes[(int)UniqueFeatures.MOUTH]], GetRandomPlacement(featurePack.mouthRange));
        model.snoz = model.AddFeature(featurePack.snozzes[featureIndexes[(int)UniqueFeatures.SNOZ]], GetRandomPlacement(featurePack.snozRange));
    }

    /// <summary>
    /// Returns an array of indexes mapped to the lists of feature objects
    /// e.g. [1, 5, 3] => eye 1, mouth 5, snoz 3
    /// </summary>
    int[] GetRandomFeatureIndexes()
    {
        int length = Enum.GetValues(typeof(UniqueFeatures)).Length;
        int[] featureIndexes = new int[length];
        featureIndexes[(int)UniqueFeatures.EYES] = Random.Range(0, featurePack.eyes.Length - 1);
        featureIndexes[(int)UniqueFeatures.MOUTH] = Random.Range(0, featurePack.mouths.Length - 1);
        featureIndexes[(int)UniqueFeatures.SNOZ] = Random.Range(0, featurePack.snozzes.Length - 1);
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
