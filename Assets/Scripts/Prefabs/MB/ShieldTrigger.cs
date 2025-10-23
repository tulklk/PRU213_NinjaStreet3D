using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTrigger : MonoBehaviour
{
    public static ShieldTrigger Instance { get; private set; }
    [Header("Shield Effect")]
    public GameObject shieldEffect;     
    private bool isShieldActive = false;
    private int remainingShieldHits = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void ActivateShield()
    {
        StartCoroutine(ShieldRoutine());
    }
    IEnumerator ShieldRoutine()
    {
        isShieldActive = true;
        if (shieldEffect != null)
            shieldEffect.SetActive(true);

        int level = PlayerPrefs.GetInt("shield", 0);
        remainingShieldHits = Mathf.Clamp(level + 1, 1, 10); // VD: level 0 = 1 hit, level 1 = 2 hits...

        Debug.Log($"??? Khiên b?t! Ch?u ???c {remainingShieldHits} va ch?m.");

        // Không yield ? ?ây n?a — ??i ??n khi b? ch?m
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isShieldActive) return;

        if (other.CompareTag("Vehicle"))
        {
            remainingShieldHits--;
            Debug.Log($"?? Va ch?m! Shield còn l?i: {remainingShieldHits}");

            if (remainingShieldHits <= 0)
            {
                isShieldActive = false;
                if (shieldEffect != null)
                    shieldEffect.SetActive(false);

                Debug.Log("??? Khiên ?ã v? sau va ch?m!");
            }
        }
    }
}
