using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

/// <summary>
/// Uses the instanced fade to black material from SceneLoader
/// </summary>
public class CameraOverlayFade : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material = SceneLoader.Instance.FadeMatInstance;
    }
}
