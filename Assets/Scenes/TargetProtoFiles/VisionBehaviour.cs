using TMPro;
using UnityEngine;

//Base written by: Rohan Anakin

//this script should be attached to the NPC's vision cone object
public class VisionBehaviour : MonoBehaviour
{
    [Header("Vision Cone")]
    private MeshCollider visionArea;

    //if something else needs the data of what is in the NPC's vision cone
    public MeshCollider VisionArea 
    { get { return visionArea; } }
    [Header("Suspicion")]
    private float suspicion = 0.0f;
    private bool playerVisible = false;
    private bool playerFullySeen = false; //this will interact differently later to allow the NPC to call for help or flee
    private const float SUSPICION_MAX = 100f;
    [SerializeField]
    private TextMeshPro suspicionText;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        suspicion = 0.0f;
        playerFullySeen = false;
        suspicionText.text = "";
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFullySeen) { return; } //for testing purposes

        if (!playerVisible && suspicion > 0)
        {
            DecreaseSuspicion();
        }
        if (playerVisible)
        {
            IncreaseSuspicion();
            if (suspicion >= SUSPICION_MAX)
            {
                playerFullySeen = true;
                //record the player's position
                //call for help
            }
        }
    }

    void IncreaseSuspicion() //these will also add other variables to the suspicion meter based on the player's actions
    {
        suspicion += 0.1f;//arbitrary value
        suspicionText.text = suspicion.ToString("F0");
    }
    void DecreaseSuspicion()
    {
        suspicion -= 0.1f;//arbitrary value
        suspicionText.text = suspicion.ToString("F0");
        CheckSuspicion();
    }
    void CheckSuspicion() //error checking
    {
        if (suspicion <= 0)
        {
            suspicion = 0;
            suspicionText.text = "";
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerVisible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerVisible = false;
        }
    }
    //on the player entering the vision cone
    //shoot 2 raycasts to the player, one to head and one to the middle
    //if both hit the player, the player is visible to the NPC
    //while the player is visible the NPC will increase its suspicion
    //if the suspicion meter is full, the player loses //will change to be the NPC flees or calls for help

}
