using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

//written by Rohan Anakin
/// <summary>
/// Handles the state of the tutorial and resetting the player.
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

    public void StartInit(List<GameObject> resetNPCs)
    {
        temp = resetNPCs;
        StartCoroutine(InitOnSceneLoad());
    }

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
    /// Intermediate handover method that eats the onDie call to properly call the correct method when NPCs die.
    /// </summary>
    /// <param name="obj">Not used but is here from the onDie call passing a game object</param>
    private void HandleNPCDie(GameObject obj = null)//object is not used but is required for the event
    {
        ResetStage(1);
    }
    /// <summary>
    /// Intermediate handover method that eats the arrest call to properly call the correct method when the player is arrested.
    /// </summary>
    /// <param name="obj">Not used but is needed for events to have listeners for some reason that is beyond my understanding right now</param>
    private void HandleArrest(GameObject obj = null)
    {
        ResetStage(2);
    }
    /// <summary>
    /// Resets the scene and sets the respawn to the correct place.
    /// </summary>
    /// <param name="stage">Where the player should be placed. Accepts 1 or 2 as valid numbers</param>
    public void ResetStage(int stage = 0)
    {
        this.stage = stage;
        StartCoroutine(WaitAsyncSceneLoad());
    }

    /// <summary>
    /// Waits asynchronously to reload the tutorial scene after the fade has fully enveloped the scene
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitAsyncSceneLoad()
    {
        yield return StartCoroutine(SceneLoader.Instance.Fade(1));
        yield return StartCoroutine(SceneLoader.Instance.LoadSceneAsync("Tutorial"));
        RespawnPlayerAtPoint();
        yield return StartCoroutine(SceneLoader.Instance.Fade(0));
    }

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
