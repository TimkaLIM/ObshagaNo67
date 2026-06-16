using UnityEngine;
using UnityEngine.UI;   // ← ОБЯЗАТЕЛЬНО!

public class Driver : MonoBehaviour
{
    [Header("Настройки скорости")]
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float crouchSpeed = 2.5f;
    
    private float currentSpeed;

    [Header("Настройки Бега / Стамины")]
    public float maxRunTime = 5f;
    public float staminaRegenSpeed = 1f;
    private float currentStamina;
    private bool isExhausted = false;

    [Header("Настройки Приседа")]
    public float standHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSmoothTime = 8f;
    
    [Header("UI")]
    public Slider staminaSlider;   // ← СЮДА ПЕРЕТАЩИТЬ SLIDER
    
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        currentSpeed = walkSpeed;
        currentStamina = maxRunTime;
    }

    void Update()
    {
        HandleStamina();
        HandleHeight();
        
        // ОБНОВЛЕНИЕ СТАМИНЫ UI
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina / maxRunTime;
        }
    }

    void FixedUpdate()
    {
        CalculateCurrentSpeed();

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        move = move.normalized * currentSpeed;
        move.y = rb.linearVelocity.y; 
        rb.linearVelocity = move;
    }

    void CalculateCurrentSpeed()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && !isExhausted && !Input.GetKey(KeyCode.LeftControl) && 
                (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            currentSpeed = runSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }
    }

    void HandleStamina()
    {
        if (currentSpeed == runSpeed)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
                Debug.Log("[Стамина] Игрок выдохся!");
            }
        }
        else
        {
            if (currentStamina < maxRunTime)
            {
                currentStamina += Time.deltaTime * staminaRegenSpeed;

                if (isExhausted)
                {
                    if (!Input.GetKey(KeyCode.LeftShift) || currentStamina >= maxRunTime)
                    {
                        isExhausted = false;
                        Debug.Log("[Стамина] Игрок отдохнул!");
                    }
                }
            }
            else
            {
                currentStamina = maxRunTime;
            }
        }
    }

    void HandleHeight()
    {
        float targetHeight = Input.GetKey(KeyCode.LeftControl) ? crouchHeight : standHeight;

        if (capsuleCollider != null)
        {
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, targetHeight, Time.deltaTime * crouchSmoothTime);
        }
    }

    public bool IsExhaustedPublic()
    {
        return isExhausted;
    }

    public float GetCurrentSpeedPublic()
    {
        return currentSpeed;
    }

    public bool IsRunningPublic()
    {
        return currentSpeed == runSpeed && !isExhausted;
    }

    public bool IsCrouchingPublic()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }
}