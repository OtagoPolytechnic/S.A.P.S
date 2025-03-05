using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class StartEndLevelPlatform : MonoBehaviour
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private MeshRenderer mesh;

    private const string PLAYER_TAG = "MainCamera";
    private bool isActive = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && isActive)
        {
            // End Level
            Debug.Log("End Level success!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(PLAYER_TAG) && !isActive)
        {
            EnablePlatform();
            Debug.Log("Player left");
        }
    }

    private void EnablePlatform()
    {
        isActive = true;
        mesh.material = enabledMaterial;
    }
}
