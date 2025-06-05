using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void HandleNPCDie(GameObject obj = null)//object is not used but is required for the event
    {
        print("Calling reset stage");
        ResetStage(1);
    }

    public void ResetStage(int stage = 0)
    {
        this.stage = stage;
        StartCoroutine(WaitAsyncSceneLoad());
    }

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
