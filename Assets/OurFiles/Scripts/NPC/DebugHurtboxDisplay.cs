using TMPro;
using UnityEngine;

/// <summary>
/// Displays the health value of a Hurtbox component on a canvas,
/// as a child of the object attached to the hurtbox
/// </summary>
public class DebugHurtboxDisplay : MonoBehaviour
{
    private Hurtbox hurtbox;
    private TextMeshProUGUI textBox;

    void Awake()
    {
        hurtbox = GetComponentInParent<Hurtbox>();
        if (hurtbox == null)
        {
            Debug.LogWarning("Could not find Hurtbox component in parent. Display is now disabled.");
            enabled = false;
            return;
        }

        textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        hurtbox.onHealthUpdate.AddListener(HandleHurtboxHealthUpdate);
        HandleHurtboxHealthUpdate(hurtbox.Health);
    }

    void HandleHurtboxHealthUpdate(int health)
    {
        textBox.text = $"Health: {health}";
    }
}
