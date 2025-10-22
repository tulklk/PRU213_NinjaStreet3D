
using UnityEngine;
using UnityEngine.UI;

public class VehicleThumbnailItem : MonoBehaviour
{
    public Image icon;
    public Image background; 

    private int vehicleIndex;
    private ShopManager shopManager;

    public void Init(Sprite thumbnail, int index, ShopManager manager)
    {
        vehicleIndex = index;
        shopManager = manager;

        if (icon != null) icon.sprite = thumbnail;

        Button btn = GetComponentInChildren<Button>();
        if (btn != null) btn.onClick.AddListener(OnClick);
        SetHighlight(false);
    }

    public void SetHighlight(bool isActive)
    {
        if (background != null)
        {
            background.color = isActive ? Color.yellow : Color.white; 
        }
    }

    private void OnClick()
    {
        shopManager.SelectVehicleByThumbnail(vehicleIndex);
    }
}

