using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public static GroundSpawner Instance;

    public GameObject[] group1Tiles;
    public GameObject[] group2Tiles;
    public GameObject[] group3Tiles;

    [Header("Spawn Settings")]
    [SerializeField] private Transform player;              // drag Player vào, hoặc tìm theo tag ở Awake
    [SerializeField] private float prewarmDistance = 50f;   // còn cách điểm spawn kế tiếp bao nhiêu thì spawn sớm
    [SerializeField] private Transform firstNextSpawnPoint; // child "NextSpawnPoint" của tile khởi đầu trong scene

    private List<GameObject> allTiles;
    private Dictionary<int, Queue<GameObject>> tilePools = new Dictionary<int, Queue<GameObject>>();
    private Vector3 nextSpawnPoint;




    //Xét sự trùng lặp
    private int stepInCycle = 0;
    private List<int> group1AvailableIndices = new List<int>();
    private List<int> group1UsedInFirst4 = new List<int>();
    private List<int> group2AvailableIndices = new List<int>();


    //private void Awake()
    //{
    //    if (Instance == null)
    //        Instance = this;
    //    else
    //        Destroy(gameObject);

    //    allTiles = new List<GameObject>();
    //    allTiles.AddRange(group1Tiles);  // 0..7
    //    allTiles.AddRange(group2Tiles);  // 8..10
    //    allTiles.AddRange(group3Tiles);  // 11..13

    //    for (int i = 0; i < allTiles.Count; i++)
    //    {
    //        tilePools[i] = new Queue<GameObject>();
    //    }

    //    ResetCycle();
    //}

    //void Start()
    //{
    //    for (int i = 0; i < 3; i++)
    //    {
    //        SpawnTile();
    //    }
    //}


    //public void SpawnTile()
    //{
    //    int tileIndex = -1;

    //    // phase 1 → random 4 tile group 1 (không trùng)
    //    if (stepInCycle < 4)
    //    {
    //        int randIndex = Random.Range(0, group1AvailableIndices.Count);
    //        tileIndex = group1AvailableIndices[randIndex];
    //        group1UsedInFirst4.Add(tileIndex);
    //        group1AvailableIndices.RemoveAt(randIndex);
    //    }
    //    // phase 2 → random 3 tile group 2 (không trùng)
    //    else if (stepInCycle >= 4 && stepInCycle < 7)
    //    {
    //        if (group2AvailableIndices.Count == 0)
    //        {

    //            for (int i = 0; i < group2Tiles.Length; i++)
    //                group2AvailableIndices.Add(i);
    //        }

    //        int randIndex = Random.Range(0, group2AvailableIndices.Count);
    //        tileIndex = group1Tiles.Length + group2AvailableIndices[randIndex];
    //        group2AvailableIndices.RemoveAt(randIndex);
    //    }
    //    // phase 3 → nhóm 3 spawn theo thứ tự
    //    else if (stepInCycle >= 7 && stepInCycle < 10)
    //    {
    //        tileIndex = group1Tiles.Length + group2Tiles.Length + (stepInCycle - 7);
    //    }
    //    // phase 4 → 4 tile group 1 còn lại
    //    else if (stepInCycle >= 10 && stepInCycle < 14)
    //    {
    //        tileIndex = group1AvailableIndices[0];
    //        group1AvailableIndices.RemoveAt(0);
    //    }

    //    var tileToSpawn = GetTileFromPool(tileIndex);
    //    stepInCycle++;

    //    if (stepInCycle >= 14)
    //        ResetCycle();

    //    if (tileToSpawn == null)
    //    {
    //        Debug.LogError($"❌ Không thể lấy tile từ pool tại index {tileIndex}");
    //        return;
    //    }

    //    tileToSpawn.transform.SetParent(transform);
    //    tileToSpawn.transform.position = nextSpawnPoint;
    //    tileToSpawn.transform.rotation = Quaternion.identity;
    //    tileToSpawn.SetActive(true);

    //    var groundTileScript = tileToSpawn.GetComponent<GroundTile>();
    //    if (groundTileScript != null)
    //        groundTileScript.ResetTile();

    //    var nextPoint = tileToSpawn.transform.Find("NextSpawnPoint");
    //    if (nextPoint != null)
    //        nextSpawnPoint = nextPoint.position;
    //    else
    //        nextSpawnPoint += new Vector3(0, 0, 30f);
    //}


    private void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }

        allTiles = new List<GameObject>();
        allTiles.AddRange(group1Tiles);
        allTiles.AddRange(group2Tiles);
        allTiles.AddRange(group3Tiles);
        for (int i = 0; i < allTiles.Count; i++) tilePools[i] = new Queue<GameObject>();

        ResetCycle();
    }

    private void Start()
    {
        // Lấy điểm spawn đầu tiên từ tile đang đặt sẵn trong scene
        nextSpawnPoint = firstNextSpawnPoint != null ? firstNextSpawnPoint.position : transform.position;

        for (int i = 0; i < 3; i++) SpawnTile();
    }

    private void Update()
    {
        // Spawn sớm khi player gần tới điểm spawn kế
        if (player != null && (nextSpawnPoint.z - player.position.z) < prewarmDistance)
        {
            SpawnTile();
        }
    }

    public void SpawnTile()
    {
        int tileIndex = -1;


        // ... (logic chọn tile giữ nguyên)
        //phase 1 → random 4 tile group 1(không trùng)
            if (stepInCycle < 4)
        {
            int randIndex = Random.Range(0, group1AvailableIndices.Count);
            tileIndex = group1AvailableIndices[randIndex];
            group1UsedInFirst4.Add(tileIndex);
            group1AvailableIndices.RemoveAt(randIndex);
        }
        // phase 2 → random 3 tile group 2 (không trùng)
        else if (stepInCycle >= 4 && stepInCycle < 7)
        {
            if (group2AvailableIndices.Count == 0)
            {

                for (int i = 0; i < group2Tiles.Length; i++)
                    group2AvailableIndices.Add(i);
            }

            int randIndex = Random.Range(0, group2AvailableIndices.Count);
            tileIndex = group1Tiles.Length + group2AvailableIndices[randIndex];
            group2AvailableIndices.RemoveAt(randIndex);
        }
        // phase 3 → nhóm 3 spawn theo thứ tự
        else if (stepInCycle >= 7 && stepInCycle < 10)
        {
            tileIndex = group1Tiles.Length + group2Tiles.Length + (stepInCycle - 7);
        }
        // phase 4 → 4 tile group 1 còn lại
        else if (stepInCycle >= 10 && stepInCycle < 14)
        {
            tileIndex = group1AvailableIndices[0];
            group1AvailableIndices.RemoveAt(0);
        }




        var tileToSpawn = GetTileFromPool(tileIndex);
        stepInCycle++;
        if (stepInCycle >= 14) ResetCycle();

        if (tileToSpawn == null)
        {
            Debug.LogError($"❌ Không thể lấy tile từ pool tại index {tileIndex}");
            return;
        }

        tileToSpawn.transform.SetParent(transform);
        tileToSpawn.transform.position = nextSpawnPoint;
        tileToSpawn.transform.rotation = Quaternion.identity;
        tileToSpawn.SetActive(true);

        var groundTileScript = tileToSpawn.GetComponent<GroundTile>();
        if (groundTileScript != null) groundTileScript.ResetTile();

        var nextPoint = tileToSpawn.transform.Find("NextSpawnPoint");
        nextSpawnPoint = nextPoint != null ? nextPoint.position : nextSpawnPoint + new Vector3(0, 0, 30f);
    }




    private void ResetCycle()
    {
        stepInCycle = 0;
        group1AvailableIndices.Clear();
        group1UsedInFirst4.Clear();
        group2AvailableIndices.Clear();

        for (int i = 0; i < group1Tiles.Length; i++)
            group1AvailableIndices.Add(i);

        for (int i = 0; i < group2Tiles.Length; i++)
            group2AvailableIndices.Add(i);
    }


    private GameObject GetTileFromPool(int index)
    {
        if (!tilePools.ContainsKey(index)) return null;

        var pool = tilePools[index];

        foreach (var tile in pool)
        {
            if (!tile.activeInHierarchy)
                return tile;
        }

        if (pool.Count < 10)
        {
            GameObject newTile = Instantiate(allTiles[index]);
            newTile.SetActive(false);
            pool.Enqueue(newTile);
            return newTile;
        }

        var reused = pool.Dequeue();
        pool.Enqueue(reused);
        return reused;
    }
}
