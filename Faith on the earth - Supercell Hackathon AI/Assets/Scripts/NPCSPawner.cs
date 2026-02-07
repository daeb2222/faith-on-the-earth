using System.Collections.Generic;
using UnityEngine;

public class NPCSPawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform[] spawnLocations;
    private List<GameObject> spawnedNPCs = new List<GameObject>();

    public void SpawnNPC()
    {
        if (spawnLocations.Length == 0)
        {
            Debug.LogWarning("No spawn locations set!");
            return;
        }

        Transform spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)];

        GameObject newNPC = Instantiate(npcPrefab, spawnLocation.position, Quaternion.identity);
        spawnedNPCs.Add(newNPC);
        Debug.Log("NPC Spawned at: " + spawnLocation.position);
    }

    public void DestroyNPC()
    {
        if (spawnedNPCs.Count > 0)
        {
            GameObject npcToDestroy = spawnedNPCs[0];

            spawnedNPCs.RemoveAt(0);
            Destroy(npcToDestroy);
            Debug.Log("NPC Destroyed!");
        }
        else
        {
            Debug.Log("No NPCs to destroy.");
        }
    }

}
