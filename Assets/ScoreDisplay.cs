using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// base written by Joshii

/// <summary>
/// Calculates a score out of 10, then displays score out of 5 stars (10 half stars)
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private int innocentKillDeduction = 5;
    [SerializeField] private float timeDeductionPerSec = 0.1f;

    [Header("Stars")]
    [SerializeField] private Image stars;
    [SerializeField] private float starFillDelaySecs = 0.4f;
    [SerializeField, Range(0, 0.01f)] private float starFillSpeed;

    [Header("Stats")]
    [SerializeField] private TextMeshProUGUI timeLabel;
    [SerializeField] private TextMeshProUGUI innocentsKilledLabel;

    private int score;
    private int Score
    {
        get => score; set
        {
            score = Mathf.Clamp(value, 0, 10);
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

        stars.fillAmount = Mathf.Lerp(stars.fillAmount, score / 10f, starFillSpeed);
    }

    void CalculateScore()
    {
        if (Contract.Instance == null)
        {
            Debug.LogWarning("No contract found");
            Score = 0;
            Debug.Log(score);

            return;
        }
        Contract contract = Contract.Instance;

        Score = 10;

        // time penalty
        if (contract.TimeSpent > contract.MinimumCompleteTime)
        {
            Score -= Mathf.FloorToInt(contract.TimeSpent * timeDeductionPerSec);
        }
        // innocent murder penalty
        if (contract.InnocentKillLimit > 0)
        {
            Score -= Mathf.FloorToInt(
                contract.InnocentsKilled * innocentKillDeduction
                / (float)contract.InnocentKillLimit
            );
        }
    }
}
