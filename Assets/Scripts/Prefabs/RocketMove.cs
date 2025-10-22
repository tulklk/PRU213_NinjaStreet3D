using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketMove : MonoBehaviour
{
    public float speed = 20f;
    public ParticleSystem fireEffectPrefab; 
    public Transform particlePoint;        

    private ParticleSystem fireEffectInstance;

    void Start()
    {
        if (fireEffectPrefab != null && particlePoint != null)
        {
            fireEffectInstance = Instantiate(fireEffectPrefab, particlePoint.position, particlePoint.rotation, particlePoint);
            if (!fireEffectInstance.gameObject.activeSelf)
            {
                fireEffectInstance.gameObject.SetActive(true);
            }
            fireEffectInstance.Play();
        }
        else
        {
            Debug.LogWarning("🔥 Chưa gán FireEffect Prefab hoặc Particle Point!");
        }
    }


    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            Destroy(gameObject);
        }
    }
}
