using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RacingBoyController : MonoBehaviour
{
    public float launchForce = 500f;
    public float deactivateDelay = 3f;
    private bool hasLaunched = false;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void LaunchTowards(Vector3 direction)
    {
        if (hasLaunched) return;

        hasLaunched = true;
        rb.velocity = Vector3.zero;
        rb.AddForce(direction.normalized * launchForce, ForceMode.Impulse);
        StartCoroutine(DeactivateAfter(deactivateDelay));
    }

    IEnumerator DeactivateAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
