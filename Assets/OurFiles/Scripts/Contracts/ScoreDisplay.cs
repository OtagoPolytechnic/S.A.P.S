using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// base written by Joshii

/// <summary>
/// Calculates a score, then displays score out of 5 stars (10 half stars)
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
        timeLabel.text += String.Format("{0:N}s", Contract.Instance.TimeSpent);
        innocentsKilledLabel.text += Contract.Instance.InnocentsKilled;
        // destroy contract now that we are done with it
        Contract.Instance.EndContract();
    }

    void Update()
    {
        if (Time.time - startTime <= starFillDelaySecs) return;

        stars.fillAmount = Mathf.Lerp(stars.fillAmount, score / 10f, starFillSpeed * Time.deltaTime);
    }

    void CalculateScore()
    {
        if (Contract.Instance == null)
        {
            Debug.LogWarning("No contract found");
            Score = 0;

            return;
        }
        Contract contract = Contract.Instance;

        Score = 10;
        float deduction = 0;

        // time penalty
        if (contract.TimeSpent > contract.GoalTime)
        {
            deduction += timeDeduction * Mathf.InverseLerp(
                contract.GoalTime, contract.TimeLimit, contract.TimeSpent
            );
        }
        // innocent murder penalty
        if (contract.InnocentKillLimit > 0)
        {
            deduction += contract.InnocentsKilled * innocentKillDeduction / (float)contract.InnocentKillLimit;
        }

        Score -= Mathf.FloorToInt(deduction);
    }
}
