using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public Transform[] spawnPoints;

    public float minSpawnDelay = 2f;
    public float maxSpawnDelay = 5f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnVehicle();

            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);
        }
    }

    void SpawnVehicle()
    {
        int laneIndex = Random.Range(0, spawnPoints.Length);
        int vehicleIndex = Random.Range(0, vehiclePrefabs.Length);

        GameObject vehicle = Instantiate(vehiclePrefabs[vehicleIndex], spawnPoints[laneIndex].position, Quaternion.identity);
        vehicle.AddComponent<ObstacleMove>();
        vehicle.GetComponent<ObstacleMove>().speed = Random.Range(7f, 12f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GroundSpawner.Instance.SpawnTile();
            gameObject.SetActive(false);
        }
    }
}
