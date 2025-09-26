using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTileRacingSpawn : MonoBehaviour
{
    
    [Header("The Vehicles have from 4 wheels")]
    //Xanh
    //Forwad
    public GameObject xanhForwardPrefabs;
    public Transform[] spawnPointsXanhsForward;
    //BackWard
    public GameObject xanhBackwardPrefabs;
    public Transform[] spawnPointsXanhsBackward;

    //KIA
    //Forward
    public GameObject kiaForwardPrefabs;
    public Transform[] spawnPointsKIAForward;
    //Backward
    public GameObject kiaBackwardPrefabs;
    public Transform[] spawnPointsKIABackward;

    //BatCoc
    //Forward
    public GameObject xeBatCocForwardPrefabs;
    public Transform[] spawnPointsXeBatCocForward;
    //Backward
    public GameObject xebatcocBackwardPrefabs;
    public Transform[] spawnPointsXeBatCocBackward;

    //Truck
    //Forward
    public GameObject truckForwardPrefabs;
    public Transform[] spawnPointsTruckForward;
    //Backward
    public GameObject truckBackwardPrefabs;
    public Transform[] spawnPointsTruckBackward;

    //Bus
    //Forward
    public GameObject busForwardPrefabs;
    public Transform[] spawnPointsBusForward;
    //Backward
    public GameObject busBackwardPrefabs;
    public Transform[] spawnPointsBusBackward;


    [Header("Motor")]
    //Vision
    //Forward
    public GameObject visionForwardPrefabs;
    public Transform[] spawnPointsVisionForward;
    //Right
    public GameObject visionRightForwardPrefabs;
    public Transform[] spawnPointsVisionRightForward;
    //Left
    public GameObject visionLeftForwardPrefabs;
    public Transform[] spawnPointsVisionLeftForward;
    //Backward
    public GameObject visionBackwardPrefabs;
    public Transform[] spawnPointsVisionBackward;

    //Shoppee
    //Forward
    public GameObject shoppeeForwardPrefabs;
    public Transform[] spawnPointsShoppeeForward;
    //Backward
    public GameObject shoppeeBackwardPrefabs;
    public Transform[] spawnPointsShoppeeBackward;

    [Header("Coin")]
    //Coin
    public GameObject coinPrefab;
    public Transform[] coinSpawnPoints;
    public int numberOfCoins = 5;
    public float coinSpacingZ = 3f;
    //private int coinLaneIndex = -1;

    [Header("MysteryBox")]
    //Mystery Box
    public GameObject mysteryBoxPrefab;
    public Transform[] spawnMysteryPoints;


    private void OnEnable()
    {
        //Coins
        SpawnCoins();
        //Motor
        SpawnVisionForward();
        SpawnVisionRightForward();
        SpawnVisionLeftForward();
        SpawnVisionBackWard();
        SpawnShoppeeForward();
        //Vehicles with 4 wheels
        //Xanh
        SpawnXanhForward();
        SpawnXanhBackWard();
        //KIA
        SpawnKIAForward();
        SpawnKIABackward();
        //Xe Bat Coc
        SpawnXeBatCocForward();
        SpawnXeBatCocBackward();
        //Truck
        SpawnTruckBackward();
        //Bus
        SpawnBusBackward();
        SpawnBusForward();
        //Mystery Box
        SpawnMysteryBox();
        //Spawn RacingBoy

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🟨 Player enter tile: {gameObject.name}");
            //RacingBoySpawner.Instance.ScheduleSpawn(transform);
            if (RacingBoySpawner.Instance != null && RacingBoySpawner.Instance.gameObject != null && RacingBoySpawner.Instance.gameObject.activeInHierarchy)
            {
                RacingBoySpawner.Instance.ScheduleSpawn(transform);
            }
            else
            {
                Debug.LogWarning("⚠️ RacingBoySpawner đã bị huỷ hoặc không tồn tại!");
            }


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"🟨 Player exited tile: {gameObject.name}");
            StartCoroutine(DisableTileWhenInvisible());
            

        }
    }
    //Hàm xóa tile khi tile ra khỏi player một khoảng



    private IEnumerator DisableTileWhenInvisible()
    {
        Camera cam = Camera.main;
        if (cam == null) yield break;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) yield break;

        float checkInterval = 0.5f;

        while (true)
        {
            bool isVisible = false;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

            foreach (Renderer rend in renderers)
            {
                if (rend != null && GeometryUtility.TestPlanesAABB(planes, rend.bounds))
                {
                    isVisible = true;
                    break;
                }
            }

            // Nếu không còn nhìn thấy và không drift thì tắt
            if (!isVisible)
            {
                GroundSpawner.Instance.SpawnTile();
                gameObject.SetActive(false);
                yield break;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    //Spawn coin
    void SpawnCoins()
    {
        if (coinPrefab == null || coinSpawnPoints.Length == 0) return;

        foreach (Transform lane in coinSpawnPoints)
        {
            Vector3 startPos = lane.position;

            for (int i = 0; i < numberOfCoins; i++)
            {
                Vector3 spawnPos = startPos + new Vector3(0, 0.5f, i * coinSpacingZ);
                GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity, transform);
                coin.tag = "Coin";
            }
        }

        //Debug.Log("[CoinSpawner] Spawned coins on all assigned lanes.");
    }

    //Vehicle from 4 wheels 
    //Xanh
    void SpawnXanhForward()
    {
        if (xanhForwardPrefabs == null || spawnPointsXanhsForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsXanhsForward)
        {
            Instantiate(xanhForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }

    void SpawnXanhBackWard()
    {
        if (xanhBackwardPrefabs == null || spawnPointsXanhsBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsXanhsBackward)
        {
            Instantiate(xanhBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }


    }
    //KIA
    void SpawnKIAForward()
    {
        if (kiaForwardPrefabs == null || spawnPointsKIAForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsKIAForward)
        {
            Instantiate(kiaForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnKIABackward()
    {
        if (kiaBackwardPrefabs == null || spawnPointsKIABackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsKIABackward)
        {
            Instantiate(kiaBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    //Xe Bat Coc
    void SpawnXeBatCocBackward()
    {
        if (xebatcocBackwardPrefabs == null || spawnPointsXeBatCocBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsXeBatCocBackward)
        {
            Instantiate(xebatcocBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    //Truck
    void SpawnTruckBackward()
    {
        if (truckBackwardPrefabs == null || spawnPointsTruckBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsTruckBackward)
        {
            Instantiate(truckBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, -90f, 0), transform);
        }

    }
    void SpawnXeBatCocForward()
    {
        if (xeBatCocForwardPrefabs == null || spawnPointsXeBatCocForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsXeBatCocForward)
        {
            Instantiate(xeBatCocForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    //Bus
    void SpawnBusForward()
    {
        if (busForwardPrefabs == null || spawnPointsBusForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsBusForward)
        {
            Instantiate(busForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnBusBackward()
    {
       
        if (busBackwardPrefabs == null || spawnPointsBusBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsBusBackward)
        {
            Instantiate(busBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }



    //Motor
    void SpawnVisionForward()
    {
        if (visionForwardPrefabs == null || spawnPointsVisionForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsVisionForward)
        {
            Instantiate(visionForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnVisionRightForward()
    {
        if (visionRightForwardPrefabs == null || spawnPointsVisionRightForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsVisionRightForward)
        {
            Instantiate(visionRightForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnVisionLeftForward()
    {
        if (visionLeftForwardPrefabs == null || spawnPointsVisionLeftForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsVisionLeftForward)
        {
            Instantiate(visionLeftForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnVisionBackWard()
    {
        if (visionBackwardPrefabs == null || spawnPointsVisionBackward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsVisionBackward)
        {
            Instantiate(visionBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, 180f, 0), transform);
        }
    }
    //Shoppee
    void SpawnShoppeeForward()
    {
        if (shoppeeForwardPrefabs == null || spawnPointsShoppeeForward.Length == 0) return;

        foreach (Transform spawnPoint in spawnPointsShoppeeForward)
        {
            Instantiate(shoppeeForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnMysteryBox()
    {
        if (mysteryBoxPrefab == null || spawnMysteryPoints.Length == 0) return;

        foreach (Transform spawnPoint in spawnMysteryPoints)
        {
            Instantiate(mysteryBoxPrefab, spawnPoint.position, Quaternion.identity, transform);
        }


    }
    public void ResetTile()
    {
        foreach (Transform child in transform)
        {
            if (child != transform &&
                !child.name.Contains("spawn", System.StringComparison.OrdinalIgnoreCase) &&
                !child.name.Contains("NextSpawnPoint") &&
                !child.name.Contains("Ground") &&
                !child.name.Contains("SideWalk") &&
                !child.name.Contains("SideWalk1") &&
                !child.name.Contains("SideWalk2") &&
                !child.name.Contains("Condo") &&
                !child.name.Contains("Convang") &&
                !child.name.Contains("Conxanh") &&
                !child.name.Contains("Doc") &&
                !child.name.Contains("Doc1") &&
                !child.name.Contains("Doc2") &&
                !child.name.Contains("MysteryBox") &&
                !child.name.Contains("NitroBoost") &&
                !child.name.Contains("Magnet") &&
                !child.name.Contains("Rocket") &&
                !child.name.Contains("Shield")


                )

            {
                Destroy(child.gameObject);
            }
        }

        //Coin
        SpawnCoins();
        //Motor
        SpawnVisionForward();
        SpawnVisionRightForward();
        SpawnVisionLeftForward();
        SpawnVisionBackWard();
        SpawnShoppeeForward();
        //Vehicles with 4 wheels
        SpawnXanhForward();
        SpawnXanhBackWard();
        SpawnKIAForward();
        SpawnKIABackward();
        SpawnXeBatCocForward();
        SpawnXeBatCocBackward();
        SpawnTruckBackward();
        SpawnBusForward();
        SpawnBusBackward();
        SpawnMysteryBox();
        RacingBoySpawner.Instance.ScheduleSpawn(transform);
    }

}
