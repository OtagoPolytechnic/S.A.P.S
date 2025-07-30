using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LostGameDisplay : MonoBehaviour
{
    [Serializable] struct LostReason
    {
        public Contract.State loseState;
        public string reason;
    }

    [SerializeField] private List<LostReason> reasons;
    [SerializeField] private TextMeshProUGUI reasonLabel;

    void Start()
    {
        SetReason();

        Contract.Instance.EndContract();
    }

    void SetReason()
    {
        LostReason lostReason = reasons.Find(m => m.loseState == Contract.Instance.CurrentState);
        reasonLabel.text = $"({lostReason.reason})";
    }
}
