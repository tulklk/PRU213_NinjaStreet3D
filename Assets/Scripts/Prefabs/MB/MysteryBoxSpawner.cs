using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBoxSpawner : MonoBehaviour
{
    public GameObject mysteryBoxPrefab;
    public List<GroundTile> spawnableTiles;  

    private GameObject currentBoxInstance;

    public void SpawnMysteryBoxOnRandomTile()
    {
        if (mysteryBoxPrefab == null || spawnableTiles == null || spawnableTiles.Count == 0)
            return;

        // Chọn tile ngẫu nhiên trong danh sách
        GroundTile chosenTile = spawnableTiles[Random.Range(0, spawnableTiles.Count)];

        Collider tileCollider = chosenTile.GetComponent<Collider>();
        if (tileCollider == null)
            return;

        Bounds bounds = tileCollider.bounds;

        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);
        float y = bounds.max.y;

        Vector3 spawnPosition = new Vector3(randomX, y, randomZ);

        if (currentBoxInstance != null)
            Destroy(currentBoxInstance);

        currentBoxInstance = Instantiate(mysteryBoxPrefab, spawnPosition, Quaternion.identity);
    }
}

