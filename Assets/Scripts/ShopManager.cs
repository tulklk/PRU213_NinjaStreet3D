using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShopManager : MonoBehaviour
{
    public int currentVehicleIndex;
    public VehicleData[] vehicles;
    public UiVehicleShop uiShop;
    public Transform thumbnailParent;
    public GameObject thumbnailPrefab;
    public float spacing = 1000f;

    private List<VehicleThumbnailItem> thumbnailItems = new List<VehicleThumbnailItem>();

    void Start()
    {
        if (vehicles == null || vehicles.Length == 0) return;

        currentVehicleIndex = PlayerPrefs.GetInt("SelectedVehicle", 0);
        if (currentVehicleIndex < 0 || currentVehicleIndex >= vehicles.Length)
            currentVehicleIndex = 0;

        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].model != null)
                vehicles[i].model.SetActive(false);

            vehicles[i].isUnlocked = PlayerPrefs.GetInt("VehicleUnlocked_" + i, i == 0 ? 1 : 0) == 1;
        }

        if (vehicles[currentVehicleIndex].model != null)
            vehicles[currentVehicleIndex].model.SetActive(true);

        GenerateThumbnailList();
        UpdateCurrentUI();
        HighlightAndPositionThumbnails();
    }

    private void GenerateThumbnailList()
    {
        foreach (Transform child in thumbnailParent)
        {
            Destroy(child.gameObject);
        }

        thumbnailItems.Clear();

        for (int i = 0; i < vehicles.Length; i++)
        {
            GameObject item = Instantiate(thumbnailPrefab, thumbnailParent);
            VehicleThumbnailItem thumbnail = item.GetComponent<VehicleThumbnailItem>();
            thumbnail.Init(vehicles[i].thumbnailSprite, i, this);
            thumbnailItems.Add(thumbnail);
        }
    }

    public void SelectVehicleByThumbnail(int index)
    {
        if (index == currentVehicleIndex) return;

        vehicles[currentVehicleIndex].model.SetActive(false);
        currentVehicleIndex = index;
        ShowOnlyCurrentVehicle();

        UpdateCurrentUI();
        HighlightAndPositionThumbnails();
    }

    public void ChangeNext()
    {
        currentVehicleIndex = (currentVehicleIndex + 1) % vehicles.Length;
        ShowOnlyCurrentVehicle();
        UpdateCurrentUI();
        HighlightAndPositionThumbnails();
    }

    public void ChangePrevious()
    {
        currentVehicleIndex = (currentVehicleIndex - 1 + vehicles.Length) % vehicles.Length;
        ShowOnlyCurrentVehicle();
        UpdateCurrentUI();
        HighlightAndPositionThumbnails();
    }

    private void ShowOnlyCurrentVehicle()
    {
        for (int i = 0; i < vehicles.Length; i++)
        {
            if (vehicles[i].model != null)
                vehicles[i].model.SetActive(i == currentVehicleIndex);
        }
    }
    private void HighlightAndPositionThumbnails()
    {
        for (int i = 0; i < thumbnailItems.Count; i++)
        {
            bool isSelected = i == currentVehicleIndex;

            float targetX = (i - currentVehicleIndex) * spacing;
            float targetY = isSelected ? 150f : 0f; 

            RectTransform rt = thumbnailItems[i].GetComponent<RectTransform>();

            rt.DOAnchorPos(new Vector2(targetX, targetY), 0.4f).SetEase(Ease.OutQuart);
            thumbnailItems[i].SetHighlight(isSelected);
            thumbnailItems[i].transform.DOScale(isSelected ? 1.2f : 0.9f, 0.3f).SetEase(Ease.OutBack);
        }
    }


    public void BuyOrSelectVehicle()
    {
        VehicleData current = GetCurrentVehicle();

        if (current.isUnlocked)
        {
            if (currentVehicleIndex == PlayerPrefs.GetInt("SelectedVehicle"))
            {
                Debug.Log("✅ Xe đang được chọn rồi.");
                return;
            }

            PlayerPrefs.SetInt("SelectedVehicle", currentVehicleIndex);
            Debug.Log("🚗 Đã chọn xe: " + currentVehicleIndex);
        }
        else
        {
            int totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
            int totalGems = PlayerPrefs.GetInt("TotalGems", 0);
            int coinPrice = current.coinPrice;
            

            if (totalCoins >= coinPrice)
            {
                totalCoins -= coinPrice;
                PlayerPrefs.SetInt("TotalCoins", totalCoins);
                UnlockVehicle();
                uiShop.ShowBuySuccessUi();
                Debug.Log($"✅ Đã mua xe bằng coin: {coinPrice}");
            }
            else
            {
                Debug.Log("❌ Không đủ coin hoặc gem để mua xe!");
                uiShop.ShowDenyUi();
            }
        }

        UpdateCurrentUI();
        HighlightAndPositionThumbnails();
    }

    private void UnlockVehicle()
    {
        PlayerPrefs.SetInt("VehicleUnlocked_" + currentVehicleIndex, 1);
        vehicles[currentVehicleIndex].isUnlocked = true;
        PlayerPrefs.SetInt("SelectedVehicle", currentVehicleIndex);
    }

    private VehicleData GetCurrentVehicle()
    {
        return vehicles[currentVehicleIndex];
    }

    private void UpdateCurrentUI()
    {
        VehicleData current = GetCurrentVehicle();
        uiShop.UpdateVehiclePriceUI(current, currentVehicleIndex, currentVehicleIndex == PlayerPrefs.GetInt("SelectedVehicle"));
        uiShop.UpdateCoinUI();
    }
}


