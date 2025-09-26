using UnityEngine;
using UnityEngine.UI;

public class ItemEffectSlider : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Image iconImage;
    public Gradient colorGradient;

    private float duration;
    private float timeLeft;
    private bool isRunning = false;

    public void StartCountdown(Sprite icon, float time)
    {
        iconImage.sprite = icon;
        duration = time;
        timeLeft = time;

        slider.maxValue = time;
        slider.value = time;

        isRunning = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.unscaledDeltaTime;
        slider.value = timeLeft;

        float t = 1f - (timeLeft / duration);
        fillImage.color = colorGradient.Evaluate(t);

        if (timeLeft <= 0)
        {
            isRunning = false;
            gameObject.SetActive(false);
        }
    }
}
