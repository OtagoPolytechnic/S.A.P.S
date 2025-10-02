using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//this script is deprecated 


//Base written by: Rohan Anakin

/// <summary>
/// Manages the spawning of crowds in the scene. 
/// This system is deprecated and has been replaced by newer crowd management logic.
/// </summary
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
    /// Spawns a crowd at a specific spawn point. 
    /// Primarily used for testing via editor tools.
    /// </summary>
    /// <param name="spawnPointIndex">The index of the spawn point to use.</param>
    /// <param name="editorControlled">If true, skips automatic group spawning.</param>
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
    /// Spawns a crowd at all spawn points, excluding any indices provided.
    /// </summary>
    /// <param name="excludedSpawnPoints">Indices of spawn points to exclude.</param>
    public void SpawnAllCrowds(List<int> excludedSpawnPoints) //call this method, ignore the other one. Use the editor tool to spawn individual crowds if you need that
    {
        List<GameObject> includedSpawnPoints = new(spawnPoints);
        if (excludedSpawnPoints.Count > 0)
        {
            includedSpawnPoints = RemoveExcludedSpawnPoints(includedSpawnPoints, excludedSpawnPoints);
        }

        foreach (GameObject spawnPoint in includedSpawnPoints)
        {
            GameObject activeCrowd = Instantiate(crowd, spawnPoint.transform.position, Quaternion.identity);
            activeCrowd.transform.position = new Vector3(activeCrowd.transform.position.x, 0.75f, activeCrowd.transform.position.z);
            activeCrowd.GetComponentInChildren<CrowdSpawner>().SpawnGroup();
        }
    }

    /// <summary>
    /// Removes excluded spawn points by index.
    /// </summary>
    private List<GameObject> RemoveExcludedSpawnPoints(List<GameObject> includedSpawnPoints, List<int> excludedSpawnPoints)
    {
        List<GameObject> destroyedPoints = new();

        for (int i = 0; i < includedSpawnPoints.Count; i++)
        {
            if (excludedSpawnPoints.Contains(includedSpawnPoints.IndexOf(includedSpawnPoints[i])))
            {
                destroyedPoints.Add(includedSpawnPoints[i]);
            }
        }

        foreach (GameObject point in destroyedPoints)
        {
            includedSpawnPoints.Remove(point);
        }
        return includedSpawnPoints;
    }

}
