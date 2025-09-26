using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiVehicleShop : MonoBehaviour
{
    public TextMeshProUGUI totalCoinTextMenu;
    public TextMeshProUGUI totalGemTextMenu;

    [Header("Vehicle UI")]
    public TextMeshProUGUI vehiclePriceText;
    public TextMeshProUGUI vehicleBuyButtonText;
    public Button vehicleBuyButton;

    [Header("Nitro UI")]
    public TextMeshProUGUI nitroPriceText;
    public TextMeshProUGUI nitroBuyButtonText;
    public Transform nitroSpawnPoint; 
    //private GameObject currentNitroVFX;
    public Button nitroBuyButton;
 
    //UI
    public GameObject denyUI;
    public GameObject buySuccessUI;
    public GameObject vehicleShopUI;
    public GameObject nitroShopUI;

    public ShopManager shopManager;
    public NitroShopManager nitroShopManager;
    private Animator camPointAnimator;


    void Start()
    {
        UpdateCoinUI();
        UpdateGemUI();
        GameObject camPoint = GameObject.Find("CamPoint");
        if (camPoint != null)
        {
            camPointAnimator = camPoint.GetComponent<Animator>();
            if (camPointAnimator != null)
            {
                camPointAnimator.SetBool("isSwitch", false);
            }
        }
    }
    //UI hiển thị xác nhận
    public void ShowDenyUi()
    {
        UIAnimator.Show(denyUI);
    }
    public void HideDenyUi()
    {
        UIAnimator.Hide(denyUI);
    }
    public void ShowBuySuccessUi()
    {
        UIAnimator.Show(buySuccessUI);
    }
    public void HideBuySuccessUi()
    {
        UIAnimator.Hide(buySuccessUI);
    }
    public void UpdateCoinUI()
    {
        int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        totalCoinTextMenu.SetText(totalCoins.ToString());
    }
    public void UpdateGemUI()
    {
        int totalGems = PlayerPrefs.GetInt("TotalGems", 0);
        totalGemTextMenu.SetText(totalGems.ToString());
    }

    //public void UpdateVehiclePriceUI(VehicleData data, int index, bool isSelected)
    //{
    //    if (isSelected)
    //    {
    //        buyButtonText.text = "Selected";
    //        buyButton.interactable = false;
    //    }
    //    else if (data.isUnlocked)
    //    {
    //        buyButtonText.text = "Select";
    //        buyButton.interactable = true;
    //    }
    //    else
    //    {
    //        //buyButtonText.text = $"Buy: {data.coinPrice:N0}💰";
    //        buyButtonText.SetText($"    {data.coinPrice:N0}  <sprite=0>");
    //        buyButton.interactable = true;
    //    }
    //}

    public void UpdateVehiclePriceUI(VehicleData data, int index, bool isSelected)
    {
        if (isSelected)
        {
            vehicleBuyButtonText.text = "Selected";
            vehicleBuyButton.interactable = false;
        }
        else if (data.isUnlocked)
        {
            vehicleBuyButtonText.text = "Select";
            vehicleBuyButton.interactable = true;
        }
        else
        {
            vehicleBuyButtonText.SetText($"    {data.coinPrice:N0}  <sprite=0>");
            vehicleBuyButton.interactable = true;
        }
    }


    //public void UpdateNitroUI(NitroData data, int index)
    //{

    //    if (nitroBuyButtonText == null || nitroPriceText == null || nitroBuyButton == null)
    //    {
    //        Debug.LogError("⚠️ Nitro UI chưa được gán trong Inspector.");
    //        return;
    //    }

    //    if (data.isUnlocked)
    //    {
    //        if (index == PlayerPrefs.GetInt("SelectedNitro", 0))
    //        {
    //            nitroBuyButtonText.text = "Selected";
    //            nitroBuyButton.interactable = false;

    //        }
    //        else
    //        {
    //            nitroBuyButtonText.text = "Select";
    //            nitroBuyButton.interactable = true;
    //        }
    //        nitroPriceText.text = "";
    //    }
    //    else
    //    {
    //        nitroPriceText.SetText($"    {data.price:N0}  <sprite=0>");
    //        nitroBuyButton.interactable = true;
    //    }
    //}
    public void UpdateNitroUI(NitroData nitro, int index, bool isSelected)
    {
        if (nitro == null) return;

        if (nitro.isUnlocked)
        {
            nitroBuyButtonText.text = isSelected ? "SELECTED" : "SELECT";
            nitroBuyButton.interactable = !isSelected;
        }
        else
        {
            //nitroBuyButtonText.text = nitro.price.ToString("N0") + " <sprite=0>";
            nitroBuyButtonText.SetText($"    {nitro.price:N0}  <sprite=0>");
            nitroBuyButton.interactable = true;
        }
    }





    public void Home()
    {
        int currentGems = PlayerPrefs.GetInt("TotalGems", 0);
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("TotalCoins", currentCoins);
        PlayerPrefs.SetInt("TotalGems", currentGems);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Play");
    }
    public void NitroShop()
    {
        if (camPointAnimator != null)
        {
            camPointAnimator.SetBool("isSwitch", true);
        }

        vehicleShopUI.SetActive(false);

        // Gọi coroutine mở Nitro UI sau 4s
        StartCoroutine(ShowNitroShopDelayed());
    }

    private IEnumerator ShowNitroShopDelayed()
    {
        yield return new WaitForSeconds(2f);

        nitroShopUI.SetActive(true);

        // 🔥 Hiện Nitro VFX sau khi shop Nitro hiện ra
        nitroShopManager.ShowCurrentNitro();

        // Cập nhật lại coin/gem
        int currentGems = PlayerPrefs.GetInt("TotalGems", 0);
        int currentCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        PlayerPrefs.SetInt("TotalCoins", currentCoins);
        PlayerPrefs.SetInt("TotalGems", currentGems);
        PlayerPrefs.Save();
    }



    public void DeleteData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("All PlayerPrefs data deleted.");
    }
}
