
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class SwitchToggle : MonoBehaviour
{
    public enum ToggleRole { Custom, Music, SFX }

    [Header("UI References")]
    public Image fillImage;               // Background (Type = Filled, Horizontal)
    public Image handleImage;             // Knob
    public TextMeshProUGUI stateText;     // Optional

    [Header("Logic")]
    [SerializeField] private ToggleRole role = ToggleRole.Custom;
    [SerializeField] private bool isOn = true;        // trạng thái mặc định nếu không có prefs
    [SerializeField] private bool onLeftWhenOn = true; // ✅ ON ở bên trái

    [Header("Animation")]
    [SerializeField, Range(1f, 20f)] private float fillLerpSpeed = 5f;
    [SerializeField, Range(1f, 20f)] private float handleLerpSpeed = 10f;
    [SerializeField, Range(1f, 30f)] private float colorLerpSpeed = 10f;

    [Header("Layout")]
    [SerializeField] private bool alignToRoundedEnds = true;
    [SerializeField] private float edgeInset = 0f; // 0..2 là đẹp
    [SerializeField] private float padding = 6f;   // nếu không alignToRoundedEnds

    [Header("Colors")]
    public Color onTextColor = new Color(37f / 255f, 37f / 255f, 37f / 255f);
    public Color offTextColor = Color.white;
    public Color fillOnColor = new Color(0.25f, 0.8f, 0.35f);   // xanh
    public Color fillOffColor = new Color(0.95f, 0.35f, 0.35f);  // đỏ

    [Header("Events")]
    public UnityEvent<bool> onValueChanged; // Bạn vẫn có thể gắn ngoài Inspector

    // ===== private =====
    private RectTransform bgRect;
    private RectTransform handleRect;
    private float leftX, rightX;
    private float target; // 0..1 (đích nội suy theo hướng đã chọn)

    // PlayerPrefs keys theo vai trò
    private string PrefKeyEnabled =>
        role == ToggleRole.Music ? "MusicOn" :
        role == ToggleRole.SFX ? "SFXOn" : "CustomOn_" + name;

    private void Awake()
    {
        if (!fillImage || !handleImage)
        {
            Debug.LogWarning("[SwitchToggle] Fill/Handle chưa được gán.");
            enabled = false; return;
        }

        bgRect = fillImage.rectTransform;
        handleRect = handleImage.rectTransform;

        // neo theo mép trái để di chuyển theo X
        handleRect.anchorMin = new Vector2(0f, 0.5f);
        handleRect.anchorMax = new Vector2(0f, 0.5f);
        handleRect.pivot = new Vector2(0.5f, 0.5f);

        // load trạng thái đã lưu nếu có
        if (PlayerPrefs.HasKey(PrefKeyEnabled))
            isOn = PlayerPrefs.GetInt(PrefKeyEnabled, 1) == 1;
    }

    private void OnEnable()
    {
        StartCoroutine(InitNextFrame());
    }

    private IEnumerator InitNextFrame()
    {
        yield return null; // đợi Canvas tính kích thước
        RecalculateTravel();

        // hướng: nếu ON nằm bên trái → target = 0 khi isOn = true
        target = MapIsOnToTarget(isOn);

        ApplyInstant();
        UpdateLabel();
        ApplyFillColor(true);

        // áp dụng ngay vào AudioManager cho đúng frame đầu
        ApplyToAudio(isOn);
    }

    private void OnRectTransformDimensionsChange()
    {
        if (!isActiveAndEnabled || bgRect == null) return;
        RecalculateTravel();
        ApplyInstant();
    }

    private float MapIsOnToTarget(bool value)
    {
        // onLeftWhenOn = true → ON = trái (target 0); OFF = phải (target 1)
        // onLeftWhenOn = false → ngược lại
        if (onLeftWhenOn) return value ? 0f : 1f;
        else return value ? 1f : 0f;
    }

    private void RecalculateTravel()
    {
        float bgWidth = Mathf.Max(0f, bgRect.rect.width);
        float knobW = Mathf.Max(0f, handleRect.rect.width);

        if (alignToRoundedEnds)
        {
            float r = knobW * 0.5f;
            leftX = r + edgeInset;
            rightX = bgWidth - r - edgeInset;
        }
        else
        {
            float travel = Mathf.Max(0f, bgWidth - knobW - padding * 2f);
            leftX = padding + knobW * 0.5f;
            rightX = leftX + travel;
        }
    }

    private void Update()
    {
        // Lerp fill amount theo hướng ON-left
        float desiredFill = onLeftWhenOn ? 1f - target : target;
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, desiredFill, Time.deltaTime * fillLerpSpeed);

        // Lerp handle
        float from = onLeftWhenOn ? rightX : leftX;
        float to = onLeftWhenOn ? leftX : rightX;
        float targetX = Mathf.Lerp(from, to, 1f - target); // mapping cho mượt
        float newX = Mathf.Lerp(handleRect.anchoredPosition.x, targetX, Time.deltaTime * handleLerpSpeed);
        handleRect.anchoredPosition = new Vector2(newX, 0f);

        // Lerp màu fill
        ApplyFillColor(false);
    }

    // ===== Public API =====
    public void Toggle() => Set(!isOn, true);

    public void Set(bool value, bool invokeEvent = false)
    {
        if (isOn == value) return;
        isOn = value;
        target = MapIsOnToTarget(isOn);

        UpdateLabel();
        SavePref(isOn);
        ApplyToAudio(isOn);

        if (invokeEvent) onValueChanged?.Invoke(isOn);
    }

    public void SetInstant(bool value, bool invokeEvent = false)
    {
        isOn = value;
        target = MapIsOnToTarget(isOn);

        ApplyInstant();
        UpdateLabel();
        ApplyFillColor(true);
        SavePref(isOn);
        ApplyToAudio(isOn);

        if (invokeEvent) onValueChanged?.Invoke(isOn);
    }

    // ===== helpers =====
    private void ApplyInstant()
    {
        float desiredFill = onLeftWhenOn ? 1f - target : target;
        fillImage.fillAmount = desiredFill;

        float from = onLeftWhenOn ? rightX : leftX;
        float to = onLeftWhenOn ? leftX : rightX;
        float x = Mathf.Lerp(from, to, 1f - target);

        handleRect.anchoredPosition = new Vector2(x, 0f);
    }

    private void UpdateLabel()
    {
        if (!stateText) return;
        stateText.text = isOn ? "ON" : "OFF";
        stateText.color = isOn ? onTextColor : offTextColor;
    }

    private void ApplyFillColor(bool instant)
    {
        Color wanted = isOn ? fillOnColor : fillOffColor;
        fillImage.color = instant ? wanted : Color.Lerp(fillImage.color, wanted, Time.deltaTime * colorLerpSpeed);
    }

    private void SavePref(bool value)
    {
        PlayerPrefs.SetInt(PrefKeyEnabled, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void ApplyToAudio(bool value)
    {
        if (AudioManager.instance == null) return;

        switch (role)
        {
            case ToggleRole.Music:
                AudioManager.instance.ToggleMusic(value);
                break;
            case ToggleRole.SFX:
                AudioManager.instance.ToggleSFX(value);
                break;
        }
    }
}

