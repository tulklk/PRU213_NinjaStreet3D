using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Kích hoạt khiên bảo vệ!");
            GameManager.instance.ActivateMagnet();
            Destroy(gameObject);
        }
    }
}
