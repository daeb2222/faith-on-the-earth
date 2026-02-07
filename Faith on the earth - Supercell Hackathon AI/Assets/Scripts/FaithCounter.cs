using UnityEngine;
using System.Collections;

public class FaithCounter : MonoBehaviour
{
    public float checkInterval = 1f;
    public int spawnThreshold = 50;
    public float timeAboveThreshold = 10f;
    public float timeBelowThreshold = 10f;
    private float timeAbove = 0f;
    private float timeBelow = 0f;
    private NPCSPawner npcSpawner;
    private int currentFaith;

    void Start()
    {
        npcSpawner = FindObjectOfType<NPCSPawner>();
        StartCoroutine(CheckFaithStatus());
    }

    IEnumerator CheckFaithStatus()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            if (currentFaith > spawnThreshold)
            {
                timeAbove += checkInterval;
                timeBelow = 0f;

                if (timeAbove >= timeAboveThreshold)
                {
                    npcSpawner.SpawnNPC();
                    timeAbove = 0f;
                    Debug.Log("Nuevo NPC spawned por fe alta.");
                }
            }
            else
            {
                timeBelow += checkInterval;
                timeAbove = 0f;

                if (timeBelow >= timeBelowThreshold)
                {
                    npcSpawner.DestroyNPC();
                    timeBelow = 0f;
                    Debug.Log("NPC destroyed por fe baja.");
                }
            }
        }
    }

    public void UpdateFaith(int newFaith)
    {
        currentFaith = newFaith;
    }
}
