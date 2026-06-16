using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Настройки мыши")]
    public float mouseSensivity = 100f;
    public Transform playerBody;

    [Header("Настройки тряски при беге")]
    public float bobSpeed = 10f;
    public float bobAmount = 0.05f;
    public float sprintMultiplier = 1.5f; // Тряска сильнее при беге

    private float xRotation = 0f;
    private float bobTimer = 0f;
    private Vector3 originalCameraPos;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalCameraPos = transform.localPosition;
    }

    void Update()
    {
        // --- ПОВОРОТ КАМЕРЫ (обычный код) ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        // --- ТРЯСКА ПРИ БЕГЕ (добавлено) ---
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || 
                        Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        if (isMoving && isRunning)
        {
            // Бег — тряска сильнее
            float currentSpeed = bobSpeed * sprintMultiplier;
            float currentAmount = bobAmount * 1.5f;
            
            bobTimer += Time.deltaTime * currentSpeed;
            float bobY = Mathf.Sin(bobTimer) * currentAmount;
            float bobX = Mathf.Cos(bobTimer * 0.5f) * currentAmount * 0.5f;
            
            transform.localPosition = originalCameraPos + new Vector3(bobX, bobY, 0);
        }
        else if (isMoving)
        {
            // Ходьба — слабая тряска
            bobTimer += Time.deltaTime * bobSpeed;
            float bobY = Mathf.Sin(bobTimer) * bobAmount;
            float bobX = Mathf.Cos(bobTimer * 0.5f) * bobAmount * 0.5f;
            
            transform.localPosition = originalCameraPos + new Vector3(bobX, bobY, 0);
        }
        else
        {
            // Стоим — без тряски
            transform.localPosition = originalCameraPos;
            bobTimer = 0f;
        }
    }
}