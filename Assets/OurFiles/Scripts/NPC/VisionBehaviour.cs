using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

//Base written by: Rohan Anakin
//Edited by: Jenna Boyes

//this script should be attached to the NPC's vision cone object
public class VisionBehaviour : MonoBehaviour
{
    [Header("Suspicion")]
    private float suspicion;
    public float Suspicion { get => suspicion; set
        {
            suspicion = value;
            if (suspicion >= SUSPICION_MAX && npcPather.State != NPCPather.NPCState.Panic)
            {
                npcPather.State = NPCPather.NPCState.Panic;
            }
        }
    }
    private float suspicionValue = 0.0f;
    private bool playerInCone = false;
    private bool playerVisible = false;
    private bool chestVisible = false;
    private bool headVisible = false;
    private bool weaponVisible = false;
    private bool playerFullySeen = false; //this will interact differently later to allow the NPC to call for help or flee
    private const float SUSPICION_MIN = 0f;
    private const float SUSPICION_MAX = 100f;
    private const float CHEST_SUSPICION_INCREASE = 2f;
    private const float HEAD_VISIBILITY_INCREASE = 1f;
    private const float WEAPON_VISIBILITY_INCREASE = 2f;
    private const float BASE_SUSPICION_INCREASE = 4f;
    private const float SUSPICION_DECAY_RATE = 4f;
    private const float SUSPICION_INCREASE_NPC_DIE = 50f; //when visible NPC dies
    private const float SUSPICION_INCREASE_PLAYER_KILL = 50f; //when player visible if visible NPC dies 
    private const float SUSPICION_INCREASE_DEAD_NPC = 100f; //when an NPC sees a dead NPC on the ground

    [SerializeField]
    private TextMeshPro suspicionText;
    [SerializeField]
    private MeshCollider visionCone;
    private Collider player;
    private Camera playerCamera;
    private LayerMask playerLayerMask;
    private LayerMask npcLayerMask;
    private GameObject thisNPC; //the NPC that this vision cone is attached to 
    private WeaponManager weaponManager;
    private NPCPather npcPather;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        npcPather = GetComponentInParent<NPCPather>();
        Suspicion = SUSPICION_MIN;
        playerFullySeen = false;
        suspicionText.text = "";
        npcLayerMask = LayerMask.GetMask("NPC", "Default");
        playerLayerMask = LayerMask.GetMask("Player", "Default"); //default is every object created in the scene. If we make a layer for map geometry, we can switch Default to that layer
        playerCamera = Camera.main;
        thisNPC = gameObject.GetComponentInParent<Hurtbox>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFullySeen) { return; } //for testing purposes

        if (!playerVisible || !playerInCone && Suspicion > SUSPICION_MIN)
        {
            DecreaseSuspicion();
        }
        
        if (CoherencyBehaviour.Instance.Coherent && !playerFullySeen) //this behaviour will be changed later to allow the player to hide again after loosing the NPC
        {
            playerVisible = false;
            return;
        }

        if (playerInCone)
        {
            CheckVisiblity();
            if (Suspicion >= SUSPICION_MAX) //this needs to be moved later for ranged attacks
            {
                playerFullySeen = true;
                //record the player's position
                //call for help
            }
        }
    }

    void OnEnable()
    {
        SetOnEnabled(true);
    }

    void OnDisable()
    {
        SetOnEnabled(false);
    }

    void SetOnEnabled(bool isEnabled)
    {
        visionCone.enabled = isEnabled;
        suspicionText.gameObject.SetActive(isEnabled);
    }

    void CheckVisiblity()
    {
        Vector3 chestRayDirection = (player.transform.position + new Vector3(0,1,0)) - transform.position;
        Vector3 headRayDirection = (playerCamera.transform.position) - transform.position;
        //shoot a raycast to the CharacterController collider
        if (Physics.Raycast(transform.position, chestRayDirection, out RaycastHit hit, Mathf.Infinity, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.Equals(player.gameObject))
            {
                Debug.DrawLine(transform.position, hit.point, Color.green);
                chestVisible = true;
                
            }
            else
            {
                Debug.DrawLine(transform.position, hit.point, Color.red);
                chestVisible = false;
            }
        }
        //shoot a raycast to the player's head
        if (Physics.Raycast(transform.position, headRayDirection, out RaycastHit headHit, Mathf.Infinity, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (headHit.collider.gameObject.Equals(playerCamera.gameObject))
            {
                Debug.DrawLine(transform.position, headHit.point, Color.green);
                headVisible = true;
            }
            else
            {
                Debug.DrawLine(transform.position, headHit.point, Color.red);
                headVisible = false;
            }
        }
        //we do both of these to allow the NPC to see the player even if they are crouching behind cover because of the way the collider works with the VR rig
        // Refactor this at some point
        if (chestVisible && headVisible)
        {
            suspicionValue = BASE_SUSPICION_INCREASE;
            playerVisible = true;
        }
        else if (headVisible)
        {
            suspicionValue = HEAD_VISIBILITY_INCREASE;
            playerVisible = true;
        }
        else if (chestVisible)
        {
            suspicionValue = CHEST_SUSPICION_INCREASE;
            playerVisible = true;
        }
        else
        {
            playerVisible = false;
        }
        if (playerVisible)
        {
            IncreaseSuspicion();
        }
    }

    void IncreaseSuspicion() //these will also add other variables to the Suspicion meter based on the player's actions
    {
        Suspicion += suspicionValue * Time.deltaTime * (weaponVisible ? WEAPON_VISIBILITY_INCREASE : 1);
        suspicionText.text = Suspicion.ToString("F0");
    }

    void DecreaseSuspicion()
    {
        Suspicion -= SUSPICION_DECAY_RATE * Time.deltaTime;
        suspicionText.text = Suspicion.ToString("F0");
        ClampSuspicion();
    }

    void ClampSuspicion()
    {
        Suspicion = Mathf.Clamp(Suspicion, SUSPICION_MIN, SUSPICION_MAX);
        if (Suspicion == SUSPICION_MIN) suspicionText.text = "";
    }

    void SetWeaponVisibility(bool isVisible)
    {
        weaponVisible = isVisible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other;
            playerInCone = true;
            weaponManager = player.GetComponent<WeaponManager>();
            weaponManager.EnableWeaponChange.AddListener(SetWeaponVisibility);
            weaponVisible = weaponManager.IsEnabled;
        }
        else if (other.CompareTag("NPC") && other.gameObject != thisNPC) //stop NPCs listening to their own death
        {
            Hurtbox otherHurtbox = other.gameObject.GetComponent<Hurtbox>();
            otherHurtbox.onDie.AddListener(HandleNPCKilled);

            if (!otherHurtbox.IsAlive)
            {
                IncreaseSuspicionByFixedValue(SUSPICION_INCREASE_DEAD_NPC);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerInCone = false;
            weaponManager.EnableWeaponChange.RemoveListener(SetWeaponVisibility);
            weaponManager = null;
            weaponVisible = false;
        }
        else if (other.CompareTag("NPC") && other.gameObject != thisNPC) //dont listen for NPC death if not in cone
        {
            other.gameObject.GetComponent<Hurtbox>().onDie.RemoveListener(HandleNPCKilled);
        }
    }

    //this runs if an NPC in the cone is killed
    void HandleNPCKilled(GameObject npc)
    {
        //check dying NPC is visible, not just in cone
        Vector3 dyingNPCDirection = npc.transform.position - thisNPC.transform.position;
        if (Physics.Raycast(thisNPC.transform.position, dyingNPCDirection, out RaycastHit hit, Mathf.Infinity, npcLayerMask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.gameObject.Equals(npc))
            {
                float increase = SUSPICION_INCREASE_NPC_DIE;
                if (playerVisible || headVisible || chestVisible)
                {
                    increase += SUSPICION_INCREASE_PLAYER_KILL;
                }
                IncreaseSuspicionByFixedValue(increase);
            }
        }
    }

    void IncreaseSuspicionByFixedValue(float value)
    {
        Suspicion += value;
        ClampSuspicion();
    }
}
