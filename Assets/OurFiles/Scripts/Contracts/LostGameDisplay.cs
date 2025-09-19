using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays a reason label when the game is lost,
/// based on the current <see cref="GameState"/>.
/// </summary>
public class LostGameDisplay : MonoBehaviour
{
    [Serializable]
    struct LostReason
    {
        public GameState.State loseState;
        public string reason;
    }

    [SerializeField] private List<LostReason> reasons;
    [SerializeField] private TextMeshProUGUI reasonLabel;

    void Start()
    {
        SetReason();
    }

    /// <summary>
    /// Finds the matching reason for the current game state
    /// and updates the reason label text.
    /// </summary>
    void SetReason()
    {
        LostReason lostReason = reasons.Find(m => m.loseState == GameState.Instance.CurrentState);
        reasonLabel.text = $"({lostReason.reason})";
    }
}
