using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class UiAnimator 
{
    public static void Show(GameObject ui, float duration = 0.3f)
    {
        ui.SetActive(true);
        CanvasGroup group = ui.GetComponent<CanvasGroup>();
        if (group == null) group = ui.AddComponent<CanvasGroup>();

        group.alpha = 0f;
        group.transform.localScale = Vector3.one * 0.5f;

        group.DOFade(1f, duration).SetUpdate(true);
        group.transform.DOScale(1f, duration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public static void Hide(GameObject ui, float duration = 0.2f)
    {
        CanvasGroup group = ui.GetComponent<CanvasGroup>();
        if (group == null) group = ui.AddComponent<CanvasGroup>();

        group.DOFade(0f, duration).SetUpdate(true);
        group.transform.DOScale(0.5f, duration).SetEase(Ease.InBack).SetUpdate(true).OnComplete(() =>
        {
            ui.SetActive(false);
        });
    }

    public static void AnimateButtonsIn(float baseDelay, params Button[] buttons)
    {
        float delayStep = 0.1f;

        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            CanvasGroup group = btn.GetComponent<CanvasGroup>();
            if (group == null) group = btn.gameObject.AddComponent<CanvasGroup>();

            // Ghi nhớ vị trí gốc để reset trước mỗi lần chạy
            Vector2 originalPos = rect.anchoredPosition;

            // Di chuyển sang phải 200 để chuẩn bị chạy về
            rect.anchoredPosition = originalPos + Vector2.right * 200f;
            rect.localScale = Vector3.one;
            group.alpha = 0f;

            float delay = baseDelay + delayStep * System.Array.IndexOf(buttons, btn);

            group.DOFade(1f, 0.3f).SetDelay(delay).SetUpdate(true);
            rect.DOAnchorPos(originalPos, 0.5f)
                .SetEase(Ease.OutCubic)
                .SetDelay(delay)
                .SetUpdate(true);
        }
    }

    public static void AnimateButtonsOut(float baseDelay, params Button[] buttons)
    {
        float delayStep = 0.1f;

        foreach (Button btn in buttons)
        {
            RectTransform rect = btn.GetComponent<RectTransform>();
            CanvasGroup group = btn.GetComponent<CanvasGroup>();
            if (group == null) continue;

            float delay = baseDelay + delayStep * System.Array.IndexOf(buttons, btn);

            group.DOFade(0f, 0.2f).SetDelay(delay).SetUpdate(true);
            rect.DOAnchorPos(rect.anchoredPosition + Vector2.left * 200f, 0.3f)
                .SetEase(Ease.InBack)
                .SetDelay(delay)
                .SetUpdate(true);
        }
    }
}
