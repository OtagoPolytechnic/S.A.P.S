using TMPro;
using UnityEngine;

//Base written by: Rohan Anakin

//this script should be attached to the NPC's vision cone object
public class VisionBehaviour : MonoBehaviour
{
    [Header("Suspicion")]
    private float suspicion = 0.0f;
    private float suspicionValue = 0.0f;
    private bool playerInCone = false;
    private bool playerVisible = false;
    private bool chestVisible = false;
    private bool headVisible = false;
    private bool weaponVisible = false;
    private bool playerFullySeen = false; //this will interact differently later to allow the NPC to call for help or flee
    private const float SUSPICION_MAX = 100f;
    private const float CHEST_SUSPICION_INCREASE = 2f;
    private const float HEAD_VISIBILITY_INCREASE = 1f;
    private const float WEAPON_VISIBILITY_INCREASE = 2f;
    private const float BASE_SUSPICION_INCREASE = 4f;
    private const float SUSPICION_DECAY_RATE = 4f;
    [SerializeField]
    private TextMeshPro suspicionText;
    private Collider player;
    private Camera playerCamera;
    private LayerMask layerMask;
    private WeaponManager weaponManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        suspicion = 0.0f;
        playerFullySeen = false;
        suspicionText.text = "";
        layerMask = LayerMask.GetMask("Player", "Default"); //default is every object created in the scene. If we make a layer for map geometry, we can switch Default to that layer
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerFullySeen) { return; } //for testing purposes

        if (!playerVisible || !playerInCone && suspicion > 0)
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
            if (suspicion >= SUSPICION_MAX)
            {
                playerFullySeen = true;
                //record the player's position
                //call for help
            }
        }
    }

    void CheckVisiblity()
    {
        Vector3 chestRayDirection = (player.transform.position + new Vector3(0,1,0)) - transform.position;
        Vector3 headRayDirection = (playerCamera.transform.position) - transform.position;
        //shoot a raycast to the CharacterController collider
        if (Physics.Raycast(transform.position, chestRayDirection, out RaycastHit hit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
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
        if (Physics.Raycast(transform.position, headRayDirection, out RaycastHit headHit, Mathf.Infinity, layerMask, QueryTriggerInteraction.Ignore))
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

    void IncreaseSuspicion() //these will also add other variables to the suspicion meter based on the player's actions
    {
        suspicion += suspicionValue * Time.deltaTime * (weaponVisible ? WEAPON_VISIBILITY_INCREASE : 1);
        suspicionText.text = suspicion.ToString("F0");
    }

    void DecreaseSuspicion()
    {
        suspicion -= SUSPICION_DECAY_RATE * Time.deltaTime;
        suspicionText.text = suspicion.ToString("F0");
        BottomLimitSuspicion();
    }

    void BottomLimitSuspicion() //error checking
    {
        if (suspicion <= 0)
        {
            suspicion = 0;
            suspicionText.text = "";
        }
    }

    void SetWeaponVisibility(bool isVisible)
    {
        weaponVisible = isVisible;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            player = other;
            playerInCone = true;
            weaponManager = player.GetComponent<WeaponManager>();
            weaponManager.EnableWeaponChange.AddListener(SetWeaponVisibility);
            weaponVisible = weaponManager.IsEnabled;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            player = null;
            playerInCone = false;
            weaponManager.EnableWeaponChange.RemoveListener(SetWeaponVisibility);
            weaponManager = null;
            weaponVisible = false;
        }
    }

}
