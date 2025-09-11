using System.Collections;
using UnityEngine;

public class GroundTile : MonoBehaviour
{
    [Header("The Vehicles have from 4 wheels")]
    //Xanh
    [Header("Xanh")]
    //Forwad
    public GameObject xanhForwardPrefabs;
    public Transform[] spawnPointsXanhsForward;
    //BackWard
    public GameObject xanhBackwardPrefabs;
    public Transform[] spawnPointsXanhsBackward;
    [Header("KIA")]
    //KIA
    //Forward
    public GameObject kiaForwardPrefabs;
    public Transform[] spawnPointsKIAForward;
    //Backward
    public GameObject kiaBackwardPrefabs;
    public Transform[] spawnPointsKIABackward;
    [Header("Carnival")]
    //Carnival
    //Forward
    public GameObject carnivalForwardPrefabs;
    public Transform[] spawnPointsCarnivalForward;
    //Backward
    public GameObject carnivalBackwardPrefabs;
    public Transform[] spawnPointsCarnivalBackward;
    [Header("Toyota")]
    //Toyota
    //Forward
    public GameObject toyotaForwardPrefabs;
    public Transform[] spawnPointsToyotaForward;
    //Backward
    public GameObject toyotaBackwardPrefabs;
    public Transform[] spawnPointsToyotaBackward;

    [Header("BatCoc")]
    //BatCoc
    //Forward
    public GameObject xeBatCocForwardPrefabs;
    public Transform[] spawnPointsXeBatCocForward;
    //Backward
    public GameObject xebatcocBackwardPrefabs;
    public Transform[] spawnPointsXeBatCocBackward;
    [Header("Truck")]
    //Truck
    //Forward
    public GameObject truckForwardPrefabs;
    public Transform[] spawnPointsTruckForward;
    //Backward
    public GameObject truckBackwardPrefabs;
    public Transform[] spawnPointsTruckBackward;
    [Header("Blue Bus")]
    //Blue Bus
    //Forward
    public GameObject blueBusForwardPrefabs;
    public Transform[] spawnPointsBlueBusForward;
    //Backward
    public GameObject blueBusBackwardPrefabs;
    public Transform[] spawnPointsBlueBusBackward;
    [Header("Green Bus")]
    //Green Bus
    //Forward
    public GameObject greenBusForwardPrefabs;
    public Transform[] spawnPointsGreenBusForward;
    //Backward
    public GameObject greenBusBackwardPrefabs;
    public Transform[] spawnPointsGreenBusBackward;


    [Header("Motor")]
    //Vision
    [Header("Vision")]
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
    [Header("Shoppee")]
    //Shoppee
    //Forward
    public GameObject shoppeeForwardPrefabs;
    public Transform[] spawnPointsShoppeeForward;
    //Backward
    public GameObject shoppeeBackwardPrefabs;
    public Transform[] spawnPointsShoppeeBackward;
    [Header("Wave")]
    //Wave
    //Forward
    public GameObject waveForwardPrefabs;
    public Transform[] spawnPointsWaveForward;
    //Backward
    public GameObject waveBackwardPrefabs;
    public Transform[] spawnPointsWaveBackward;

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
        SpawnShoppeeBackward();
        SpawnWaveForward();
        SpawnWaveBackward();

        //Vehicles with 4 wheels
        //Xanh
        SpawnXanhForward();
        SpawnXanhBackWard();
        //KIA
        SpawnKIAForward();
        SpawnKIABackward();
        //Carnival
        SpawnCarnivalForward();
        SpawnCarnivalBackward();
        //Xe Bat Coc
        SpawnXeBatCocForward();
        SpawnXeBatCocBackward();
        //Truck
        SpawnTruckBackward();
        //Bus
        SpawnBlueBusForward();
        SpawnGreenBusForward();
        SpawnBlueBusBackward();
        SpawnGreenBusBackward();
        //Toyota
        SpawnToyotaForward();
        SpawnToyotaBackward();

        //Mystery Box
        SpawnMysteryBox();


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
            Instantiate(xanhBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, 180f, 0), transform);
        }


    }
    //Toyota
    void SpawnToyotaForward()
    {
        if (toyotaForwardPrefabs == null || spawnPointsToyotaForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsToyotaForward)
        {
            Instantiate(toyotaForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnToyotaBackward()
    {
        if (toyotaBackwardPrefabs == null || spawnPointsToyotaBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsToyotaBackward)
        {
            Instantiate(toyotaBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
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
    //Carnival
    void SpawnCarnivalForward()
    {
        if (carnivalForwardPrefabs == null || spawnPointsCarnivalForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsCarnivalForward)
        {
            Instantiate(carnivalForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnCarnivalBackward()
    {
        if (carnivalBackwardPrefabs == null || spawnPointsCarnivalBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsCarnivalBackward)
        {
            Instantiate(carnivalBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }

    //Xe Bat Coc
    void SpawnXeBatCocBackward()
    {
        if (xebatcocBackwardPrefabs == null || spawnPointsXeBatCocBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsXeBatCocBackward)
        {
            Instantiate(xebatcocBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, 180f, 0), transform);
        }
    }
    //Truck
    void SpawnTruckBackward()
    {
        if (truckBackwardPrefabs == null || spawnPointsTruckBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsTruckBackward)
        {
            Instantiate(truckBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
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
    void SpawnBlueBusForward()
    {
        if (blueBusForwardPrefabs == null || spawnPointsBlueBusForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsBlueBusForward)
        {
            Instantiate(blueBusForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }

    void SpawnBlueBusBackward()
    {
        if (blueBusBackwardPrefabs == null || spawnPointsBlueBusBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsBlueBusBackward)
        {
            Instantiate(blueBusBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }

    }
    void SpawnGreenBusForward()
    {
        if (greenBusForwardPrefabs == null || spawnPointsGreenBusForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsGreenBusForward)
        {
            Instantiate(greenBusForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }
    void SpawnGreenBusBackward()
    {
        if (greenBusBackwardPrefabs == null || spawnPointsGreenBusBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsGreenBusBackward)
        {
            Instantiate(greenBusBackwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
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
    void SpawnShoppeeBackward()
    {
        if (shoppeeBackwardPrefabs == null || spawnPointsShoppeeBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsShoppeeBackward)
        {
            Instantiate(shoppeeBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, 180f, 0), transform);
        }
    }
    //Wave
    void SpawnWaveForward()
    {
        if (waveForwardPrefabs == null || spawnPointsWaveForward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsWaveForward)
        {
            Instantiate(waveForwardPrefabs, spawnPoint.position, Quaternion.identity, transform);
        }
    }


    void SpawnWaveBackward()
    {
        if (waveBackwardPrefabs == null || spawnPointsWaveBackward.Length == 0) return;
        foreach (Transform spawnPoint in spawnPointsWaveBackward)
        {
            Instantiate(waveBackwardPrefabs, spawnPoint.position, Quaternion.Euler(0, 180f, 0), transform);
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
        // 🔁 Xoá toàn bộ con được spawn trước đó

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
        //Xanh
        SpawnXanhForward();
        SpawnXanhBackWard();
        //KIA
        SpawnKIAForward();
        SpawnKIABackward();
        //Carnival
        SpawnCarnivalForward();
        SpawnCarnivalBackward();
        //Xe Bat Coc
        SpawnXeBatCocForward();
        SpawnXeBatCocBackward();
        //Truck
        SpawnTruckBackward();
        //Blue Bus
        SpawnBlueBusForward();
        SpawnBlueBusBackward();
        //Green Bus
        SpawnGreenBusForward();
        SpawnGreenBusBackward();
        //Toyota
        SpawnToyotaForward();
        SpawnToyotaBackward();

        //Mystery Box
        SpawnMysteryBox();
    }

}
