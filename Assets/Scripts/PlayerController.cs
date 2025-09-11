using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public static Transform PlayerTransform { get; private set; }

    private bool isAlive = true;
    private bool isGameStarted = false;
    private Rigidbody rb;

    [Header("Speed Control")]
    [SerializeField] private float speed = 7f;
    //[SerializeField] private float rotationSmoothness = 5f;
    public float Speed
    {
        get => speed;
        set => speed = value;
    }
    public bool isBoosting = false;

    private float horizontalInput;
    private float screenWidth;
    private float tiltSensitivity = 3.0f;

    [Header("Turning")]
    private float maxTurnAngle = 30f;
    private float turnSpeed = 5f;

    //private bool isSpeedBoosted = false;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        PlayerTransform = transform;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                     RigidbodyConstraints.FreezeRotationZ;
        screenWidth = Screen.width;

        //if (coinDetector != null)
        //    coinDetector.SetActive(false);

        //if (shieldEffect != null)
        //    shieldEffect.SetActive(false);
    }

    void Update()
    {
        if (!isGameStarted) return;

        HandleInput();
        //HandleTiltRotation();

        if (transform.position.y < -5)
        {
            //Die();
            Time.timeScale = 0f;
        }
    }
    private void HandleInput()
    {
        horizontalInput = 0f;

        foreach (Touch touch in Input.touches)
        {
            // ❗ Nếu chạm vào UI thì bỏ qua
            if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
            {
                if (IsTouchOverUI(touch.fingerId))
                    continue;

                if (touch.position.x < screenWidth / 2)
                    horizontalInput = -1f;
                else
                    horizontalInput = 1f;
            }
        }

        float tilt = Input.acceleration.x * tiltSensitivity;
        horizontalInput += Mathf.Clamp(tilt, -1f, 1f);
    }
    private bool IsTouchOverUI(int fingerId)
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(fingerId);
    }
    private void HandleTiltRotation()
    {
        float tiltAngle = maxTurnAngle * horizontalInput;
        Quaternion targetRotation = Quaternion.Euler(0f, tiltAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    void FixedUpdate()
    {
        if (!isAlive || !isGameStarted) return;

        Vector3 moveDirection = transform.forward * speed * Time.fixedDeltaTime;
        moveDirection += transform.right * horizontalInput * speed * Time.fixedDeltaTime;

        Vector3 newPosition = transform.position + moveDirection;

        // ✅ Giới hạn vùng di chuyển (ví dụ từ -3 đến +3 theo trục X)
        newPosition.x = Mathf.Clamp(newPosition.x, -9.5f, 9.5f);

        transform.position = newPosition;

        HandleTiltRotation();
    }
    //public void Die()
    //{
    //    isAlive = false;
    //    GameManager.instance.GameOver();
    //}

    public void SetGameStarted(bool state)
    {
        isGameStarted = state;
    }
    public void UpdateSpeedByDistance(float distance)
    {
        if (isBoosting) return;

        float newSpeed = 5f + Mathf.Floor(distance / 100f);
        newSpeed = Mathf.Min(newSpeed, 20f);

        if (Mathf.Abs(speed - newSpeed) > 0.01f)
        {
            speed = newSpeed;
        }
    }

    public Rigidbody GetRigidbody()
    {
        return rb;
    }

    
}
