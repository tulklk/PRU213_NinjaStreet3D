
using UnityEngine;
using DG.Tweening;

public class DriftTrigger : MonoBehaviour
{
    [Header("Điểm kết thúc của drift")]
    public Transform endPoint;

    [Header("Thời gian di chuyển")]
    public float duration = 1.2f;

    private bool hasDrifted = false;
    public static bool IsDrifting = false;

    private void OnTriggerEnter(Collider other)
    {
       
        if (hasDrifted || !other.CompareTag("Player")) return;

        DriftTrigger.IsDrifting = true;

        
        Transform vehicle = transform.parent;

        if (vehicle != null)
        {
            vehicle.DOMoveX(endPoint.position.x, duration)
                .SetEase(Ease.OutSine)
                .OnStart(() => Debug.Log("🚘 Xe đang drift vào phía Player..."))
                .OnComplete(() => Debug.Log("✅ Xe đã drift xong."));

            hasDrifted = true;
        }
    }
}
