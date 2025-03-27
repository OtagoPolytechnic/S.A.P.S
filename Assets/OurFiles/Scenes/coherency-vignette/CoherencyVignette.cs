using UnityEngine;

//Base written by: Jenna Boyes

/// <summary>
/// Allow coherency vignette to be controlled by other scripts
/// </summary>
public class CoherencyVignette : MonoBehaviour
{
    [SerializeField, Tooltip("The material that shows the vinette with an aperture in the middle")] 
    Material material;

    [SerializeField, Range(0.0f, 1.0f), Tooltip("The aperture size that is to be shown when the vingette is turned on")] 
    float workingApertureSize;

    [SerializeField, Tooltip("The percentage of the aperture to grows/shrinks per frame")] 
    float fadeSpeed;

    [SerializeField, Tooltip("The percentage of aperture size that will be unnoticable to jump, since change is exponential and can run forever")] 
    float exponentialThreshold;

    float hiddenApertureSize = 1; //100% of vignette is covered by transparent aperture
    float currentApertureSize;
    float goalSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //hide the vignette on start
        goalSize = hiddenApertureSize;
        material.SetFloat("_Aperture_Size", hiddenApertureSize);
    }

    private void Update()
    {
        currentApertureSize = material.GetFloat("_Aperture_Size");

        //fade the vinette in/out when needed
        if (goalSize != currentApertureSize) 
        {
            //stop the exponential change of current never reaching goal
            if (Mathf.Abs(currentApertureSize - goalSize) < exponentialThreshold)
            {
                material.SetFloat("_Aperture_Size", goalSize);
            }
            else 
            {
                float newSize = Mathf.Lerp(currentApertureSize, goalSize, fadeSpeed * Time.deltaTime);
                material.SetFloat("_Aperture_Size", newSize);
            }
        }
    }

    public void Show()
    {
        if (currentApertureSize != workingApertureSize)
        {
            goalSize = workingApertureSize;
        }
    }

    public void Hide()
    {
        if (currentApertureSize != hiddenApertureSize)
        {
            goalSize = hiddenApertureSize;
        }
    }

    private void OnDestroy()
    {
        //before game closes reset the value of the material cause SetFloat edits it permanently
        material.SetFloat("_Aperture_Size", workingApertureSize);
    }
}
