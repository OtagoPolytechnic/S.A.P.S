using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// Base written by: Rohan Anakin
// Edited by: Jenna Boyes

/// <summary>
/// Vision cone logic for NPCs. Tracks whether the player (and weapons/deaths)
/// are visible, builds/decays suspicion, and triggers NPC panic at max suspicion.
/// Attach to the NPC's vision-cone object.
/// </summary>
public class VisionBehaviour : MonoBehaviour
{
    [Header("Suspicion")]
    private float suspicion;

    /// <summary>
    /// Current suspicion (0–100). Setting this will trigger Panic when it reaches max (unless tutorial rules say otherwise).
    /// </summary>
    public float Suspicion
    {
        get => suspicion;
        set => SetSuspicion(value);
    }

    private float suspicionValue = 0.0f;
    private bool isGuard = false;
    private bool isTutorial = false;
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
    private const float GUARD_SUSPICION_MULTIPLIER = 3f; //Guards gain suspicion faster than other NPCs
    private const float TUTORIAL_GUARD_SUSPICION_MULTIPLIER = 8f;

    [SerializeField] private TextMeshPro suspicionText;
    [SerializeField] private MeshCollider visionCone;

    private Collider player;
    private Camera playerCamera;
    private LayerMask playerLayerMask;
    private LayerMask npcLayerMask;
    private GameObject thisNPC; //the NPC that this vision cone is attached to 
    private WeaponManager weaponManager;
    private NPCPather npcPather;
    public bool isTutorialGuard;
    public bool hasSeenWeapon; //should never be set false in code

    private NPCExpressionController expression;

    void Start()
    {
        npcPather = GetComponentInParent<NPCPather>();
        expression = GetComponentInParent<NPCExpressionController>();
        if (expression == null) expression = GetComponentInChildren<NPCExpressionController>(true);
        Suspicion = SUSPICION_MIN;
        playerFullySeen = false;
        if (suspicionText) suspicionText.text = "";
        npcLayerMask = LayerMask.GetMask("NPC", "Default", "Geometry");
        playerLayerMask = LayerMask.GetMask("Player", "Default", "Geometry");
        playerCamera = PlayerReferences.Instance.MainCamera;
        thisNPC = gameObject.GetComponentInParent<Hurtbox>().gameObject;

        if (gameObject.GetComponent<GuardLeader>() != null || gameObject.GetComponent<GuardFollower>() != null)
        {
            isGuard = true;
        }

        if (SceneManager.GetActiveScene().name == "Tutorial")
        {
            isTutorial = true;
        }
    }

    void Update()
    {
        if (playerFullySeen) return; //for testing purposes

        // decay when player isn’t being seen
        if ((!playerVisible || !playerInCone) && Suspicion > SUSPICION_MIN)
        {
            Suspicion -= SUSPICION_DECAY_RATE * Time.deltaTime;
        }

        if (!isTutorial && CoherencyBehaviour.Instance.Coherent && !playerFullySeen)
        {
            playerVisible = false;
            return;
        }

        if (playerInCone)
        {
            CheckVisiblity();
            if (Suspicion >= SUSPICION_MAX)
            {
                playerFullySeen = true;
                // record the player's position / call for help (future)
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
        if (visionCone) visionCone.enabled = isEnabled;
        if (suspicionText) suspicionText.gameObject.SetActive(isEnabled);
    }

    /// <summary>
    /// Casts rays to the player's chest and head to decide visibility and how quickly suspicion should rise.
    /// Also boosts suspicion if a weapon is visible or the NPC is a guard.
    /// </summary>
    void CheckVisiblity()
    {
        Vector3 chestRayDirection = (player.transform.position + new Vector3(0,1,0)) - transform.position;
        Vector3 headRayDirection  = (playerCamera.transform.position) - transform.position;

        // chest ray
        if (Physics.Raycast(transform.position, chestRayDirection, out RaycastHit hit, Mathf.Infinity, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            chestVisible = hit.collider.gameObject.Equals(player != null ? player.gameObject : null);
            Debug.DrawLine(transform.position, hit.point, chestVisible ? Color.green : Color.red);
        }

        // head ray
        if (Physics.Raycast(transform.position, headRayDirection, out RaycastHit headHit, Mathf.Infinity, playerLayerMask, QueryTriggerInteraction.Ignore))
        {
            headVisible = headHit.collider.gameObject.Equals(playerCamera != null ? playerCamera.gameObject : null);
            Debug.DrawLine(transform.position, headHit.point, headVisible ? Color.green : Color.red);
        }

        // visibility → base suspicion rate
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

        // only gain suspicion when we *should* (seen weapon OR guard OR tutorial guard)
        if (playerVisible && (hasSeenWeapon || isGuard || isTutorialGuard))
        {
            float mult =
                (weaponVisible ? WEAPON_VISIBILITY_INCREASE : 1f) *
                (isGuard ? GUARD_SUSPICION_MULTIPLIER : 1f) *
                (isTutorialGuard ? TUTORIAL_GUARD_SUSPICION_MULTIPLIER : 1f);

            Suspicion += suspicionValue * mult * Time.deltaTime;
        }
    }

    // central setter
    void SetSuspicion(float value)
    {
        suspicion = Mathf.Clamp(value, SUSPICION_MIN, SUSPICION_MAX);

        if (suspicionText)
            suspicionText.text = suspicion > SUSPICION_MIN ? suspicion.ToString("F0") : "";

        // Drive expressions *only* via the level (Neutral/Scared thresholds handled inside)
        expression?.UpdateSuspicionLevel(suspicion);

        // Only state/event change here; don't directly set faces.
        if ((isTutorialGuard || !isTutorial) && suspicion >= SUSPICION_MAX)
        {
            if (npcPather.State != NPCPather.NPCState.Panic)
                npcPather.State = NPCPather.NPCState.Panic;

            NPCEventManager.Instance?.onPanic.Invoke(thisNPC);
        }
    }

    /// <summary>
    /// Sets whether the weapon is currently visible.
    /// If visible while the player is visible, permanently flags that the weapon was seen.
    /// </summary>
    /// <param name="isVisible">True if the weapon is visible; otherwise false.</param>
    void SetWeaponVisibility(bool isVisible)
    {
        weaponVisible = isVisible;
        if (weaponVisible && playerVisible)
        {
            hasSeenWeapon = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other;
            playerInCone = true;
            weaponManager = player.GetComponent<WeaponManager>();
            weaponManager.EnableWeaponChange.AddListener(SetWeaponVisibility);
            SetWeaponVisibility(weaponManager.IsEnabled);
        }
        else if (other.CompareTag("NPC") && other.gameObject != thisNPC) // stop listening to our own death
        {
            Hurtbox otherHurtbox = other.gameObject.GetComponent<Hurtbox>();
            otherHurtbox.onDie.AddListener(HandleNPCKilled);

            if (!otherHurtbox.IsAlive)
            {
                // just add suspicion; do NOT directly set an expression here
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
            if (weaponManager != null)
            {
                weaponManager.EnableWeaponChange.RemoveListener(SetWeaponVisibility);
                weaponManager = null;
            }
            weaponVisible = false;
        }
        else if (other.CompareTag("NPC") && other.gameObject != thisNPC)
        {
            other.gameObject.GetComponent<Hurtbox>().onDie.RemoveListener(HandleNPCKilled);
        }
    }

    /// <summary>
    /// Called when an NPC in the cone dies.
    /// Increases suspicion if the corpse is visible and adds an extra amount if the player is visible.
    /// </summary>
    /// <param name="npc">The NPC GameObject that died.</param>
    void HandleNPCKilled(GameObject npc)
    {
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

    /// <summary>
    /// Adds suspicion instantly by a fixed value, then clamps it to the valid range.
    /// </summary>
    /// <param name="value">Amount of suspicion to add before clamping.</param>
    void IncreaseSuspicionByFixedValue(float value)
    {
        Suspicion += value;
        Suspicion = Mathf.Clamp(Suspicion, SUSPICION_MIN, SUSPICION_MAX);
    }
}
