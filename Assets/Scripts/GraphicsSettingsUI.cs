using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsUI : MonoBehaviour
{
    public Button lowButton;
    public Button mediumButton;
    public Button highButton;

    public GameObject highlightLow;
    public GameObject highlightMedium;
    public GameObject highlightHigh;

    void Start()
    {
        int savedLevel = PlayerPrefs.GetInt("QualityLevel", 0); // Mặc định: Medium
        ApplyQuality(savedLevel);

        lowButton.onClick.AddListener(() => SetQuality(0));
        mediumButton.onClick.AddListener(() => SetQuality(1));
        highButton.onClick.AddListener(() => SetQuality(2));
    }

    public void SetQuality(int level)
    {
        QualitySettings.SetQualityLevel(level);
        PlayerPrefs.SetInt("QualityLevel", level);
        PlayerPrefs.Save();

        ApplyQuality(level);
    }

    void ApplyQuality(int level)
    {
        highlightLow.SetActive(level == 0);
        highlightMedium.SetActive(level == 1);
        highlightHigh.SetActive(level == 2);
    }
}
