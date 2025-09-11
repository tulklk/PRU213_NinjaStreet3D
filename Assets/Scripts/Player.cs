using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Lane Settings")]
    [SerializeField] private float laneOffset = 2.5f;   // Khoảng cách giữa tâm 2 làn
    [SerializeField] private int startingLane = 1;      // 0 = Trái, 1 = Giữa, 2 = Phải

    [Header("Movement")]
    [SerializeField] private float laneChangeSpeed = 12f; 
    [SerializeField] private float forwardSpeed = 8f;     

    private int currentLane;   // 0..2
    private float targetX;     // vị trí X mục tiêu theo làn

    private void Start()
    {
        currentLane = Mathf.Clamp(startingLane, 0, 2);
        SetLane(currentLane);
        Vector3 pos = transform.position;
        pos.x = targetX;
        transform.position = pos;
    }

    private void Update()
    {
        // Input đổi làn
        if (Input.GetKeyDown(KeyCode.A)) ChangeLane(-1);
        if (Input.GetKeyDown(KeyCode.D)) ChangeLane(+1);

        // Chạy thẳng 
        if (forwardSpeed != 0f)
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime, Space.World);

        // Lướt sang X mục tiêu
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, targetX, laneChangeSpeed * Time.deltaTime);
        transform.position = pos;
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
