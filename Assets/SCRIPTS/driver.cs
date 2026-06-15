using UnityEngine;

public class Driver : MonoBehaviour
{
    [Header("Настройки скорости")]
    public float walkSpeed = 5f;       // Обычная скорость ходьбы
    public float runSpeed = 9f;        // Скорость бега (настраиваемая)
    public float crouchSpeed = 2.5f;   // Скорость в приседе (настраиваемая)
    
    private float currentSpeed;        // Текущая скорость в данный момент

    [Header("Настройки Бега / Стамины")]
    public float maxRunTime = 5f;      // Сколько секунд можно бежать (настраиваемое)
    public float staminaRegenSpeed = 1f; // Как быстро восстанавливается выносливость (коэффициент)
    private float currentStamina;      // Текущее время бега в запасе
    private bool isExhausted = false;  // Устал ли игрок полностью?

    [Header("Настройки Приседа")]
    public float standHeight = 2f;     // Обычная высота игрока
    public float crouchHeight = 1f;    // Высота игрока в приседе
    public float crouchSmoothTime = 8f; // Плавность приседания
    
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider; // Нужен для изменения физического роста

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        currentSpeed = walkSpeed;
        currentStamina = maxRunTime; // В начале игры стамина полная
    }

    void Update()
    {
        // Логику стамины и роста лучше считать в Update, чтобы таймеры шли ровно секунда в секунду
        HandleStamina();
        HandleHeight();
    }

    void FixedUpdate()
    {
        // Выбираем скорость в зависимости от зажатых кнопок
        CalculateCurrentSpeed();

        Vector3 move = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) move += transform.forward;
        if (Input.GetKey(KeyCode.S)) move -= transform.forward;
        if (Input.GetKey(KeyCode.A)) move -= transform.right;
        if (Input.GetKey(KeyCode.D)) move += transform.right;

        // Нормализуем движение и умножаем на текущую скорость (ходьба/бег/присед)
        move = move.normalized * currentSpeed;

        // Сохраняем гравитацию (для Unity 6 используется rb.linearVelocity, в старых версиях rb.velocity)
        move.y = rb.linearVelocity.y; 

        rb.linearVelocity = move;
    }

    // Определение текущей скорости
    void CalculateCurrentSpeed()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed = crouchSpeed;
        }
        // Изменили условие: теперь мы бежим только если зажат Shift, мы НЕ устали, двигаемся И при этом НЕ зажат Ctrl
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

    // Логика выносливости (5 секунд)
    void HandleStamina()
    {
        // Если мы физически бежим — тратим стамину
        if (currentSpeed == runSpeed)
        {
            currentStamina -= Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true; // Выдохся!
                Debug.Log("[Стамина] Игрок полностью выдохся. Отпустите Shift!");
            }
        }
        // Если мы НЕ бежим (идем или стоим) — восстанавливаем силы
        else
        {
            if (currentStamina < maxRunTime)
            {
                // Стамина восстанавливается плавно
                currentStamina += Time.deltaTime * staminaRegenSpeed;

                // ХАК: Если игрок устал, он обязан ОТПУСТИТЬ кнопку Shift, чтобы начать переводить дух,
                // либо стамина должна восстановиться до самого конца (до 100%)
                if (isExhausted)
                {
                    // Если игрок отпустил Shift ИЛИ стамина восстановилась полностью
                    if (!Input.GetKey(KeyCode.LeftShift) || currentStamina >= maxRunTime)
                    {
                        isExhausted = false;
                        Debug.Log("[Стамина] Игрок отдохнул. Бег снова доступен!");
                    }
                }
            }
            else
            {
                currentStamina = maxRunTime;
            }
        }
    }

    // Логика плавного изменения роста игрока
    void HandleHeight()
    {
        float targetHeight = Input.GetKey(KeyCode.LeftControl) ? crouchHeight : standHeight;

        if (capsuleCollider != null)
        {
            // Плавно меняем высоту нашего коллайдера (чтобы пролезать под препятствиями)
            capsuleCollider.height = Mathf.Lerp(capsuleCollider.height, targetHeight, Time.deltaTime * crouchSmoothTime);
        }
    }

// Добавьте ЭТИ МЕТОДЫ в конец вашего скрипта Driver (перед последней скобкой)

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

