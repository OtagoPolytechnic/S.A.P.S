using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

/// <summary>
/// Applies the fade-to-black material from <see cref="SceneLoader"/> 
/// to this camera overlay's MeshRenderer.
/// </summary>
public class CameraOverlayFade : MonoBehaviour
{
    void Start()
    {
        GetComponent<MeshRenderer>().material = SceneLoader.Instance.FadeMatInstance;
    }
}
