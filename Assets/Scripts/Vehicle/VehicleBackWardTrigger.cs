using UnityEngine;

public class VehicleBackWardTrigger : MonoBehaviour
{
 
    public enum TriggerType { MoveZone, KillZone, PhysicsLeft, PhysicsRight }
    [SerializeField] private TriggerType triggerType; // thêm dòng này ?? Unity hi?n dropdown


    [Header("Explosion Settings")]
    public ParticleSystem explosionEffect;
    public Transform explosionEffectPoint;


    private void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("Player") && !other.CompareTag("Rocket") && !other.CompareTag("Shield")) return;

        VehicleBackWard vehicleBackWardScript = GetComponentInParent<VehicleBackWard>();

        switch (triggerType)
        {
            case TriggerType.MoveZone:
                vehicleBackWardScript?.StartMoving();
                break;

            case TriggerType.KillZone:
                if (other.CompareTag("Player"))
                {
                
                    if (PlayerPowerUp.Instance != null && PlayerPowerUp.Instance.IsShieldActive())
                    {
                        bool absorbed = PlayerPowerUp.Instance.AbsorbHitWithShield();
                        if (absorbed)
                        {
                            Debug.Log("🛡️ KillZone: Đỡ sát thương bằng khiên!");
                            break; 
                        }
                    }
                    vehicleBackWardScript?.KillPlayer();
                }
                else if (other.CompareTag("Shield") || other.CompareTag("Rocket"))
                {
                    if (explosionEffect != null)
                    {
                        ParticleSystem explosion = Instantiate(explosionEffect, explosionEffectPoint.position, Quaternion.identity);
                        explosion.Play();
                        Destroy(explosion.gameObject, 2f);
                    }

                    Destroy(transform.parent.gameObject);
                }
                break;


            case TriggerType.PhysicsLeft:
                if (other.CompareTag("Player") && PlayerEffect.Instance != null)
                    PlayerEffect.Instance.sparkEffectRight?.SetActive(true);
                break;
            case TriggerType.PhysicsRight:
                if (other.CompareTag("Player") && PlayerEffect.Instance != null)
                    PlayerEffect.Instance.sparkEffectLeft?.SetActive(true);
                break;


            default:
                Debug.LogWarning($"[TriggerZone] Chưa xử lý loại trigger: {triggerType}");
                break;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (triggerType)
        {
            case TriggerType.MoveZone:
                Destroy(transform.parent.gameObject);
                break;

            case TriggerType.PhysicsLeft:
                if (other.CompareTag("Player") && PlayerEffect.Instance != null)
                    PlayerEffect.Instance.sparkEffectRight?.SetActive(false);
                break;
            case TriggerType.PhysicsRight:
                if (other.CompareTag("Player") && PlayerEffect.Instance != null)
                    PlayerEffect.Instance.sparkEffectLeft?.SetActive(false);
                break;
        }
    }
}
