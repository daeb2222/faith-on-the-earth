using System.Collections.Generic;
using UnityEngine;

public class NPCSPawner : MonoBehaviour
{

    public GameObject npcPrefab;  // El modelo del NPC que quieres spawnear
    public Transform[] spawnPlanes;  // Array de planos donde los NPCs pueden spawnear
    public float spawnAreaWidth = 10f;  // Ancho del área de spawn en cada plano
    public float spawnAreaHeight = 10f;  // Alto del área de spawn en cada plano

    private GameObject currentNPC;  // Referencia al NPC actual


   

           private List<GameObject> spawnedNPCs = new List<GameObject>();  // Lista para almacenar los NPCs instanciados

    // Método para spawnear un NPC en el plano
    public void SpawnNPC()
    {
        // Verificamos si hay al menos un plano asignado
        if (spawnPlanes.Length == 0)
        {
            Debug.LogWarning("No spawn planes set!");
            return;
        }

        // Seleccionamos aleatoriamente un plano de la lista de planos disponibles
        Transform selectedPlane = spawnPlanes[Random.Range(0, spawnPlanes.Length)];

        // Generamos coordenadas aleatorias dentro de ese plano
        float randomX = Random.Range(selectedPlane.position.x - spawnAreaWidth / 2, selectedPlane.position.x + spawnAreaWidth / 2);
        float randomZ = Random.Range(selectedPlane.position.z - spawnAreaHeight / 2, selectedPlane.position.z + spawnAreaHeight / 2);

        // La posición Y la tomamos directamente del plano (supongamos que el plano está a nivel Y = 0)
        Vector3 spawnPosition = new Vector3(randomX, selectedPlane.position.y, randomZ);

        // Generamos una rotación aleatoria en el eje Y (de 0 a 360 grados)
        float randomRotationY = Random.Range(0f, 360f);  // Aleatorio entre 0 y 360 grados

        // Creamos la rotación basada en el valor aleatorio para el eje Y
        Quaternion randomRotation = Quaternion.Euler(0f, randomRotationY, 0f);  // Solo rotación en Y

        // Spawneamos el NPC en la ubicación seleccionada con la rotación aleatoria
        GameObject newNPC = Instantiate(npcPrefab, spawnPosition, randomRotation);
        spawnedNPCs.Add(newNPC);  // Añadimos el nuevo NPC a la lista
        Debug.Log("NPC Spawned at: " + spawnPosition);
    }

    // Método para destruir un NPC aleatorio
    public void DestroyNPC()
    {
        if (spawnedNPCs.Count > 0)
        {
            // Seleccionamos un NPC aleatorio de la lista
            int randomIndex = Random.Range(0, spawnedNPCs.Count);
            GameObject npcToDestroy = spawnedNPCs[randomIndex];

            // Eliminamos el NPC de la lista
            spawnedNPCs.RemoveAt(randomIndex);

            // Destruimos el NPC
            Destroy(npcToDestroy);
            Debug.Log("NPC Destroyed!");
        }
        else
        {
            Debug.Log("No NPCs to destroy.");
        }
    }

}
