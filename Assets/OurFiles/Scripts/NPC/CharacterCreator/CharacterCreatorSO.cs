using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterCreatorSO", menuName = "Scriptable Objects/CharacterCreatorSO")]
public class CharacterCreatorSO : ScriptableObject
{
    [Header("Face")]
    public Texture2D[] eyes;
    [Header("Accessories")]
    public GameObject[] hats;
}
