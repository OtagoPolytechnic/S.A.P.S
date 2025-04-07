using System.Collections.Generic;
using UnityEngine;

// base written by joshii

[CreateAssetMenu(fileName = "CharacterCreatorSO", menuName = "Scriptable Objects/CharacterCreatorSO")]
public class CharacterFeaturePackSO : ScriptableObject
{
    public GameObject prefab;

    [Header("Body")]
    [Range(0, 2)] public float minRadius;
    [Range(0, 2)] public float maxRadius;
    [Range(0, 5)] public float minHeight;
    [Range(0, 5)] public float maxHeight;

    [Header("Face")]
    public GameObject[] eyes;
    [Tooltip("Default eye height"), Range(0, 100)]
    public int eyeHeight;
    
    [Header("Accessories")]
    public GameObject[] hats;
}
