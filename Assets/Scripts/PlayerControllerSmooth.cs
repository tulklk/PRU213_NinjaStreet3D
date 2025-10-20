
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerSmooth : MonoBehaviour
{
    public static PlayerControllerSmooth Instance { get; private set; }
    public static Transform PlayerTransform { get; private set; }

    private bool isAlive = true;
    private bool isGameStarted = false;
    private Rigidbody rb;

    [Header("Speed Control")]
    [SerializeField] private float speed = 7f;
    public float Speed { get => speed; set => speed = value; }
    public bool isBoosting = false;

    private float horizontalInput;
    private float screenWidth;
    private float tiltSensitivity = 3.0f;

    [Header("Turning")]
    private float maxTurnAngle = 30f;
    private float turnSpeed = 5f;

    [Header("Wheelie (Bốc đầu)")]
    [SerializeField] private bool canWheelie = true;
    [SerializeField] private float wheelieForce = 10f;
    [SerializeField] private float wheelieDuration = 2f;
    [SerializeField] private float wheelieCooldown = 3f;
    [SerializeField] private float wheelieAngle = 45f; // Góc bốc đầu (độ)
    [SerializeField] private float wheelieSpeed = 5f; // Tốc độ animation bốc đầu
    [SerializeField] private Transform motorcycleModel; // Mô hình xe máy để xoay
    private bool isWheelieing = false;
    private float wheelieTimer = 0f;
    private float wheelieCooldownTimer = 0f;
    private Quaternion originalRotation;
    private Quaternion wheelieRotation;

    // === GAMEPAD settings ===
    [Header("Gamepad")]
    [SerializeField] private float gamepadSensitivity = 1.0f;   // scale độ nhạy ngang
    [SerializeField] private float gamepadDeadzone = 0.15f;     // deadzone cho analog

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
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        screenWidth = Screen.width;
        
        // Tự động tìm mô hình xe máy nếu chưa gán
        if (motorcycleModel == null)
        {
            // Tìm đối tượng con có MeshRenderer (mô hình xe)
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length > 0)
            {
                motorcycleModel = renderers[0].transform;
                Debug.Log($"🎮 Tìm thấy mô hình xe: {motorcycleModel.name}");
            }
        }
        
        // Lưu rotation gốc cho animation bốc đầu
        if (motorcycleModel != null)
        {
            originalRotation = motorcycleModel.rotation;
            wheelieRotation = Quaternion.Euler(wheelieAngle, 0f, 0f);
        }
        else
        {
            originalRotation = transform.rotation;
            wheelieRotation = Quaternion.Euler(wheelieAngle, 0f, 0f);
        }
    }

    void Update()
    {
        if (!isGameStarted) return;

        HandleInput();
        UpdateWheelie();

        if (transform.position.y < -5)
            Die();
    }

    private void HandleInput()
    {
        horizontalInput = 0f;

        // --- Touch (legacy) ---
        foreach (UnityEngine.Touch touch in UnityEngine.Input.touches)
        {
            if (touch.phase == UnityEngine.TouchPhase.Began ||
                touch.phase == UnityEngine.TouchPhase.Stationary)
            {
                if (IsTouchOverUI(touch.fingerId)) continue;
                horizontalInput = (touch.position.x < screenWidth / 2) ? -1f : 1f;
            }
        }

        // --- Tilt ---
        float tilt = UnityEngine.Input.acceleration.x * tiltSensitivity;
        horizontalInput += Mathf.Clamp(tilt, -1f, 1f);

        // --- Gamepad (Input System) ---
#if ENABLE_INPUT_SYSTEM
        if (Gamepad.current != null)
        {
            float stickX = Gamepad.current.leftStick.ReadValue().x;
            if (Mathf.Abs(stickX) < gamepadDeadzone) stickX = 0f;
            float dpadX = Gamepad.current.dpad.ReadValue().x;
            float padX = Mathf.Clamp((stickX + dpadX) * gamepadSensitivity, -1f, 1f);
            horizontalInput += padX;
        }
#endif

        // --- Wheelie Input ---
        HandleWheelieInput();
    }


    private bool IsTouchOverUI(int fingerId)
    {
        return UnityEngine.EventSystems.EventSystem.current != null &&
               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(fingerId);
    }

    private void HandleTiltRotation()
    {
        float tiltAngle = maxTurnAngle * Mathf.Clamp(horizontalInput, -1f, 1f);
        Quaternion targetRotation = Quaternion.Euler(0f, tiltAngle, 0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    void FixedUpdate()
    {
        if (!isAlive || !isGameStarted) return;

        // Di chuyển thẳng + ngang (liên tục – hợp với analog)
        Vector3 moveDirection = transform.forward * speed * Time.fixedDeltaTime;
        moveDirection += transform.right * horizontalInput * speed * Time.fixedDeltaTime;

        Vector3 newPosition = transform.position + moveDirection;

        // Giới hạn biên X
        newPosition.x = Mathf.Clamp(newPosition.x, -9.5f, 9.5f);

        transform.position = newPosition;

        HandleTiltRotation();
    }

    public void Die()
    {
        isAlive = false;
        GameManager.instance.GameOver();
    }

    public void SetGameStarted(bool state) => isGameStarted = state;

    public void UpdateSpeedByDistance(float distance)
    {
        if (isBoosting) return;

        float newSpeed = 5f + Mathf.Floor(distance / 100f);
        newSpeed = Mathf.Min(newSpeed, 20f);

        if (Mathf.Abs(speed - newSpeed) > 0.01f)
            speed = newSpeed;
    }

    public Rigidbody GetRigidbody() => rb;

    // =========================
    // WHEELIE (BỐC ĐẦU) FUNCTIONS
    // =========================
    
    private void HandleWheelieInput()
    {
        // Bốc đầu bằng phím Space (backup)
        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            if (canWheelie && wheelieCooldownTimer <= 0f)
            {
                StartWheelie();
            }
        }
        
        // Gamepad input - nút A (buttonSouth)
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            if (canWheelie && wheelieCooldownTimer <= 0f)
            {
                StartWheelie();
            }
        }
        
        // Touch input - chạm đúp để bốc đầu
        foreach (UnityEngine.Touch touch in UnityEngine.Input.touches)
        {
            if (touch.tapCount >= 2 && touch.phase == UnityEngine.TouchPhase.Began)
            {
                if (canWheelie && wheelieCooldownTimer <= 0f)
                {
                    StartWheelie();
                }
                break;
            }
        }
    }
    
    // Input System callback cho Wheelie action
    public void OnWheelie(InputAction.CallbackContext context)
    {
        Debug.Log($"🎮 OnWheelie called: {context.phase}, canWheelie: {canWheelie}, cooldown: {wheelieCooldownTimer}");
        
        if (context.performed && canWheelie && wheelieCooldownTimer <= 0f)
        {
            StartWheelie();
        }
    }
    
    private void StartWheelie()
    {
        if (isWheelieing) return;
        
        isWheelieing = true;
        wheelieTimer = wheelieDuration;
        wheelieCooldownTimer = wheelieCooldown;
        
        // Áp dụng lực bốc đầu
        rb.AddTorque(transform.right * wheelieForce, ForceMode.Impulse);
        
        // Hiển thị UI bốc đầu
        if (UIManager.instance != null)
        {
            UIManager.instance.TurnWheelieUI();
        }
        
        Debug.Log("🔥 BỐC ĐẦU!");
    }
    
    private void UpdateWheelie()
    {
        // Cập nhật timer bốc đầu
        if (isWheelieing)
        {
            wheelieTimer -= Time.deltaTime;
            
            // Animation bốc đầu - xoay mô hình xe lên
            if (motorcycleModel != null)
            {
                Quaternion targetRotation = wheelieRotation;
                motorcycleModel.rotation = Quaternion.Lerp(motorcycleModel.rotation, targetRotation, Time.deltaTime * wheelieSpeed);
            }
            
            if (wheelieTimer <= 0f)
            {
                EndWheelie();
            }
        }
        else
        {
            // Animation hạ bánh - xoay mô hình xe về vị trí bình thường
            if (motorcycleModel != null)
            {
                if (motorcycleModel.rotation != originalRotation)
                {
                    motorcycleModel.rotation = Quaternion.Lerp(motorcycleModel.rotation, originalRotation, Time.deltaTime * wheelieSpeed);
                }
            }
        }
        
        // Cập nhật cooldown
        if (wheelieCooldownTimer > 0f)
        {
            wheelieCooldownTimer -= Time.deltaTime;
        }
    }
    
    private void EndWheelie()
    {
        isWheelieing = false;
        
        // Tắt UI bốc đầu
        if (UIManager.instance != null)
        {
            UIManager.instance.StopWheelieUI();
        }
        
        Debug.Log("💥 Hạ bánh trước!");
    }
    
    // Public methods để UI có thể hiển thị trạng thái
    public bool IsWheelieing() => isWheelieing;
    public float GetWheelieCooldown() => wheelieCooldownTimer;
    public bool CanWheelie() => canWheelie && wheelieCooldownTimer <= 0f;
}
