using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class SAPSArrowLights : MonoBehaviour
{
    /// <summary>
    /// A pair of lights on the SAPS arrow. Can be enabled or disabled (will toggle which object is actually enabled visually rather than switch material for performance)
    /// </summary>
    [System.Serializable]
    private class LightPair
    {
        public List<GameObject> lightsOn;
        public List<GameObject> lightsOff;

        /// <summary>
        /// Enables the pair of lights
        /// </summary>
        public void Enable()
        {
            foreach (GameObject light in lightsOff)
            {
                light.SetActive(false);
            }
            foreach (GameObject light in lightsOn)
            {
                light.SetActive(true);
            }
        }

        /// <summary>
        /// Disables the pair of lights
        /// </summary>
        public void Disable()
        {
            foreach (GameObject light in lightsOff)
            {
                light.SetActive(true);
            }
            foreach (GameObject light in lightsOn)
            {
                light.SetActive(false);
            }
        }
    }

    // Negative goes downwards
    [SerializeField] private int direction = -1;
    [SerializeField] private int length = 2;
    [SerializeField] private float lightChangeDelay = 0.25f;
    [SerializeField] private List<LightPair> lights = new List<LightPair>();
    [SerializeField] private float hiddenZPos;
    [SerializeField] private float shownZPos;
    [SerializeField] private float moveArrowSpeed;

    private Coroutine lightLoop;

    private void Start()
    {
        // Disables all lights to begin with
        StopLights();

        lightLoop = StartCoroutine(RunLights());
    }

    private IEnumerator EnableArrow()
    {
        yield return StartCoroutine(MoveArrow(shownZPos));
        StartCoroutine(RunLights());
    }

    private IEnumerator MoveArrow(float goalPos)
    {
        float startZ = transform.localPosition.z;
        float lerpValue = 0;

        while (lerpValue <= 1)
        {
            float newZ = Mathf.Lerp(startZ, goalPos, lerpValue);

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);

            yield return null;
            lerpValue += Time.deltaTime * moveArrowSpeed;
        }
    }

    /// <summary>
    /// The lights running loop. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator RunLights()
    {
        int mainLightIndex = 0;
        // The amount of lights that will be off at once
        int lightGroupSize = lights.Count / length;

        while (true)
        {
            // Gets the index of the next light in the sequence.
            int nextLightIndex = TrueMod(mainLightIndex + direction, lights.Count);

            // Enables the front and disables the back of each light group (groups of size `length`)
            for (int i = 0; i < lightGroupSize; i++)
            {
                int index = TrueMod(mainLightIndex + (i * length), lights.Count);

                lights[TrueMod(index + 1, lights.Count)].Enable();
                lights[index].Disable();
            }

            yield return new WaitForSeconds(lightChangeDelay);

            // Updates the light index count
            mainLightIndex = nextLightIndex;
        }
    }

    /// <summary>
    /// Stops the light loop and turns off all lights
    /// </summary>
    private void StopLights()
    {
        if (lightLoop != null) StopCoroutine(lightLoop);

        foreach (LightPair light in lights)
        {
            light.Disable();
        }
    }

    /// <summary>
    /// Returns a true Modulo.
    /// The default % is a remainder and does not work for negatives, this will always return a positive Modulo.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private int TrueMod(int input, int modMax)
    {
        return ((input % modMax) + modMax) % modMax;
    }
}
