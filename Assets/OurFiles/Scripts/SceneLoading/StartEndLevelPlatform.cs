using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class StartEndLevelPlatform : MonoBehaviour
{
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private string sceneToLoadName;

    private const string PLAYER_TAG = "MainCamera";
    private bool isActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG) && isActive)
        {
            // End Level
            sceneLoader.LoadScene(sceneToLoadName);
        }
    }

    public void EnablePlatform()
    {
        isActive = true;
        mesh.material = enabledMaterial;
    }
}
