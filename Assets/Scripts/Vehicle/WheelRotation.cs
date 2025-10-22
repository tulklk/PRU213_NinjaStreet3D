using UnityEngine;

public class WheelRotation : MonoBehaviour
{
    public float rotationSpeed = 500f;

    void Update()
    {
        float moveAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward, moveAmount); 
    }
}
