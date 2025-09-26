
using UnityEngine;

public static class PowerUpDatabase
{
    public static float GetValue(string id)
    {
        PowerUpData data = Resources.Load<PowerUpData>($"PowerUps/{id}");
        if (data == null)
        {
            Debug.LogWarning("❌ Không tìm thấy PowerUpData với ID: " + id);
            return 0f;
        }

        int level = PlayerPrefs.GetInt(id, 0);

        if (data.levelValues == null || data.levelValues.Count == 0)
        {
            Debug.LogWarning($"⚠️ PowerUpData [{id}] không có giá trị levelValues!");
            return 0f;
        }

        
        level = Mathf.Clamp(level, 0, data.levelValues.Count - 1);
        return data.levelValues[level];
    }
}
