using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//Base written by: Rohan Anakin
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
        List<GameObject> includedSpawnPoints = new(spawnPoints);
        if (excludedSpawnPoints.Count > 0)
        { 
            includedSpawnPoints = RemoveExcludedSpawnPoints(includedSpawnPoints ,excludedSpawnPoints);
        }

        foreach (GameObject spawnPoint in includedSpawnPoints)
        {
            Debug.Log(spawnPoint.name);
            GameObject activeCrowd = Instantiate(crowd, spawnPoint.transform.position, Quaternion.identity);
            activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
            activeCrowd.GetComponentInChildren<CrowdSpawner>().SpawnGroup();
        }
    }

    private List<GameObject> RemoveExcludedSpawnPoints(List<GameObject> includedSpawnPoints, List<int> excludedSpawnPoints)
    {
        List<GameObject> destroyedPoints = new();

        for (int i = 0; i < includedSpawnPoints.Count; i++)
        {
            for (int j = 0; j < excludedSpawnPoints.Count; j++)
            {
                if (excludedSpawnPoints[j] == includedSpawnPoints.IndexOf(includedSpawnPoints[i]))
                {
                    destroyedPoints.Add(includedSpawnPoints[i]);
                    
                }
            } 
        }

        foreach (GameObject point in destroyedPoints)
        {
            includedSpawnPoints.Remove(point);
        }
        return includedSpawnPoints;
    }

}
