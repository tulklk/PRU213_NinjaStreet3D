using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheelRotate : MonoBehaviour
{
    [Header("Gắn từng bánh xe vào đây")]
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public Transform rearLeftWheel;
    public Transform rearRightWheel;

    [Header("Tốc độ quay bánh xe")]
    public float rotationSpeed = 500f;

    [Header("Lấy tốc độ từ Rigidbody (nếu có)")]
    public Rigidbody carRigidbody;

    void Update()
    {
        float speed = rotationSpeed;

        // Nếu có Rigidbody, dùng tốc độ thực
        if (carRigidbody != null)
        {
            speed = carRigidbody.velocity.magnitude * rotationSpeed;
        }

        RotateWheel(frontLeftWheel, speed);
        RotateWheel(frontRightWheel, speed);
        RotateWheel(rearLeftWheel, speed);
        RotateWheel(rearRightWheel, speed);
    }

    void RotateWheel(Transform wheel, float speed)
    {
        if (wheel != null)
        {
            wheel.Rotate(Vector3.right, speed * Time.deltaTime, Space.Self);
        }
    }
}
