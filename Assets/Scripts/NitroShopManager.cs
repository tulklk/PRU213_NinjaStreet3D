using UnityEngine;

public class NitroShopManager : MonoBehaviour
{
    public NitroData[] nitros;
    public UiVehicleShop uiShop;

    private int currentNitroIndex;

    void Start()
    {
        HideAllNitroVFX(); // ✳️ ẩn VFX khi vừa load scene
        LoadNitroData();
    }

    void LoadNitroData()
    {
        for (int i = 0; i < nitros.Length; i++)
        {
            nitros[i].isUnlocked = PlayerPrefs.GetInt("NitroUnlocked_" + i, i == 0 ? 1 : 0) == 1;
        }

        currentNitroIndex = PlayerPrefs.GetInt("SelectedNitro", 0);
    }

    void SelectNitro(int index)
    {
        currentNitroIndex = index;
        ShowCurrentNitro();
    }

    //public void ShowCurrentNitro()
    //{
    //    if (uiShop == null)
    //    {
    //        Debug.LogError("⚠️ uiShop (UiVehicleShop) chưa được gán trong Inspector.");
    //        return;
    //    }

    //    // Cập nhật UI
    //    NitroData nitro = nitros[currentNitroIndex];
    //    //uiShop.UpdateNitroUI(nitro, currentNitroIndex);
    //    uiShop.UpdateNitroUI(nitro, currentNitroIndex);

    //    // Tắt toàn bộ VFX trước
    //    foreach (NitroData n in nitros)
    //    {
    //        if (n.nitroVFX != null)
    //            n.nitroVFX.SetActive(false);
    //    }

    //    // Bật Nitro VFX hiện tại
    //    if (nitro.nitroVFX != null)
    //    {
    //        nitro.nitroVFX.SetActive(true);
    //        var ps = nitro.nitroVFX.GetComponent<ParticleSystem>();
    //        if (ps != null)
    //        {
    //            ps.Clear();
    //            ps.Play();
    //        }
    //    }
    //}

    public void ShowCurrentNitro()
    {
        if (uiShop == null)
        {
            Debug.LogError("⚠️ uiShop (UiVehicleShop) chưa được gán trong Inspector.");
            return;
        }

        NitroData nitro = nitros[currentNitroIndex];
        bool isSelected = currentNitroIndex == PlayerPrefs.GetInt("SelectedNitro", 0);

        uiShop.UpdateNitroUI(nitro, currentNitroIndex, isSelected);

        foreach (NitroData n in nitros)
        {
            if (n.nitroVFX != null)
                n.nitroVFX.SetActive(false);
        }

        if (nitro.nitroVFX != null)
        {
            nitro.nitroVFX.SetActive(true);
            var ps = nitro.nitroVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Clear();
                ps.Play();
            }
        }
    }

    void HideAllNitroVFX()
    {
        foreach (NitroData n in nitros)
        {
            if (n.nitroVFX != null)
                n.nitroVFX.SetActive(false);
        }
    }

    public void NextNitro()
    {
        currentNitroIndex = (currentNitroIndex + 1) % nitros.Length;
        ShowCurrentNitro();
    }

    public void PreviousNitro()
    {
        currentNitroIndex = (currentNitroIndex - 1 + nitros.Length) % nitros.Length;
        ShowCurrentNitro();
    }

    public void BuyOrSelectNitro()
    {
        NitroData current = nitros[currentNitroIndex];

        if (current.isUnlocked)
        {
            PlayerPrefs.SetInt("SelectedNitro", currentNitroIndex);
            Debug.Log("Nitro selected: " + current.name);
        }
        else
        {
            int coins = PlayerPrefs.GetInt("TotalCoins", 0);
            if (coins >= current.price)
            {
                coins -= current.price;
                PlayerPrefs.SetInt("TotalCoins", coins);
                PlayerPrefs.SetInt("NitroUnlocked_" + currentNitroIndex, 1);
                current.isUnlocked = true;
                PlayerPrefs.SetInt("SelectedNitro", currentNitroIndex);
                uiShop.ShowBuySuccessUi();
                Debug.Log("Nitro purchased: " + current.name);
            }
            else
            {
                Debug.Log("Not enough coins!");
                uiShop.ShowDenyUi();
                return;
            }
        }

        PlayerPrefs.Save();
        ShowCurrentNitro();
        UpdateCurrentUI();
    }
    private NitroData GetCurrentNitro()
    {
        return nitros[currentNitroIndex];
    }


    private void UpdateCurrentUI()
    {
        NitroData current = GetCurrentNitro();
        bool isSelected = currentNitroIndex == PlayerPrefs.GetInt("SelectedNitro", 0);
        Debug.Log("UpdateCurrentUI - currentIndex: " + currentNitroIndex + ", isSelected: " + isSelected);

        // Truyền đúng tham số thứ 3
        uiShop.UpdateNitroUI(current, currentNitroIndex, isSelected);
        uiShop.UpdateCoinUI();
    }



}
