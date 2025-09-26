using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    public float boostForce = 1000f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 launchDirection = (transform.forward + transform.up).normalized;
                rb.AddForce(launchDirection * boostForce, ForceMode.Impulse);
            }
        }
    }
}

