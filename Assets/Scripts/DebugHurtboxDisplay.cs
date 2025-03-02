using TMPro;
using UnityEngine;

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
        hurtbox.onHealthUpdate += HandleHurtboxHealthUpdate;
        HandleHurtboxHealthUpdate(hurtbox.Health);
    }

    void HandleHurtboxHealthUpdate(int health)
    {
        textBox.text = $"Health: {health}";
    }
}
