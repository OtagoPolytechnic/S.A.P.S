using System.Collections.Generic;
using UnityEngine;

// base written by joshii

/// <summary>
/// Creates variation in character objects
/// </summary>
public class CharacterCreator : MonoBehaviour
{
    [SerializeField] private CharacterFeaturePackSO featurePack;

    public CharacterFeaturePackSO FeaturePack => featurePack;
    public List<CharacterModel> Models { get; private set; }
    
    public CharacterModel SpawnCharacterModel()
    {
        return Instantiate(featurePack.prefab).GetComponent<CharacterModel>();
    }

    public void EditModel(CharacterModel model)
    {
        
    }
}