using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//a base written by Joshii

/// <summary>
/// Calculates a run score and animates filling star icons (out of 5 stars / 10 half-stars).
/// </summary>

public class ScoreDisplay : MonoBehaviour
{
    [Header("Score")]
    [SerializeField, Tooltip("How many half-stars to deduct if the player killed as many NPCs as the contract allowed")]
    private int innocentKillDeduction = 5;
    [SerializeField, Tooltip("How many half-stars to deduct if the player reached the time limit")]
    private int timeDeduction = 9;

    [Header("Stars")]
    [SerializeField] private Image stars;
    [SerializeField] private float starFillDelaySecs = 0.4f;
    [SerializeField, Range(0, 1)] private float starFillSpeed;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI innocentsKilledLabel;

    private int score;
    private int Score
    {
        get => score; set
        {
            // minimum score is 1 because 0 would be failure
            score = Mathf.Clamp(value, 1, 10);
        }
    }

    private float startTime;

    void Start()
    {
        CalculateScore();
        stars.fillAmount = 0;
        timeLabel.text += String.Format("{0:N}s", GameState.Instance.TimeSpent);
        innocentsKilledLabel.text += GameState.Instance.InnocentsKilled;
    }

    void Update()
    {
        if (Time.time - startTime <= starFillDelaySecs) return;

        stars.fillAmount = Mathf.Lerp(stars.fillAmount, score / 10f, starFillSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Computes score deductions based on time taken and innocents killed,
    /// then clamps to a 1–10 range.
    /// </summary> 
    void CalculateScore()
    {
        if (GameState.Instance == null)
        {
            Debug.LogWarning("No game state found");
            Score = 0;

            return;
        }
        GameState gameState = GameState.Instance;

        Score = 10;
        float deduction = 0;

        // time penalty
        if (gameState.TimeSpent > gameState.GoalTime)
        {
            deduction += timeDeduction * Mathf.InverseLerp(
                gameState.GoalTime, gameState.TimeLimit, gameState.TimeSpent
            );
        }
        // innocent murder penalty
        if (gameState.InnocentKillLimit > 0)
        {
            deduction += gameState.InnocentsKilled * innocentKillDeduction / (float)gameState.InnocentKillLimit;
        }

        Score -= Mathf.FloorToInt(deduction);
    }
}
