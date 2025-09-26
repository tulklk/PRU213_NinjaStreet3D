using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Kích hoạt khiên bảo vệ!");
            GameManager.instance.ActivateShield();
            Destroy(gameObject);
        }

    }
}
