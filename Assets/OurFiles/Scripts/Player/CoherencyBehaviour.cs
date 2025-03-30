using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

//Base written by: Rohan Anakin

//this script should be attached to the player's Coherency object
public class CoherencyBehaviour : MonoBehaviour
{
    public List<GameObject> npcs = new();
    private bool coherent = false;
    public bool Coherent { get { return coherent; } }
    private bool decaying = false;
    private bool readyForDecay = false;
    public static CoherencyBehaviour Instance { get; private set; }
    private const int NEEDED_NPCS = 3;
    private const int DECAY_RATE = 1;
    private const float DECAY_TIME = 1f;
    private float decayTimer = DECAY_TIME;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }    
    }

    // Update is called once per frame
    void Update()
    {
        if (decaying) 
        { 
            DecayCoherency();
            return; 
        }

        if (npcs.Count >= NEEDED_NPCS)
        {
            coherent = true;
            readyForDecay = true;
        }
        else
        {
            if (readyForDecay)
            {
                decaying = true;
                readyForDecay = false;
            }
            else 
            {
                coherent = false;
            }
        }
    }

    void DecayCoherency()
    {
        if (npcs.Count >= NEEDED_NPCS)
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcs.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            npcs.Remove(other.gameObject);
        }
    }
}
