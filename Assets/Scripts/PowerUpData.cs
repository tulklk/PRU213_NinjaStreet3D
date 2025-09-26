
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUp/New PowerUp")]
public class PowerUpData : ScriptableObject
{
    [Header("General Info")]
    public string id;
    public string powerUpName;
    public Sprite icon;

    [Header("Upgrade Stats")]
    public List<float> levelValues = new List<float>(); 
    public int baseCost = 5000;

    [Header("Hiển thị đơn vị")]
    public string unit = "s";

    public int MaxLevel => levelValues.Count - 1;

    public float GetCurrentValue()
    {
        int level = PlayerPrefs.GetInt(id, 0);
        return levelValues[Mathf.Clamp(level, 0, MaxLevel)];
    }

    public float GetNextValue()
    {
        int level = PlayerPrefs.GetInt(id, 0);
        return level < MaxLevel ? levelValues[level + 1] : levelValues[MaxLevel];
    }

    public int GetUpgradeCost()
    {
        int level = PlayerPrefs.GetInt(id, 0);
        return baseCost * (level + 1);
    }
}

