using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketPowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Kích hoạt khiên bảo vệ!");
            GameManager.instance.ActivateRocket();
            Destroy(gameObject);
        }

    }
}
