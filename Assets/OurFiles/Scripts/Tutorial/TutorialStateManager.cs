using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//written by Rohan Anakin
/// <summary>
/// Handles the state of the tutorial and resetting the player.
/// </summary>
public class TutorialStateManager : Singleton<TutorialStateManager>
{
    int stage = 0;
    bool onFaded;
    public List<GameObject> resetTargets = new();

    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneLoader.Instance.faded.AddListener(() => onFaded = true);
        yield return null;
        foreach (GameObject npc in resetTargets)
        {
            npc.GetComponent<Hurtbox>().onDie.AddListener(HandleNPCDie);
        }
    }
    /// <summary>
    /// Intermediate handover method that eats the onDie call to properly call the correct method when NPCs die.
    /// </summary>
    /// <param name="obj">Not used but is here from the onDie call passing a game object</param>
    private void HandleNPCDie(GameObject obj = null)//object is not used but is required for the event
    {
        print("Calling reset stage");
        ResetStage(1);
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
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync("Tutorial"));
        yield return new WaitUntil(() => onFaded);
        onFaded = false;
        GameObject player = GameObject.Find("Player");
        Transform resetSpot1 = GameObject.Find("Room (3)/PlayerRespawn").transform;
        Transform resetSpot2 = GameObject.Find("Ending Room/PlayerRespawn").transform;
        if (stage == 1)
        {
            player.transform.position = resetSpot1.position;
            player.transform.rotation = resetSpot1.rotation;
        }
        if (stage == 2)
        {
            player.transform.position = resetSpot2.position;
            player.transform.rotation = resetSpot2.rotation;
        }
    }
}
