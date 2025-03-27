using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;

//Base written by: Jenna Boyes

/// <summary>
/// Allow coherency vignette to be controlled by other scripts
/// </summary>
public class CoherencyVignette : MonoBehaviour
{
    [SerializeField]
    Material material;
    float defaultApertureSize;
    float hiddenApertureSize = 1; //all vignette is covered by transparent aperture

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        defaultApertureSize = material.GetFloat("_Aperature_Size");
        print("default size: " + defaultApertureSize);
        Hide();
    }

    public void Show()
    {
        print("Showing coh vig");
        material.SetFloat("_Aperature_Size", defaultApertureSize);
    }

    public void Hide()
    {
        print("Hiding coh vig");
        material.SetFloat("_Aperature_Size", hiddenApertureSize);
    }

    private void OnDestroy()
    {
        //before game closes reset the value of the material cause SetFloat edits it permanently
        material.SetFloat("_Aperature_Size", defaultApertureSize);
    }
}
