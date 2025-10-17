using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//written by Rohan Anakin
/// <summary>
/// Tutorial flow coordinator.
/// - Subscribes to NPC death and guard arrest signals
/// - Fades, reloads the Tutorial scene, and respawns the player at the correct checkpoint
/// - Persists across scene loads
/// </summary>
public class TutorialStateManager : Singleton<TutorialStateManager>
{
    int stage = 0;
    bool onFaded;
    public List<GameObject> resetTargets = new();
    List<GameObject> temp = new();

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Entry point from the spawner: seeds the NPC list and begins post-load binding.
    /// </summary>
    /// <param name="resetNPCs">NPCs whose death should reset the tutorial.</param>
    public void StartInit(List<GameObject> resetNPCs)
    {
        temp = resetNPCs;
        StartCoroutine(InitOnSceneLoad());
    }
    /// <summary>
    /// Copies the provided NPC list into <see cref="resetTargets"/> and wires event listeners.
    /// </summary>
    IEnumerator InitOnSceneLoad()
    {
        resetTargets.Clear();
        foreach (GameObject obj in temp)
        {
            resetTargets.Add(obj);
        }
        
        yield return null;

        foreach (GameObject npc in resetTargets)
        {
            npc.GetComponent<Hurtbox>().onDie.AddListener(HandleNPCDie);
        }
        TutorialSpawner.Instance.GuardArrest.AddListener(HandleArrest);
    }

    /// <summary>
    /// Forwards NPC death into a stage reset.
    /// </summary>
    /// <param name="obj">Unused (required by event signature).</param>
    private void HandleNPCDie(GameObject obj = null)//object is not used but is required for the event
    {
        ResetStage(1);
    }

    /// <summary>
    /// Forwards guard arrest into a stage reset.
    /// </summary>
    /// <param name="obj">Unused (required by event signature).</param>
    private void HandleArrest(GameObject obj = null)
    {
        ResetStage(2);
    }

    /// <summary>
    /// Triggers a fade → async reload of the Tutorial scene → respawn at the requested checkpoint.
    /// </summary>
    /// <param name="stage">Checkpoint index (1 or 2). Defaults to 0 = no move.</param>
    public void ResetStage(int stage = 0)
    {
        this.stage = stage;
        StartCoroutine(WaitAsyncSceneLoad());
    }

    /// <summary>
    /// Waits for fade, reloads the scene asynchronously, respawns, then fades back in.
    /// </summary>
    IEnumerator WaitAsyncSceneLoad()
    {
        yield return StartCoroutine(SceneLoader.Instance.Fade(1));
        yield return StartCoroutine(SceneLoader.Instance.LoadSceneAsync("Tutorial"));
        RespawnPlayerAtPoint();
        yield return StartCoroutine(SceneLoader.Instance.Fade(0));
    }

    /// <summary>
    /// Moves the player to the appropriate checkpoint based on <see cref="stage"/>.
    /// </summary>
    void RespawnPlayerAtPoint()
    {
        GameObject player = GameObject.Find("Player");
        Transform resetSpot1 = GameObject.Find("Room (3)/PlayerRespawn").transform;
        Transform resetSpot2 = GameObject.Find("Ending Room/PlayerRespawn").transform;
        if (stage == 1)
        {
            player.transform.SetPositionAndRotation(resetSpot1.position, resetSpot1.rotation);
        }
        if (stage == 2)
        {
            player.transform.SetPositionAndRotation(resetSpot2.position, resetSpot2.rotation);
        }
    }
}
