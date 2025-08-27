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

    [SerializeField] private int direction = -1;
    [SerializeField] private int distance = 2;
    [SerializeField] private List<LightPair> lights = new List<LightPair>();

    private Coroutine lightLoop;

    private void Start()
    {
        // Disables all lights to begin with
        StopLights();

        lightLoop = StartCoroutine(RunLights());
    }

    private IEnumerator RunLights()
    {
        int mainLightIndex = 0;
        // The amount of lights that will be on at once
        int offsetCount = lights.Count / distance;

        while (true)
        {
            int nextLightIndex = TrueMod(mainLightIndex + direction, lights.Count);

            for (int i = 0; i < offsetCount; i++)
            {
                int index = TrueMod(mainLightIndex + (i * distance), lights.Count);

                lights[TrueMod(index + 1, lights.Count)].Disable();
                lights[index].Enable();
            }

            yield return new WaitForSeconds(0.25f);

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
