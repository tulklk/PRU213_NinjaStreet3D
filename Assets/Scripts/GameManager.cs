using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private PlayerController player;
    private bool isGameStarted = false;
    public bool IsGameStarted { get; private set; } = false;

    //coins, gems, scores, distance
    public int scores;
    public int coinsCollected;
    public int gemsCollected;
    public int totalGems;
    private int totalCoins;
    private float startZPos;
    public float distanceTravelled;
    public float maxDistanceTravelled;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        if (!PlayerPrefs.HasKey("QualityLevel"))
        {
            PlayerPrefs.SetInt("QualityLevel", 0); // 1 = Medium
            PlayerPrefs.Save();
        }


        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QualityLevel", 0));
    }

    void Start()
    {
        Debug.Log("🔥 Current Quality Level: " + QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("TotalCoins", 1000000);
        //PlayerPrefs.SetInt("TotalGems", 1000000);
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        totalGems = PlayerPrefs.GetInt("TotalGems", 0);
        maxDistanceTravelled = PlayerPrefs.GetFloat("MaxDistance", 0f);
        UiManager.instance.coinText.SetText(scores.ToString());
        UiManager.instance.coinTextLose.SetText(totalCoins.ToString());
        //Time.timeScale = 0; 
        //UIManager.instance.gemText.SetText(totalGems.ToString());
        //player = FindObjectOfType<PlayerControllerSmooth>();
        //if (player != null)
        //{
        //    player.SetGameStarted(false);
        //    startZPos = player.transform.position.z;
        //}

    }
    public void AddCoin()
    {
        coinsCollected++;
        UiManager.instance.coinText.SetText(coinsCollected.ToString());
    }

    //public void AddGem()
    //{
    //    gemsCollected++;
    //    totalGems += gemsCollected; // Cộng dồn vào tổng gems
    //    UIManager.instance.gemText.SetText(totalGems.ToString()); // Cập nhật UI
    //    //PlayerPrefs.SetInt("TotalGems", totalGems); // Lưu tổng gems
    //    //PlayerPrefs.Save(); // Lưu vào PlayerPrefs
    //}

    void Update()
    {
        if (player != null && UiManager.instance.distanceText != null)
        {
            distanceTravelled = player.transform.position.z - startZPos;
            distanceTravelled = Mathf.Max(0, distanceTravelled);
            UiManager.instance.distanceText.SetText($"{distanceTravelled:F1} M");
        }
        if (UiManager.instance.coinText != null)
        {
            UiManager.instance.coinText.SetText(coinsCollected.ToString());
        }
        player.UpdateSpeedByDistance(distanceTravelled);

    }

    public void StartGame()
    {
        IsGameStarted = true;

        if (!isGameStarted && player != null)
        {
            isGameStarted = true;
            player.SetGameStarted(true);


        }
    }

    public void PauseGame()
    {
        isGameStarted = false;
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        Time.timeScale = 0;


        totalCoins += coinsCollected;
        totalGems += gemsCollected;


        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.SetInt("TotalGems", totalGems);


        if (distanceTravelled > maxDistanceTravelled)
        {
            maxDistanceTravelled = distanceTravelled;
            PlayerPrefs.SetFloat("MaxDistance", maxDistanceTravelled);
        }

        PlayerPrefs.Save();
        UiManager.instance.ShowGameOverUI(coinsCollected, distanceTravelled, maxDistanceTravelled);
    }


    public void VehcilceShop()
    {
        PlayerPrefs.SetInt("TotalGems", totalGems);
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.Save();

    }
    public void ResetMaxDistance()
    {
        PlayerPrefs.DeleteKey("MaxDistance");
        maxDistanceTravelled = 0;
    }
    public void Home()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.SetInt("TotalGems", totalGems);
        PlayerPrefs.Save();
    }


    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
    public void SpendCoin(int amount)
    {
        totalCoins -= amount;
        if (totalCoins < 0) totalCoins = 0;

        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        UiManager.instance.coinText.SetText(totalCoins.ToString());
    }


    public int GetTotalGem()
    {
        return totalGems;
    }
    public float GetMaxDistance()
    {
        return maxDistanceTravelled;
    }
    // =========================
    // MAGNET
    // =========================
    //public void ActivateMagnet()
    //{
    //    PlayerPowerUp.Instance.ActivateMagnet();
    //    UiManager.instance.TurnMagnetUi();
    //}


    // =========================
    // SHIELD
    // =========================
    //public void ActivateShield()
    //{

    //    PlayerPowerUp.Instance.ActivateShield();
    //}
    ////SpeedBoost
    //public void ActivateSpeedBoost()
    //{
    //    UiManager.instance.TurnBoostUi();


    //    PlayerPowerUp.Instance.ActivateSpeedBoost();


    //    float duration = PowerUpDatabase.GetValue("speedboost");


    //    Invoke(nameof(StopBoostUIAfterDelay), duration);
    //}

    private void StopBoostUIAfterDelay()
    {
            UiManager.instance.StopBoostUI();
    }

    //public void ActivateRocket()
    //{

    //    PlayerPowerUp.Instance.ActivateRocket();
    //    //UIManager.instance.TurnRocketUi();
    //}


    //Add Coin
    public void AddCoinForMysteryBox(int amount = 50)
    {
        coinsCollected += amount;

        // Cập nhật ngay UI hiển thị xu đang chơi
            UiManager.instance.coinText.SetText(coinsCollected.ToString());

        Debug.Log($"[GameManager] +{amount} xu từ MysteryBox → coinsCollected = {coinsCollected}");
    }
    //public void AddGemForMysteryBox(int amount = 1)
    //{
    //    totalGems += amount;
    //    UIManager.instance.gemText.SetText(totalGems.ToString());
    //    Debug.Log($"[GameManager] +{amount} gem từ MysteryBox → totalGems = {totalGems}");



    //}
}
