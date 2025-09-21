using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingBoySpawner : MonoBehaviour
{
    public static RacingBoySpawner Instance;
    [Header("References")]
    public GameObject redLineWarning;
    private Transform playerTransform;


    [Header("Settings")]
    public float delayBeforeWarning = 1.5f;     // Delay sau khi chạm tile
    public float redLineDuration = 2.5f;        // Cảnh báo trong bao lâu
    public float blinkInterval = 0.25f;         // Nhấp nháy
    public float redLineFollowSpeed = 5f;       // Red line bám theo X của Player
    public float redLineZOffset = 1f;           // Z phía trước player
    public float racingBoyOffsetZ = 8f;         // Spawn phía sau red line

    private bool isRedLineActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (redLineWarning != null)
            redLineWarning.SetActive(false);
    }

    void Update()
    {
        if (isRedLineActive && redLineWarning.activeSelf)
        {
            FollowPlayerX();
            float playerZ = playerTransform.position.z;
            redLineWarning.transform.position = new Vector3(
                redLineWarning.transform.position.x,
                redLineWarning.transform.position.y,
                playerZ + redLineZOffset
            );
        }
    }

    void FollowPlayerX()
    {
        Vector3 current = redLineWarning.transform.position;
        Vector3 target = new Vector3(playerTransform.position.x, current.y, current.z);
        redLineWarning.transform.position = Vector3.Lerp(current, target, Time.deltaTime * redLineFollowSpeed);
    }

    public void ScheduleSpawn(Transform groundTile)
    {
        if (PlayerController.Instance == null)
        {
            Debug.LogWarning("Không tìm thấy PlayerControllerSmooth.Instance!");
            return;
        }

        playerTransform = PlayerController.Instance.transform;
        StartCoroutine(SpawnRoutine(groundTile));
    }



    IEnumerator SpawnRoutine(Transform tile)
    {
        yield return new WaitForSeconds(delayBeforeWarning);

        SetupRedLine(tile);
        StartCoroutine(BlinkRedLine());

        yield return new WaitForSeconds(redLineDuration);


        SpawnWheels();
    }


    void SetupRedLine(Transform groundTile)
    {
        Vector3 targetPos = new Vector3(
            playerTransform.position.x,
            0.01f,
            playerTransform.position.z + redLineZOffset
        );

        redLineWarning.transform.SetParent(groundTile);
        redLineWarning.transform.position = targetPos;

        redLineWarning.SetActive(true);
        isRedLineActive = true;
    }

    IEnumerator BlinkRedLine()
    {
        float timer = 0f;
        bool visible = true;

        while (timer < redLineDuration)
        {
            redLineWarning.SetActive(visible);
            visible = !visible;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        redLineWarning.SetActive(false);
        isRedLineActive = false;
    }
    void SpawnWheels()
    {
        Vector3 redLinePos = redLineWarning.transform.position;
        Vector3 spawnPos = redLinePos - new Vector3(0f, 0f, racingBoyOffsetZ);

        Vector3 targetPos = redLinePos + Vector3.forward * 10f;
        Vector3 direction = (targetPos - spawnPos).normalized;

        float groundOffset = 0.5f;
        Vector3 adjustedSpawnPos = spawnPos + Vector3.up * groundOffset;

        GameObject rb = ObjectPooler.Instance.SpawnFromPool("Wheels", adjustedSpawnPos);

        if (rb != null && rb.TryGetComponent(out RacingBoyController controller))
        {
            controller.LaunchTowards(direction);
        }
    }
}
