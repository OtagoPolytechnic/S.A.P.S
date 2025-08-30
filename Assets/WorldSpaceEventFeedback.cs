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

    void Start()
    {
        // set sphere material to instance so we don't modify the one in the asset database
        playerSphere.material = new Material(playerSphere.material);
        playerSphere.material.SetFloat("_Alpha", 0);
        playerSphere.transform.localScale = Vector3.one * sphereOpenRadius;

        // temporary
        StartCoroutine(TestCoroutine());
    }

    void OpenSphere()
    {
        StartCoroutine(AnimateSphereAlphaCoroutine(0));
        StartCoroutine(AnimateSphereRadiusCoroutine(sphereOpenRadius));
    }

    void CloseSphere() => StartCoroutine(CloseSphereCoroutine());

    IEnumerator CloseSphereCoroutine()
    {
        StartCoroutine(AnimateSphereAlphaCoroutine(1));
        yield return new WaitForSeconds(0.5f); // looks nicer when alpha starts earlier
        StartCoroutine(AnimateSphereRadiusCoroutine(sphereClosedRadius));
    }

    IEnumerator AnimateSphereAlphaCoroutine(float targetAlpha)
    {
        float startAlpha = playerSphere.material.GetFloat("_Alpha");
        float alpha = startAlpha;
        float animLinear = 0;
        float animCurved;

        while (alpha != targetAlpha)
        {
            animLinear += Time.deltaTime / sphereAlphaAnimationDuration;
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
            animLinear += Time.deltaTime / sphereRadiusAnimationDuration;
            animLinear = Mathf.Clamp(animLinear, 0, 1);
            animCurved = sphereRadiusAnimationCurve.Evaluate(animLinear);
            radius = Mathf.Lerp(startRadius, targetRadius, animCurved);
            playerSphere.transform.localScale = Vector3.one * radius;
            yield return null;
        }
    }

    IEnumerator TestCoroutine()
    {
        while (true)
        {
            CloseSphere();
            yield return new WaitForSeconds(3);
            OpenSphere();
            yield return new WaitForSeconds(3);
        }
    }
}
