using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Closes the players's range of vision and gives them text prompts in world space
/// </summary>
public class WorldSpaceEventFeedback : MonoBehaviour
{
    [Header("Sphere")]
    [SerializeField] MeshRenderer playerSphere;
    [SerializeField] AnimationCurve sphereAlphaAnimationCurve;
    [SerializeField] float sphereAlphaAnimationDuration;
    [SerializeField, Range(10, 50)] float sphereClosedRadius;
    [SerializeField, Range(50, 1000)] float sphereOpenRadius;
    [SerializeField] AnimationCurve sphereRadiusAnimationCurve;
    [SerializeField] float sphereRadiusAnimationDuration;
    [Header("Time scale")]
    [SerializeField, Range(0.01f, 1)] float closedTimeScale;
    [SerializeField] float timeScaleAnimationDuration;
    [Header("Objects")]
    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] PlayerEnterTrigger startEventTrigger;

    private Vector3 textLocalPosition;

    IEnumerator Start()
    {
        // set sphere material to instance so we don't modify the one in the asset database
        playerSphere.material = new Material(playerSphere.material);
        playerSphere.material.SetFloat("_Alpha", 0);
        playerSphere.transform.parent = Camera.main.transform;
        playerSphere.transform.localScale = Vector3.one * sphereOpenRadius;
        playerSphere.transform.localPosition = Vector3.zero;
        Time.timeScale = 1;
        textLocalPosition = textMeshPro.transform.localPosition;

        while (NPCSpawner.Instance.Target == null) yield return null; // wait for target to spawn
        startEventTrigger.onPlayerExit.AddListener(HandlePlayerExitedElevator);
        startEventTrigger.onPlayerEnter.AddListener(HandlePlayerEnteredElevator);
        NPCSpawner.Instance.Target.GetComponent<Hurtbox>().onDie.AddListener(HandleTargetDeath);
    }

    #region feedback display
    /// <summary>
    /// Gives feedback via text prompts to the player while closing their vision, then goes away
    /// </summary>
    /// <param name="feedback">Prompts to display one at a time</param>
    /// <param name="overlayObjects">Objects to put in the overlay layer when vision range is shortened</param>
    /// <param name="overlayChildObjects"></param>
    public void DisplayFeedback(string[] feedback, GameObject[] overlayObjects, bool overlayChildObjects = true)
    {
        Dictionary<GameObject, int> objectLayers = new(); // remember what layer the objects are originally on
        foreach (GameObject obj in overlayObjects)
        {
            GetObjectLayers(obj, objectLayers, overlayChildObjects);
        }
        StartCoroutine(DisplayFeedbackCoroutine(feedback, objectLayers));
    }

    /// <summary>
    /// Creates a dictionary of gameobjects and their current layers
    /// </summary>
    void GetObjectLayers(GameObject obj, Dictionary<GameObject, int> objectLayers, bool includeChildren = true)
    {
        objectLayers.Add(obj, obj.layer);
        if (includeChildren)
        {
            foreach (Transform childTransform in obj.transform)
            {
                GetObjectLayers(childTransform.gameObject, objectLayers, true);
            }
        }
    }

    /// <summary>
    /// Gives feedback via text prompts to the player while closing their vision, then goes away
    /// </summary>
    IEnumerator DisplayFeedbackCoroutine(string[] feedback, Dictionary<GameObject, int> objectLayers)
    {
        StartCoroutine(CloseSphereCoroutine());
        yield return new WaitForSecondsRealtime(2);
        foreach (GameObject obj in objectLayers.Keys)
        {
            obj.layer = LayerMask.NameToLayer("Overlay");
        }
        foreach (string feedbackString in feedback)
        {
            DisplayText(feedbackString);
            yield return new WaitForSecondsRealtime(2.4f);
            HideText();
            yield return new WaitForSecondsRealtime(0.4f);
        }
        foreach (GameObject obj in objectLayers.Keys)
        {
            obj.layer = objectLayers[obj];
        }
        yield return StartCoroutine(OpenSphereCoroutine());
    }

    /// <summary>
    /// Places text in front of the player based on orientation of the camera
    /// </summary>
    void DisplayText(string text)
    {
        textMeshPro.transform.parent = Camera.main.transform;
        textMeshPro.transform.localPosition = textLocalPosition;
        textMeshPro.transform.localRotation = Quaternion.identity;
        textMeshPro.text = text;
        textMeshPro.gameObject.SetActive(true);
        textMeshPro.transform.parent = transform;
    }

    void HideText() => textMeshPro.gameObject.SetActive(false);
    #endregion

    #region event listeners
    void HandlePlayerExitedElevator()
    {
        if (GameState.Instance.CurrentContractState == GameState.ContractState.BEGINNING)
        {
            // TODO move this to the elevator script
            GameState.Instance.CurrentContractState = GameState.ContractState.SEEKING_TARGET;
            DisplayFeedback(new[] { "Kill the target", "Spare the innocent", "Avoid guards" }, new[] { NPCSpawner.Instance.Target.gameObject }, true);
        }
    }

    void HandlePlayerEnteredElevator()
    {
        if (GameState.Instance.CurrentContractState == GameState.ContractState.SEEKING_TARGET)
        {
            DisplayFeedback(new[] { "Kill the target" }, new[] { NPCSpawner.Instance.Target.gameObject }, true);
        }
    }

    void HandleTargetDeath(GameObject targetObj)
    {
        // TODO move this to NPCEventManager
        GameState.Instance.CurrentContractState = GameState.ContractState.RETURNING_TO_BASE;
        DisplayFeedback(new[] { "Go back to the elevator", "Don't get caught" }, new[] { GameObject.Find("SAPS Building"), targetObj }, true);
    }
    #endregion

    #region animation coroutines
    /// <summary>
    /// Animates time scale to be normal and opens up the sphere, making it invisible
    /// </summary>
    IEnumerator OpenSphereCoroutine()
    {
        StartCoroutine(AnimateSphereRadiusCoroutine(sphereOpenRadius));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(AnimateSphereAlphaCoroutine(0));
        yield return new WaitForSecondsRealtime(0.2f);
        yield return StartCoroutine(AnimateTimeScale(1));
    }

    /// <summary>
    /// Slows down time, closes the sphere in and makes it opaque
    /// </summary>
    IEnumerator CloseSphereCoroutine()
    {
        StartCoroutine(AnimateTimeScale(closedTimeScale));
        StartCoroutine(AnimateSphereAlphaCoroutine(1));
        yield return new WaitForSecondsRealtime(0.1f);
        yield return StartCoroutine(AnimateSphereRadiusCoroutine(sphereClosedRadius));
    }

    /// <summary>
    /// Changes the transparency of the sphere over time, using an animation curve
    /// </summary>
    IEnumerator AnimateSphereAlphaCoroutine(float targetAlpha)
    {
        float startAlpha = playerSphere.material.GetFloat("_Alpha");
        float alpha = startAlpha;
        float animLinear = 0;
        float animCurved;

        while (alpha != targetAlpha)
        {
            animLinear += Time.unscaledDeltaTime / sphereAlphaAnimationDuration;
            animLinear = Mathf.Clamp(animLinear, 0, 1);
            animCurved = sphereAlphaAnimationCurve.Evaluate(animLinear);
            alpha = Mathf.Lerp(startAlpha, targetAlpha, animCurved);
            playerSphere.material.SetFloat("_Alpha", alpha);
            yield return null;
        }
    }

    /// <summary>
    /// Changes the size of the sphere over time, using an animation curve
    /// </summary>
    IEnumerator AnimateSphereRadiusCoroutine(float targetRadius)
    {
        float startRadius = playerSphere.transform.localScale.x;
        float radius = startRadius;
        float animLinear = 0;
        float animCurved;

        while (radius != targetRadius)
        {
            animLinear += Time.unscaledDeltaTime / sphereRadiusAnimationDuration;
            animLinear = Mathf.Clamp(animLinear, 0, 1);
            animCurved = sphereRadiusAnimationCurve.Evaluate(animLinear);
            radius = Mathf.Lerp(startRadius, targetRadius, animCurved);
            playerSphere.transform.localScale = Vector3.one * radius;
            yield return null;
        }
    }

    /// <summary>
    /// Linearly changes time scale
    /// </summary>
    IEnumerator AnimateTimeScale(float targetTimeScale)
    {
        float startScale = Time.timeScale;
        float animValue = 0;

        while (Time.timeScale != targetTimeScale)
        {
            animValue += Time.unscaledDeltaTime / timeScaleAnimationDuration;
            animValue = Mathf.Clamp(animValue, 0, 1);
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, animValue);
            yield return null;
        }
    }
    #endregion
}
