using UnityEngine;

public class NPCSPawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public Transform[] spawnLocations;
    private GameObject currentNPC;

    public void SpawnNPC()
    {
        if (spawnLocations.Length == 0)
        {
            Debug.LogWarning("No spawn locations set!");
            return;
        }

        Transform spawnLocation = spawnLocations[Random.Range(0, spawnLocations.Length)];

        Instantiate(npcPrefab, spawnLocation.position, Quaternion.identity);
        Debug.Log("NPC Spawned at: " + spawnLocation.position);
    }

    public void DestroyNPC()
    {
        if (currentNPC != null)
        {
            Destroy(currentNPC);
            currentNPC = null;
            Debug.Log("NPC Destroyed!");
        }
    }
}
