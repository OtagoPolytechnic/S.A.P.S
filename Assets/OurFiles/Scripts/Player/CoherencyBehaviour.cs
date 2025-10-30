using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

//Base written by: Rohan Anakin
//Edited by: Jenna Boyes

//this script should be attached to the player's Coherency object

/// <summary>
/// Tracks nearby NPCs to determine if the player is "coherent".
/// Coherency activates a vignette effect when enough NPCs are present,
/// and decays if NPCs leave.
/// </summary>
public class CoherencyBehaviour : Singleton<CoherencyBehaviour>
{
    public List<GameObject> npcs = new();
    private bool coherent = false;
    public bool Coherent { get { return coherent; } }
    private bool decaying = false;
    private bool readyForDecay = false;
    private const int NEEDED_NPCS = 3;
    private const int DECAY_RATE = 1;
    private const float DECAY_TIME = 1f;
    private float decayTimer = DECAY_TIME;
    private bool isHiding = false;
    [SerializeField]
    private CoherencyVignette coherencyVignette;

    // Update is called once per frame
    void Update()
    {
        if (decaying)
        {
            DecayCoherency();
            return;
        }

        if (npcs.Count >= NEEDED_NPCS || isHiding)
        {
            coherent = true;
            readyForDecay = true;
            coherencyVignette.Show();
        }
        else
        {
            if (readyForDecay)
            {
                decaying = true;
                readyForDecay = false;
                coherencyVignette.Hide();
            }
            else
            {
                coherent = false;
            }
        }
    }

    /// <summary>
    /// Handles gradual loss of coherency when NPCs fall below the threshold.
    /// </summary>
    void DecayCoherency()
    {
        if (npcs.Count >= NEEDED_NPCS || isHiding)
        {
            decaying = false;
            decayTimer = DECAY_TIME;
            return;
        }

        decayTimer -= DECAY_RATE * Time.deltaTime;

        if (decayTimer <= 0)
        {
            decaying = false;
            decayTimer = DECAY_TIME;
        }
    } 

    public void SetHiding(bool isHiding)
    {
        this.isHiding = isHiding;
    }
}
