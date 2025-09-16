using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public static UiManager instance;
    private Animator camPointAnimator;
    [SerializeField] private float hideBackgroundAtDistance = 5f;
    private bool isBackgroundHidden = false;

    //Button
    [Header("Button")]
    public Button startButton;
    public Button resumeButton;
    public Button restartButton;
    public Button pausedButton;
    public Button vehicleButton;
    public Button shopButton;
    public Button powerUpButton;

    //Text
    [Header("Text")]
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinTextLose;
    public TextMeshProUGUI powerUpCoinText;
    public TextMeshProUGUI powerUpGemText;
    public TextMeshProUGUI itemCoinText;
    public TextMeshProUGUI itemGemText;

    //public TextMeshProUGUI gemText; 
    public TextMeshProUGUI totalCoinTextMenu;
    public TextMeshProUGUI totalGemTextMenu;
    public TextMeshProUGUI highestDistance;
    public TextMeshProUGUI maxDistanceText;


    //Các panel
    [Header("Panel")]
    public GameObject menuUI;
    public GameObject playerSceneUI;
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public GameObject backgroundObject;
    public GameObject settingUI;
    public GameObject boostUI;
    public GameObject loadingUI;
    public GameObject magnetUI;
    public GameObject shieldUI;
    public GameObject rocketUI;
    public GameObject powerUpShopUI;
    public GameObject updateSuccessUI;
    public GameObject denyUpdateUI;
    public GameObject itemShopUI;

    [Header("Coin Fly Effect")]
    public GameObject flyCoinPrefab;
    public RectTransform uiCoinTarget;

    [Header("Setting Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // Các hiệu ứng
    public GameObject magnetUiCountDown;
    public GameObject shieldUiCountDown;
    public GameObject rocketUiCountDown;

    public Sprite magnetIcon;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CheckHideBackground();
    }

    private void CheckHideBackground()
    {
        if (isBackgroundHidden || backgroundObject == null || GameManager.instance == null) return;

        if (GameManager.instance.distanceTravelled >= hideBackgroundAtDistance)
        {
            backgroundObject.SetActive(false);
            isBackgroundHidden = true;
        }
    }

    void Start()
    {
        GameObject camPoint = GameObject.Find("CamPoint");
        if (camPoint != null)
        {
            camPointAnimator = camPoint.GetComponent<Animator>();
            if (camPointAnimator != null)
            {
                //camPointAnimator.SetBool("isSwitch", false);
                camPointAnimator.SetBool("isSwitch1", false);
            }
        }

        // Gán nút bắt đầu
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        // Cập nhật thông tin coin, gem, max distance
        if (totalCoinTextMenu != null)
        {
            totalCoinTextMenu.SetText(GameManager.instance.GetTotalCoins().ToString());
        }
        if (totalGemTextMenu != null)
        {
            totalGemTextMenu.SetText(GameManager.instance.GetTotalGem().ToString());
        }
        if (highestDistance != null)
        {
            highestDistance.SetText($"{GameManager.instance.GetMaxDistance():F1} M");
        }

        // Hiển thị Loading UI và chuẩn bị chuyển mượt sang Menu UI
        if (loadingUI != null && menuUI != null)
        {
            CanvasGroup loadingGroup = loadingUI.GetComponent<CanvasGroup>();
            CanvasGroup menuGroup = menuUI.GetComponent<CanvasGroup>();

            loadingUI.SetActive(true);
            menuUI.SetActive(true); // Đặt active trước để canvas group hoạt động
            loadingGroup.alpha = 1f;
            loadingGroup.interactable = true;
            loadingGroup.blocksRaycasts = true;

            menuGroup.alpha = 0f;
            menuGroup.interactable = false;
            menuGroup.blocksRaycasts = false;

            StartCoroutine(FadeInToMenu(2f));
        }
        else
        {
            if (menuUI != null)
            {
                menuUI.SetActive(true);
            }
        }
        // Cập nhật giá trị ban đầu cho sliders từ AudioManager
        //if (musicSlider != null)
        //{
        //    musicSlider.value = AudioManager.instance.musicVolume;
        //    musicSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(musicSlider.value); });
        //}

        //if (sfxSlider != null)
        //{
        //    sfxSlider.value = AudioManager.instance.sfxVolume;
        //    sfxSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChanged(sfxSlider.value); });
        //}

    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        StartCoroutine(DelayPlayVehicleDriving());

        if (camPointAnimator != null)
        {
            camPointAnimator.SetBool("isSwitch1", true);
        }

        if (menuUI != null)
        {
            menuUI.SetActive(false);
            playerSceneUI.SetActive(true);
        }

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
            isBackgroundHidden = false;
        }

        GameManager.instance.StartGame();
    }
    IEnumerator DelayPlayVehicleDriving()
    {
        yield return new WaitForSeconds(2.6f);
        //AudioManager.instance.PlayVehicleDrivingLoop();
    }

    public void VehicleShop()
    {
        GameManager.instance.VehcilceShop();
        SceneManager.LoadScene("VehicleShop");

    }

    public void PowerUpShop()
    {
        if (powerUpCoinText != null)
            powerUpCoinText.SetText(GameManager.instance.GetTotalCoins().ToString());

        if (powerUpGemText != null)
            powerUpGemText.SetText(GameManager.instance.GetTotalGem().ToString());

        UiAnimator.Show(powerUpShopUI);
    }
    // Mở item shop panel
    public void OpenItemShop()
    {
        if (powerUpCoinText != null)
            powerUpCoinText.SetText(GameManager.instance.GetTotalCoins().ToString());

        if (powerUpGemText != null)
            powerUpGemText.SetText(GameManager.instance.GetTotalGem().ToString());
        UiAnimator.Show(itemShopUI);
    }
    // Đóng item shop panel
    public void CloseItemShop()
    {
        PlayerPrefs.SetInt("TotalCoins", GameManager.instance.GetTotalCoins());
        PlayerPrefs.SetInt("TotalGems", GameManager.instance.GetTotalGem());
        PlayerPrefs.Save();
        RefreshMenuUI();
        UiAnimator.Hide(itemShopUI);
    }


    public void ClosePowerUpShop()
    {
        PlayerPrefs.SetInt("TotalCoins", GameManager.instance.GetTotalCoins());
        PlayerPrefs.SetInt("TotalGems", GameManager.instance.GetTotalGem());
        PlayerPrefs.Save();
        RefreshMenuUI();
        UiAnimator.Hide(powerUpShopUI);
    }
    public void RefreshMenuUI()
    {
        if (totalCoinTextMenu != null)
            totalCoinTextMenu.SetText(GameManager.instance.GetTotalCoins().ToString());

        if (totalGemTextMenu != null)
            totalGemTextMenu.SetText(GameManager.instance.GetTotalGem().ToString());
    }

    public void Play()
    {
        SceneManager.LoadScene("Play");

    }
    public void ReturnHome()
    {
        SceneManager.LoadScene("Play");

    }

    public void Quit()
    {
        GameManager.instance.RetryGame();
    }
    public void Home()
    {
        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
            isBackgroundHidden = false;
        }


        GameManager.instance.RetryGame();


        if (totalCoinTextMenu != null)
        {
            totalCoinTextMenu.SetText(GameManager.instance.GetTotalCoins().ToString());
        }
        RefreshMenuUI();
    }
    public void PausedGame()
    {
        UiAnimator.Show(pauseUI);
        GameManager.instance.PauseGame();

    }
    public void ResumeGame()
    {
        UiAnimator.Hide(pauseUI);
        playerSceneUI.SetActive(true);
        GameManager.instance.ResumeGame();

    }
    public void RestartGame()
    {

        if (backgroundObject != null)
        {
            backgroundObject.SetActive(true);
            isBackgroundHidden = false;
        }

        GameManager.instance.RetryGame();
    }

    public void ShowGameOverUI(int coins, float distance, float maxDistance)
    {
        gameOverUI.SetActive(true);
        playerSceneUI.SetActive(false);


        if (coinTextLose != null)
            coinTextLose.SetText($"{coins}");


        if (distanceText != null)
            distanceText.SetText($"{distance:F1} M");


        if (maxDistanceText != null)
        {
            if (distance > maxDistance)
                maxDistanceText.SetText($"🏆 Record: {distance:F1} M");
            else
                maxDistanceText.SetText($"{maxDistance:F1} M");
        }


        if (totalCoinTextMenu != null)
            totalCoinTextMenu.SetText(GameManager.instance.GetTotalCoins().ToString());
    }
    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        RefreshMenuUI();
    }
    //Setting Panel
    public void SettingButton()
    {

        UiAnimator.Show(settingUI);

    }

    //Deny Buy UI
    public void ShowDenyBuyUI()
    {
        UiAnimator.Show(denyUpdateUI);
    }
    public void CloseDenyBuyUI()
    {
        UiAnimator.Hide(denyUpdateUI);
    }
    //Deny Update UI
    public void ShowDenyUpdateUI()
    {
        UiAnimator.Show(denyUpdateUI);
    }
    public void CloseDenyUpdateUI()
    {
        UiAnimator.Hide(denyUpdateUI);
    }
    public void CloseSetting()
    {

        UiAnimator.Hide(settingUI);
    }
    //Boost UI
    public void TurnBoostUi()
    {
        boostUI.SetActive(true);
    }
    public void StopBoostUI()
    {
        boostUI.SetActive(false);
    }
    //Magnet UI
    //public void TurnMagnetUi()
    //{
    //    magnetUI.SetActive(true);

    //    if (magnetUI != null)
    //    {
    //        var slider = magnetUI.GetComponent<ItemEffectSlider>();
    //        if (slider != null)
    //        {
    //            float duration = PowerUpDatabase.GetValue("magnet");
    //            slider.StartCountdown(magnetIcon, duration);
    //        }
    //    }
    //}

    //Mở power up shop panel

    //public void PlayCoinBurstEffect(Vector3 worldStartPos, int coinAmount = 5)
    //{
    //    for (int i = 0; i < coinAmount; i++)
    //    {
    //        StartCoroutine(SpawnFlyCoinWithDelay(worldStartPos, i * 0.05f));
    //    }
    //}
    //Effect

    //Hiệu ứng coin bay
    //IEnumerator SpawnFlyCoinWithDelay(Vector3 worldStartPos, float delay)
    //{
    //    yield return new WaitForSeconds(delay);

    //    Vector3 screenPos = Camera.main.WorldToScreenPoint(worldStartPos);

    //    GameObject coin = Instantiate(flyCoinPrefab, transform);
    //    RectTransform rect = coin.GetComponent<RectTransform>();
    //    rect.position = screenPos + new Vector3(Random.Range(-30f, 30f), Random.Range(-30f, 30f), 0f);

    //    CoinFlyEffect fly = coin.GetComponent<CoinFlyEffect>();
    //    fly.targetUI = uiCoinTarget.GetComponent<RectTransform>();
    //}

    IEnumerator FadeInToMenu(float delay)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup loadingGroup = loadingUI.GetComponent<CanvasGroup>();
        CanvasGroup menuGroup = menuUI.GetComponent<CanvasGroup>();

        yield return StartCoroutine(FadeTransition(loadingGroup, menuGroup, 1f));
    }

    public IEnumerator FadeTransition(CanvasGroup fromGroup, CanvasGroup toGroup, float duration)
    {
        float time = 0f;

        toGroup.alpha = 0f;
        toGroup.interactable = false;
        toGroup.blocksRaycasts = false;

        fromGroup.interactable = false;
        fromGroup.blocksRaycasts = false;

        toGroup.gameObject.SetActive(true);

        while (time < duration)
        {
            float t = time / duration;
            fromGroup.alpha = Mathf.Lerp(1f, 0f, t);
            toGroup.alpha = Mathf.Lerp(0f, 1f, t);
            time += Time.deltaTime;
            yield return null;
        }

        fromGroup.alpha = 0f;
        fromGroup.gameObject.SetActive(false);

        toGroup.alpha = 1f;
        toGroup.interactable = true;
        toGroup.blocksRaycasts = true;
    }

    //public void OnMusicVolumeChanged(float value)
    //{
    //    AudioManager.instance.SetMusicVolume(value);
    //}

    //public void OnSFXVolumeChanged(float value)
    //{
    //    AudioManager.instance.SetSFXVolume(value);
    //}
}
