using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image icon;                         // Icon hình ảnh
    public TextMeshProUGUI nameText;           // Tên PowerUp (VD: "Rocket")
    public TextMeshProUGUI statText;           // Thông số: "Duration: 10s >> 12s"
    public TextMeshProUGUI priceText;          // Giá tiền: "11,000"
    public Button upgradeButton;               // Nút nâng cấp

    private PowerUpData data;
    private int currentLevel;

    public void Init(PowerUpData powerUpData)
    {
        data = powerUpData;
        currentLevel = PlayerPrefs.GetInt(data.id, 0);

        // Gán dữ liệu ban đầu
        icon.sprite = data.icon;
        nameText.text = data.powerUpName;

        RefreshUI();

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(Upgrade);
    }
    void RefreshUI()
    {
        currentLevel = PlayerPrefs.GetInt(data.id, 0);

        if (currentLevel >= data.MaxLevel)
        {
            statText.text = "Maxed";
            upgradeButton.interactable = false;
            return;
        }

        float current = data.GetCurrentValue();
        float next = data.GetNextValue();
        int cost = data.GetUpgradeCost();

        statText.text = $" {current}{data.unit} >> {next}{data.unit} ";
        priceText.text = $"{cost:N0}";
    }
    void Upgrade()
    {
        int cost = data.baseCost * (currentLevel + 1);
        int coin = GameManager.instance.GetTotalCoins();

        if (coin >= cost)
        {
            GameManager.instance.SpendCoin(cost); 
            currentLevel++;

            PlayerPrefs.SetInt(data.id, currentLevel);
            PlayerPrefs.Save();

            RefreshUI();
        }
        else
        {
            UIManager.instance.ShowDenyUpdateUI();
            Debug.LogWarning("❌ Không đủ xu để nâng cấp.");
        }
    }

    public int GetLevel()
    {
        return currentLevel;
    }
}
