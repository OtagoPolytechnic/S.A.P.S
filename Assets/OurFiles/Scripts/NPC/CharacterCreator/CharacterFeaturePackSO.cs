using UnityEngine;

// base written by joshii

[CreateAssetMenu(fileName = "CharacterCreatorSO", menuName = "Scriptable Objects/CharacterCreatorSO")]
public class CharacterFeaturePackSO : ScriptableObject
{
    [Header("Body")]
    public GameObject bodyMesh;
    public CharacterModel.BodyMargins bodyMargins = new(){
        height = 2,
        radius = 0.5f,
        cylinderBottom = 0.5f,
        cylinderTop = 1.5f,
    };
    [Range(0, 2)] public float minRadius;
    [Range(0, 2)] public float maxRadius;
    [Range(0, 5)] public float minHeight;
    [Range(0, 5)] public float maxHeight;

    [Header("Skin Colours")]
    public Material[] skinColors;
    public Material guardColor;

    [Header("Face")]
    public GameObject[] eyes;
    public CharacterModel.Feature.PlacementRange eyeRange;
    public GameObject[] mouths;
    public CharacterModel.Feature.PlacementRange mouthRange;
    public GameObject[] snozzes;
    public CharacterModel.Feature.PlacementRange snozRange;

    /// <summary>
    /// accessories are features that don't have random positioning
    /// </summary>
    [Header("Accessories")]
    public GameObject[] accessories;
    public int maxAccessories;

    [Header("Voices")]
    public CharacterVoicePackSO[] voices;
}
