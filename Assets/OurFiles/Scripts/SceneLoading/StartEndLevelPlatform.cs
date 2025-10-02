using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]

/// <summary>
/// Activates a platform at the end of a level. 
/// When enabled, it changes material and triggers a win event if the player steps on it.
/// </summary>
public class StartEndLevelPlatform : MonoBehaviour
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private string sceneToLoadName;

    public event Action onGameWin;

    private const string PLAYER_TAG = "MainCamera";
    private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG) && isActive)
        {
            onGameWin?.Invoke();
        }
    }

    public void EnablePlatform()
    {
        isActive = true;
        mesh.material = enabledMaterial;
    }
}
