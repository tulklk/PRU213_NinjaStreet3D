using System.Collections;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float speed = 30f;                  
    public Vector3 direction = Vector3.forward; 

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    void Update()
    {
        transform.position += direction.normalized * speed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControllerSmooth.Instance.Die(); 
        }
        else
        {
            Debug.Log($"Khong cham đc Player");
        }

    }
}
