
//using System.Collections;
//using UnityEngine;

//#if ENABLE_INPUT_SYSTEM
//using UnityEngine.InputSystem; 
//#endif

//public class PlayerControllerSmooth : MonoBehaviour
//{
//    public static PlayerControllerSmooth Instance { get; private set; }
//    public static Transform PlayerTransform { get; private set; }

//    private bool isAlive = true;
//    private bool isGameStarted = false;
//    private Rigidbody rb;

//    [Header("Speed Control")]
//    [SerializeField] private float speed = 7f;
//    public float Speed { get => speed; set => speed = value; }
//    public bool isBoosting = false;

//    private float horizontalInput;
//    private float screenWidth;
//    private float tiltSensitivity = 3.0f;

//    [Header("Turning")]
//    private float maxTurnAngle = 30f;
//    private float turnSpeed = 5f;

//    // === GAMEPAD settings ===
//    [Header("Gamepad")]
//    [SerializeField] private float gamepadSensitivity = 1.0f;   // scale độ nhạy ngang
//    [SerializeField] private float gamepadDeadzone = 0.15f;     // deadzone cho analog

//    void Awake()
//    {
//        if (Instance != null && Instance != this)
//        {
//            Destroy(gameObject);
//            return;
//        }
//        Instance = this;
//        PlayerTransform = transform;
//    }

//    void Start()
//    {
//        rb = GetComponent<Rigidbody>();
//        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
//        screenWidth = Screen.width;
//    }

//    void Update()
//    {
//        if (!isGameStarted) return;

//        HandleInput();

//        if (transform.position.y < -5)
//            Die();
//    }

//    private void HandleInput()
//    {
//        horizontalInput = 0f;

//        // --- Touch (legacy) ---
//        foreach (UnityEngine.Touch touch in UnityEngine.Input.touches)
//        {
//            if (touch.phase == UnityEngine.TouchPhase.Began ||
//                touch.phase == UnityEngine.TouchPhase.Stationary)
//            {
//                if (IsTouchOverUI(touch.fingerId)) continue;
//                horizontalInput = (touch.position.x < screenWidth / 2) ? -1f : 1f;
//            }
//        }

//        // --- Tilt ---
//        float tilt = UnityEngine.Input.acceleration.x * tiltSensitivity;
//        horizontalInput += Mathf.Clamp(tilt, -1f, 1f);

//        // --- Gamepad (Input System) ---
//#if ENABLE_INPUT_SYSTEM
//        if (Gamepad.current != null)
//        {
//            float stickX = Gamepad.current.leftStick.ReadValue().x;
//            if (Mathf.Abs(stickX) < gamepadDeadzone) stickX = 0f;
//            float dpadX = Gamepad.current.dpad.ReadValue().x;
//            float padX = Mathf.Clamp((stickX + dpadX) * gamepadSensitivity, -1f, 1f);
//            horizontalInput += padX;
//        }
//#endif
//    }


//    private bool IsTouchOverUI(int fingerId)
//    {
//        return UnityEngine.EventSystems.EventSystem.current != null &&
//               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(fingerId);
//    }

//    private void HandleTiltRotation()
//    {
//        float tiltAngle = maxTurnAngle * Mathf.Clamp(horizontalInput, -1f, 1f);
//        Quaternion targetRotation = Quaternion.Euler(0f, tiltAngle, 0f);
//        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
//    }

//    void FixedUpdate()
//    {
//        if (!isAlive || !isGameStarted) return;

//        // Di chuyển thẳng + ngang (liên tục – hợp với analog)
//        Vector3 moveDirection = transform.forward * speed * Time.fixedDeltaTime;
//        moveDirection += transform.right * horizontalInput * speed * Time.fixedDeltaTime;

//        Vector3 newPosition = transform.position + moveDirection;

//        // Giới hạn biên X
//        newPosition.x = Mathf.Clamp(newPosition.x, -9.5f, 9.5f);

//        transform.position = newPosition;

//        HandleTiltRotation();
//    }

//    public void Die()
//    {
//        isAlive = false;
//        GameManager.instance.GameOver();
//    }

//    public void SetGameStarted(bool state) => isGameStarted = state;

//    public void UpdateSpeedByDistance(float distance)
//    {
//        if (isBoosting) return;

//        float newSpeed = 5f + Mathf.Floor(distance / 100f);
//        newSpeed = Mathf.Min(newSpeed, 20f);

//        if (Mathf.Abs(speed - newSpeed) > 0.01f)
//            speed = newSpeed;
//    }

//    public Rigidbody GetRigidbody() => rb;
//}


using System.Collections;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// === Alias để tránh trùng Input System ===
using LegacyInput = UnityEngine.Input;
using LegacyTouch = UnityEngine.Touch;
using LegacyTouchPhase = UnityEngine.TouchPhase;

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

    [Header("Wheelie Settings")]
    [SerializeField] private float wheelieAngle = 25f;      // góc bốc đầu
    [SerializeField] private float wheelieDuration = 2f;    // thời gian giữ bốc đầu
    [SerializeField] private float wheelieSpeed = 3f;       // tốc độ xoay
    private bool isWheelie = false;

    private Vector2 swipeStartPos;
    private bool isSwiping = false;

    // === GAMEPAD settings ===
    [Header("Gamepad")]
    [SerializeField] private float gamepadSensitivity = 1.0f;
    [SerializeField] private float gamepadDeadzone = 0.15f;

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
    }

    void Update()
    {
        if (!isGameStarted) return;
        HandleInput();

        // Kiểm tra vuốt màn hình
        HandleSwipe();

        if (transform.position.y < -5)
            Die();
    }

    private void HandleInput()
    {
        horizontalInput = 0f;

        // --- Legacy Touch ---
        foreach (LegacyTouch touch in LegacyInput.touches)
        {
            if (touch.phase == LegacyTouchPhase.Began ||
                touch.phase == LegacyTouchPhase.Stationary)
            {
                if (IsTouchOverUI(touch.fingerId)) continue;
                horizontalInput = (touch.position.x < screenWidth / 2) ? -1f : 1f;
            }
        }

        // --- Tilt (nghiêng điện thoại) ---
        float tilt = LegacyInput.acceleration.x * tiltSensitivity;
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
    }

    private void HandleSwipe()
    {
        if (LegacyInput.touchCount > 0)
        {
            LegacyTouch t = LegacyInput.GetTouch(0);

            if (t.phase == LegacyTouchPhase.Began)
            {
                swipeStartPos = t.position;
                isSwiping = true;
            }
            else if (t.phase == LegacyTouchPhase.Ended && isSwiping)
            {
                Vector2 swipeDelta = t.position - swipeStartPos;
                // Vuốt lên: y > x
                if (swipeDelta.magnitude > 50f && swipeDelta.y > Mathf.Abs(swipeDelta.x))
                {
                    StartCoroutine(DoWheelie());
                }
                isSwiping = false;
            }
        }
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

        // Di chuyển thẳng + ngang
        Vector3 moveDirection = transform.forward * speed * Time.fixedDeltaTime;
        moveDirection += transform.right * horizontalInput * speed * Time.fixedDeltaTime;

        Vector3 newPosition = transform.position + moveDirection;
        newPosition.x = Mathf.Clamp(newPosition.x, -9.5f, 9.5f);
        transform.position = newPosition;

        if (!isWheelie)
            HandleTiltRotation();
    }

    IEnumerator DoWheelie()
    {
        if (isWheelie) yield break;
        isWheelie = true;

        Quaternion startRot = transform.rotation;
        Quaternion wheelieRot = Quaternion.Euler(-wheelieAngle, transform.eulerAngles.y, transform.eulerAngles.z);

        float elapsed = 0f;
        while (elapsed < wheelieDuration / 2f)
        {
            transform.rotation = Quaternion.Lerp(startRot, wheelieRot, elapsed * wheelieSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.3f);

        elapsed = 0f;
        while (elapsed < wheelieDuration / 2f)
        {
            transform.rotation = Quaternion.Lerp(wheelieRot, startRot, elapsed * wheelieSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isWheelie = false;
    }

    public void Die()
    {
        isAlive = false;
        GameManager.instance.GameOver();
    }

    public void SetGameStarted(bool state)
    {
        isGameStarted = state;
        if (state)
            StartCoroutine(AutoWheelieAfterDelay());
    }

    IEnumerator AutoWheelieAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        if (isAlive && isGameStarted)
            StartCoroutine(DoWheelie());
    }

    public void UpdateSpeedByDistance(float distance)
    {
        if (isBoosting) return;
        float newSpeed = 5f + Mathf.Floor(distance / 100f);
        newSpeed = Mathf.Min(newSpeed, 20f);
        if (Mathf.Abs(speed - newSpeed) > 0.01f)
            speed = newSpeed;
    }

    public Rigidbody GetRigidbody() => rb;
}

