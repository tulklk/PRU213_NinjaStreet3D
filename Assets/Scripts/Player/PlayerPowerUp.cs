using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUp : MonoBehaviour
{
    public static PlayerPowerUp Instance { get; private set; }

    [Header("Coin")]
    public GameObject coinDetector;     
    

    [Header("Shield Effect")]
    public GameObject shieldEffect;     
    private bool isShieldActive = false;
    private int remainingShieldHits = 0;

    [Header("Boost Settings")]
    public float boostedSpeed = 12f;    
    public float boostDuration = 5f;   
    private bool isSpeedBoosted = false;


    [Header("Rocket Settings")]
    public GameObject rocketPrefab;     
    public Transform rocketSpawnPoint;  
    public int maxRockets = 3;
    public float delayBetweenRockets = 3f;
    private bool isFiringRockets = false;

    // =========================
    // MAGNET
    // =========================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void ActivateMagnet()
    {
        StartCoroutine(MagnetRoutine());

    }
    IEnumerator MagnetRoutine()
    {
        if (coinDetector != null)
            coinDetector.SetActive(true);

        float duration = PowerUpDatabase.GetValue("magnet"); 
        yield return new WaitForSeconds(duration);

        if (coinDetector != null)
            coinDetector.SetActive(false);
    }


    // =========================
    // SHIELD
    // =========================
    public void ActivateShield()
    {
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        if (shieldEffect != null)
            shieldEffect.SetActive(true);

        int level = PlayerPrefs.GetInt("shield", 0);
        remainingShieldHits = Mathf.Clamp(level + 1, 1, 10); // VD: level 0 = 1 hit, level 1 = 2 hits

        Debug.Log($"🛡️ Khiên bật! Chịu được {remainingShieldHits} va chạm.");
        yield break;
    }


    //SpeedBoost

    public void ActivateSpeedBoost()
    {
        if (!isSpeedBoosted)
            StartCoroutine(SpeedBoostRoutine());
    }

    
    IEnumerator SpeedBoostRoutine()
    {
        isSpeedBoosted = true;
        PlayerControllerSmooth.Instance.isBoosting = true;

        float originalSpeed = PlayerControllerSmooth.Instance.Speed;
        float boostedSpeed = this.boostedSpeed;

        float duration = PowerUpDatabase.GetValue("speedboost");

        PlayerControllerSmooth.Instance.Speed = boostedSpeed;

        // 🔥 Bật Nitro VFX đã chọn
        GameObject currentNitroVFX = NitroSelected.Instance?.GetCurrentNitro();
        if (currentNitroVFX != null)
            currentNitroVFX.SetActive(true);

        float decelerationDuration = duration * 0.2f;
        yield return new WaitForSeconds(duration - decelerationDuration);

        float elapsed = 0f;
        while (elapsed < decelerationDuration)
        {
            PlayerControllerSmooth.Instance.Speed = Mathf.Lerp(boostedSpeed, originalSpeed, elapsed / decelerationDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        PlayerControllerSmooth.Instance.Speed = originalSpeed;
        PlayerControllerSmooth.Instance.isBoosting = false;
        isSpeedBoosted = false;

        // ❌ Tắt VFX
        if (currentNitroVFX != null)
            currentNitroVFX.SetActive(false);
    }



    //Rocket
    public void ActivateRocket()
    {
        if (!isFiringRockets)
            StartCoroutine(FireRocketSequence());
    }
    IEnumerator FireRocketSequence()
    {
        isFiringRockets = true;

        for (int i = 0; i < maxRockets; i++)
        {
            FireSingleRocket();
            yield return new WaitForSeconds(delayBetweenRockets);
        }

        isFiringRockets = false;
    }
    void FireSingleRocket()
    {
        if (rocketPrefab == null || rocketSpawnPoint == null)
        {
            Debug.LogWarning("🚀 RocketPrefab hoặc RocketSpawnPoint chưa được gán!");
            return;
        }
        GameObject rocket = Instantiate(rocketPrefab, rocketSpawnPoint.position, rocketSpawnPoint.rotation);
        rocket.AddComponent<RocketMove>();

        Destroy(rocket, 5f); // Hủy sau 5 giây
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (!isShieldActive) return;

    //    if (other.CompareTag("Vehicle"))
    //    {
    //        remainingShieldHits--;
    //        Debug.Log($"🛻 Va chạm! Shield còn lại: {remainingShieldHits}");

    //        if (remainingShieldHits <= 0)
    //        {
    //            isShieldActive = false;
    //            if (shieldEffect != null)
    //                shieldEffect.SetActive(false);

    //            Debug.Log("🛡️ Khiên đã vỡ sau va chạm!");
    //        }
    //    }
    //}

    public bool IsShieldActive()
    {
        return isShieldActive;
    }
    public bool AbsorbHitWithShield()
    {
        if (!isShieldActive) return false;

        remainingShieldHits--;
        Debug.Log($"🛻 KillZone – va chạm! Shield còn lại: {remainingShieldHits}");

        if (remainingShieldHits <= 0)
        {
            isShieldActive = false;

            if (shieldEffect != null)
                shieldEffect.SetActive(false);

            Debug.Log("🛡️ Khiên đã vỡ khi trúng KillZone!");

           
#if UNITY_ANDROID && !UNITY_EDITOR
        Handheld.Vibrate();
#endif
        }

        return true;
    }



}
