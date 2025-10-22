using UnityEngine;
using UnityEngine.UI;

public class CoinFlyEffect : MonoBehaviour
{
    public RectTransform targetUI;
    public float duration = 0.8f;

    private RectTransform rect;
    private Vector3 startPos;
    private float time;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.position;
    }

    void Update()
    {
        time += Time.deltaTime;
        float t = time / duration;
        rect.position = Vector3.Lerp(startPos, targetUI.position, t);

        if (t >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
