using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MysteryBox : MonoBehaviour
{
    public List<GameObject> itemPrefabs;
    //public Transform displayPoint;
    public float displayDuration = 2f;
    public ParticleSystem explosionEffect;
    private GameObject currentItem;
    private bool isOpened = false;
    void Start()
    {
        GameObject coinDetectorObj = GameObject.Find("CoinDetector");
        if (coinDetectorObj != null)
        {
            coinDetectorObj.SetActive(false);
            Debug.Log("[MysteryBox] Đã tắt CoinDetector ở Start()");
        }
        else
        {
            Debug.Log("[MysteryBox] Không tìm thấy CoinDetector trong scene!");
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (isOpened) return;
        if (!other.CompareTag("Player")) return;

        Debug.Log($"[MysteryBox] Player chạm vào box: {gameObject.name}");

        isOpened = true;

        if (explosionEffect != null)
            explosionEffect.Play();

        Debug.Log("[MysteryBox] ⏳ Chuẩn bị gọi SpinAndChooseItem...");
        AudioManager.instance.StopVehicleDrivingMusic();

        StartCoroutine(SpinAndChooseItem(other.gameObject));

        Debug.Log("[MysteryBox] ✅ Đã gọi xong StartCoroutine SpinAndChooseItem.");
    }
    IEnumerator SpinAndChooseItem(GameObject player)
    {
        float totalSpinTime = 10f;
        float elapsed = 0f;
        float delay = 0.1f;
        float delayIncrement = 0.05f;

        
        AudioManager.instance.PlaySpinMusic();
        

        Transform displayPoint = player.transform.Find("MysteryItemPoint");
        if (displayPoint == null)
        {
            Debug.LogWarning("[MysteryBox] ❌ Không tìm thấy MysteryItemPoint trong Player!");
            yield break;
        }

        while (elapsed < totalSpinTime)
        {
            if (currentItem != null)
                Destroy(currentItem);

            int randomIndex = Random.Range(0, itemPrefabs.Count);
            currentItem = Instantiate(itemPrefabs[randomIndex], displayPoint.position, Quaternion.identity);
            currentItem.transform.SetParent(displayPoint);

            yield return new WaitForSeconds(delay);
            elapsed += delay;
            delay += delayIncrement;
        }

        AudioManager.instance.StopSpinMusic();
        AudioManager.instance.PlayVehicleDrivingLoop();

        ApplyItemEffect(currentItem.name.Replace("(Clone)", "").Trim(), player);
        Destroy(gameObject);
        yield return new WaitForSeconds(0.5f);
        
    }
    void ApplyItemEffect(string itemName, GameObject player)
    {
        Debug.Log($"[MysteryBox] ApplyItemEffect itemName = '{itemName}'");
        switch (itemName)
        {
            case "CoinMy":
                Debug.Log("Tăng 50 xu từ MysteryBox!");
                AudioManager.instance.sfxSource.PlayOneShot(AudioManager.instance.coinCollectMysteryBox);
                UIManager.instance.PlayCoinBurstEffect(player.transform.position, 8);
                GameManager.instance.AddCoinForMysteryBox();
                Destroy(currentItem);
                break;
            //case "Gem":
            //    Debug.Log("Tăng 1 đá quý!");
            //    break;
            case "Magnet":
                Debug.Log("Kích hoạt nam châm!");
                GameManager.instance.ActivateMagnet();
                GameObject coinDetectorObj = GameObject.Find("CoinDetector");
                coinDetectorObj.SetActive(true);
                Destroy(currentItem);
                break;
            //case "Shield":
            //    Debug.Log("Kích hoạt khiên bảo vệ!");
            //    GameManager.instance.ActivateShield();
            //    Destroy(currentItem);
            //    break;
            case "NitroBoost":
                Debug.Log("Kích hoạt tăng tốc!");
                GameManager.instance.ActivateSpeedBoost();
                Destroy(currentItem);
                break;
            case "RocketImg":
                Debug.Log("kích hoạt tên lửa");
                GameManager.instance.ActivateRocket();
                Destroy(currentItem);
                break;
        }
    }
}
