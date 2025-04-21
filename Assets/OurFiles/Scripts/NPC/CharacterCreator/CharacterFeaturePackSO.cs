using UnityEngine;

// base written by joshii

[CreateAssetMenu(fileName = "CharacterCreatorSO", menuName = "Scriptable Objects/CharacterCreatorSO")]
public class CharacterFeaturePackSO : ScriptableObject
{
    public GameObject body;

    [Header("Body")]
    [Range(0, 2)] public float minRadius;
    [Range(0, 2)] public float maxRadius;
    [Range(0, 5)] public float minHeight;
    [Range(0, 5)] public float maxHeight;

    [Header("Face")]
    public GameObject[] eyes;
    public CharacterModel.Feature.PlacementRange eyeRange;
    public GameObject[] mouths;
    public CharacterModel.Feature.PlacementRange mouthRange;
    public GameObject[] snozzes;
    public CharacterModel.Feature.PlacementRange snozRange;
    
    
    [Header("Accessories")]
    public GameObject[] hats;
}
