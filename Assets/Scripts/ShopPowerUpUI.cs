using UnityEngine;

public class ShopPowerUpUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform contentPanel;
    public PowerUpData[] powerUps;

    void Start()
    {
        foreach (var p in powerUps)
        {
            GameObject go = Instantiate(itemPrefab, contentPanel);
            go.GetComponent<PowerUpItemUI>().Init(p);
        }
    }
}
