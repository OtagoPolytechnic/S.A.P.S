using System.Collections;
using UnityEngine;

public class WorldSpaceEventFeedback : MonoBehaviour
{
    [SerializeField] MeshRenderer playerSphere;
    [SerializeField] AnimationCurve sphereAlphaAnimationCurve;
    [SerializeField] float sphereAlphaAnimationDuration;
    [SerializeField, Range(10, 50)] float sphereClosedRadius;
    [SerializeField, Range(50, 1000)] float sphereOpenRadius;
    [SerializeField] AnimationCurve sphereRadiusAnimationCurve;
    [SerializeField] float sphereRadiusAnimationDuration;
    [SerializeField, Range(0.01f, 1)] float closedTimeScale;
    [SerializeField] float timeScaleAnimationDuration;

    void Start()
    {
        // set sphere material to instance so we don't modify the one in the asset database
        playerSphere.material = new Material(playerSphere.material);
        playerSphere.material.SetFloat("_Alpha", 0);
        playerSphere.transform.localScale = Vector3.one * sphereOpenRadius;
        Time.timeScale = 1;

        // temporary
        StartCoroutine(TestCoroutine());
    }

    IEnumerator OpenSphereCoroutine()
    {
        StartCoroutine(AnimateSphereRadiusCoroutine(sphereOpenRadius));
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(AnimateSphereAlphaCoroutine(0));
        yield return new WaitForSecondsRealtime(0.2f);
        yield return StartCoroutine(AnimateTimeScale(1));
    }

    IEnumerator CloseSphereCoroutine()
    {
        StartCoroutine(AnimateTimeScale(closedTimeScale));
        StartCoroutine(AnimateSphereAlphaCoroutine(1));
        yield return new WaitForSecondsRealtime(0.1f);
        yield return StartCoroutine(AnimateSphereRadiusCoroutine(sphereClosedRadius));
    }

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

    IEnumerator AnimateTimeScale(float targetTimeScale)
    {
        float startScale = Time.timeScale;
        float animValue = 0;

        while (Time.timeScale != targetTimeScale)
        {
            animValue += Time.unscaledDeltaTime / timeScaleAnimationDuration;
            animValue = Mathf.Clamp(animValue, 0, 1);
            Time.timeScale = Mathf.Lerp(startScale, targetTimeScale, animValue);
            print("time scale: " + Time.timeScale);
            yield return null;
        }
    }

    IEnumerator TestCoroutine()
    {
        yield return new WaitForSecondsRealtime(1);
        while (true)
        {
            yield return StartCoroutine(CloseSphereCoroutine());
            yield return new WaitForSecondsRealtime(1);
            yield return StartCoroutine(OpenSphereCoroutine());
            yield return new WaitForSecondsRealtime(1);
        }
    }
}
