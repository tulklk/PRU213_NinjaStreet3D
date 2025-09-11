using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTest : MonoBehaviour
{
    
    [Header("Lane Settings")]
    [SerializeField] private float laneOffset = 2.5f;   // Khoảng cách giữa tâm 2 làn
    [SerializeField] private int startingLane = 1;      // 0 = Trái, 1 = Giữa, 2 = Phải

    [Header("Movement")]
    [SerializeField] private float laneChangeSpeed = 12f; // Tốc độ lướt sang làn
    [SerializeField] private float forwardSpeed = 8f;     // 0 nếu không muốn tự chạy thẳng

    private int currentLane;   // 0..2
    private float targetX;     // vị trí X mục tiêu theo làn

    public static Transform PlayerTransform { get; private set; }
    private void Start()
    {
        currentLane = Mathf.Clamp(startingLane, 0, 2);
        SetLane(currentLane);
        // Đặt ngay đúng X ban đầu
        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;
    }

    private void Update()
    {
        Turn();
        MoveForward();
        // Lướt sang X mục tiêu
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = pos;
    }
    private void MoveForward() 
    {
        // Chạy thẳng (nếu muốn)
        if (forwardSpeed != 0f)
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);
    }
    private void Turn() 
    {
        // Input đổi làn
        if (Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeLane(+1);
    }
    private void ChangeLane(int dir)
    {
        currentLane = Mathf.Clamp(currentLane + dir, 0, 2);
        SetLane(currentLane);
    }

    private void SetLane(int lane)
    {
        // Giữa = 0, Trái = -laneOffset, Phải = +laneOffset
        targetX = (lane - 1) * laneOffset;
    }
}
