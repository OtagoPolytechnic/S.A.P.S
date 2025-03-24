using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawner : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> spawnPoints = new List<GameObject>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void SpawnIndividualCrowd(int spawnPointIndex = 0)
    {
        if (spawnPointIndex > spawnPoints.Count)
        {
            spawnPointIndex = spawnPoints.Count - 1;
        }
        GameObject activeCrowd = (GameObject)Instantiate(Resources.Load("Crowd"), spawnPoints[spawnPointIndex].transform.position, Quaternion.identity);
        activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
        activeCrowd.GetComponentInChildren<CrowdManager>().SpawnGroup();
    }

    public void SpawnAllCrowds()
    {
        foreach (GameObject spawnPoint in spawnPoints)
        {
            GameObject activeCrowd = (GameObject)Instantiate(Resources.Load("Crowd"), spawnPoint.transform.position, Quaternion.identity);
            activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
            activeCrowd.GetComponentInChildren<CrowdManager>().SpawnGroup();
        }
    }

}
