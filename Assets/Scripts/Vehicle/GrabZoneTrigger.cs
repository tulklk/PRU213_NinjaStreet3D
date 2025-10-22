using UnityEngine;

public class GrabZoneTrigger : MonoBehaviour
{
    public enum TriggerType { MoveZone, KillZone }
    [SerializeField] private TriggerType triggerType; 

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Grab grabScript = GetComponentInParent<Grab>();

        if (triggerType == TriggerType.MoveZone)
        {
            grabScript.StartMoving();
        }
        else if (triggerType == TriggerType.KillZone)
        {
            grabScript.KillPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && triggerType == TriggerType.MoveZone)
        {
            Destroy(transform.parent.gameObject); 
        }
    }
}
