using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorZoneTrigger : MonoBehaviour
{
    public enum TriggerType { MoveZone, KillZone }
    [SerializeField] private TriggerType triggerType; // thêm dòng này ?? Unity hi?n dropdown

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MotorMoveToward motoScript = GetComponentInParent<MotorMoveToward>();

        if (triggerType == TriggerType.MoveZone)
        {
            motoScript.StartMoving(); // báo xe b?t ??u ch?y
        }
        else if (triggerType == TriggerType.KillZone)
        {
            motoScript.KillPlayer();
        }
    }
}
