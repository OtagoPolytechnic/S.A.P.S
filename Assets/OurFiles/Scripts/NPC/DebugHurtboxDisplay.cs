using TMPro;
using UnityEngine;

/// <summary>
/// Displays the health value of a <see cref="Hurtbox"/> on a child canvas.
/// Should be attached to the same GameObject hierarchy as the Hurtbox.
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

    /// <summary>
    /// Updates the health display text.
    /// </summary>
    /// <param name="health">The current health value.</param>    
    void HandleHurtboxHealthUpdate(int health)
    {
        textBox.text = $"Health: {health}";
    }
}
