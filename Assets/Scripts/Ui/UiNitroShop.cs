using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UiNitroShop : MonoBehaviour
{
    public TextMeshProUGUI nitroCoinText;
    public TextMeshProUGUI nitroPriceText;
    public TextMeshProUGUI buttonText;
    public Button actionButton;
    public GameObject denyUI;

    public NitroShopManager shopManager;

    void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        nitroCoinText.SetText(PlayerPrefs.GetInt("TotalCoins", 0).ToString());
    }

    public void UpdateNitroUI(NitroData data, int index)
    {
        UpdateCoinUI();

        if (data.isUnlocked)
        {
            if (index == PlayerPrefs.GetInt("SelectedNitro", 0))
            {
                buttonText.text = "Selected";
                actionButton.interactable = false;
            }
            else
            {
                buttonText.text = "Select";
                actionButton.interactable = true;
            }
            nitroPriceText.text = "";
        }
        else
        {
            buttonText.text = "Buy";
            nitroPriceText.text = $"{data.price} <sprite=0>"; // giả sử bạn dùng TextMeshPro Sprite cho coin
            actionButton.interactable = true;
        }
    }

    public void ShowDenyUI()
    {
        denyUI.SetActive(true);
    }

    public void HideDenyUI()
    {
        denyUI.SetActive(false);
    }
}
