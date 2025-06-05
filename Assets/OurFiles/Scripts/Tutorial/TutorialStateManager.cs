using System.Collections;
using UnityEngine;

public class TutorialStateManager : Singleton<TutorialStateManager>
{
    [SerializeField]
    Transform resetSpot1;
    [SerializeField]
    Transform resetSpot2;
    [SerializeField]
    GameObject player;
    int stage = 0;
    bool onFaded;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        SceneLoader.Instance.faded.AddListener(() => onFaded = true);
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
