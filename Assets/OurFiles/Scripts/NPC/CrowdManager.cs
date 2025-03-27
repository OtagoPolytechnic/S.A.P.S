using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>CrowdManager</c> is used to manage the spawning of crowds in the scene.
/// </summary>
public class CrowdManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> spawnPoints = new List<GameObject>();

    [SerializeField]
    private GameObject crowd;
    [SerializeField]
    private bool spawnAllCrowdsOnStart = false;
    void Start()
    {
        if (spawnAllCrowdsOnStart)
        {
            SpawnAllCrowds(new List<int>());
        }
    }

    /// <summary>
    /// Method <c>SpawnIndividualCrowd</c> spawns a crowd at a specific spawn point.
    /// <para>
    /// This method should mainly be used by the Editor tool to spawn a crowd at a specific spawn point for testing.
    /// </para>
    /// </summary>
    public void SpawnIndividualCrowd(int spawnPointIndex = 0, bool editorControlled = false) //call if you need to spawn a crowd at a specific spawn point
    {
        if (spawnPointIndex > spawnPoints.Count)
        {
            spawnPointIndex = spawnPoints.Count - 1;
        }
        GameObject activeCrowd = Instantiate(crowd, spawnPoints[spawnPointIndex].transform.position, Quaternion.identity);
        activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
        if (!editorControlled)
        {
            activeCrowd.GetComponentInChildren<CrowdSpawner>().SpawnGroup();
        }
 
    }
    /// <summary>
    /// Method <c>SpawnAllCrowds</c> spawns a crowd with 7 max people at all available spawn points. 
    /// <para>
    /// This is able to exclude spawn points by passing a list of integers that represent the index of the spawn point to exclude.
    /// </para>
    /// </summary>
    public void SpawnAllCrowds(List<int> excludedSpawnPoints) //call this method, ignore the other one. Use the editor tool to spawn individual crowds if you need that
    {
        int i = 0;
        foreach (GameObject spawnPoint in spawnPoints)
        {
            if (excludedSpawnPoints.Count > 0)
            {
                if (excludedSpawnPoints[i] == spawnPoints.IndexOf(spawnPoint))
                {
                    continue;
                }
            }
            GameObject activeCrowd = Instantiate(crowd, spawnPoint.transform.position, Quaternion.identity);
            activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
            activeCrowd.GetComponentInChildren<CrowdSpawner>().SpawnGroup();
            i++;
        }
    }

}
