using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Unity.VRTemplate
{
    /// <summary>
    /// Controls the steps in the in coaching card.
    /// </summary>
    public class StepManager : MonoBehaviour
    {
        [Serializable]
        class Step
        {
            [SerializeField]
            public GameObject stepObject;

            [SerializeField]
            public string buttonText;
        }

        public TextMeshProUGUI stepButtonTextField;

        [SerializeField]
        List<Step> stepList = new List<Step>();

        [SerializeField]
        GameObject panel;

        int currentStepIndex = 0;

        public void Next()
        {
            stepList[currentStepIndex].stepObject.SetActive(false);
            currentStepIndex++;
            if (currentStepIndex % stepList.Count == 0)
            {
                panel.SetActive(false);
                return;
            }
            stepList[currentStepIndex].stepObject.SetActive(true);
            stepButtonTextField.text = stepList[currentStepIndex].buttonText;
        }
    }
}
