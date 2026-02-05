using UnityEngine;

public class Note : MonoBehaviour
{
    // --- Timing ---
    public float targetBeat;
    public FacingDirection noteDirection;

    [Header("Timing Windows")]
    public float perfectWindow = 0.5f;
    public float goodWindow = 0.25f;
    public float missWindow = 0.4f;

    // --- Daño por fallo ---
    [Header("Damage Settings")]
    public int missDamage = 1;          // Daño cuando fallas (Miss)
    public int wrongSideDamage = 1;     // Daño cuando presionas dirección incorrecta

    // --- State ---
    public bool alreadyHit = false;
    private bool initialized = false;

    // --- Movement ---
    private Vector3 targetPosition;
    public float moveSpeed = 6f;

    // --- Approach Circle ---
    [Header("Approach Circle")]
    public Transform approachCircle;
    public float approachMaxMultiplier = 2.5f;
    public float approachMinMultiplier = 0.6f;

    private Vector3 approachBaseScale;
    private float startDistance;

    // --- Events ---
    public event System.Action<HitResult, FacingDirection> OnHitResult;

    // --- Referencia al sistema de vida ---
    private PlayerHealth playerHealth;

    // --- Init ---
    public void Initialize(float _targetBeat, Vector3 _targetPosition, FacingDirection _direction)
    {
        Debug.Log("========================================");
        Debug.Log("INITIALIZE NOTA");
        Debug.Log($"Beat objetivo: {_targetBeat}");
        Debug.Log($"Posición objetivo: {_targetPosition}");
        Debug.Log($"Dirección: {_direction}");

        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        noteDirection = _direction;
        initialized = true;

        // Buscar la referencia al sistema de vida del jugador
        if (playerHealth == null)
        {
            Debug.Log("Buscando PlayerHealth...");
            playerHealth = FindAnyObjectByType<PlayerHealth>();

            if (playerHealth != null)
            {
                Debug.Log($"✓ PlayerHealth ENCONTRADO en: {playerHealth.gameObject.name}");
                Debug.Log($"Vida máxima: {playerHealth.maxHealth}");
            }
            else
            {
                Debug.LogError("✗ NO se encontró PlayerHealth!");
            }
        }

        if (approachCircle != null)
        {
            approachBaseScale = approachCircle.localScale;
            startDistance = Vector3.Distance(transform.position, targetPosition);
            approachCircle.localScale = approachBaseScale * approachMaxMultiplier;
        }

        Debug.Log("========================================\n");
    }

    void Start()
    {
        Debug.Log($"NOTA START - Objeto: {gameObject.name}");
    }

    void Update()
    {
        if (!initialized || alreadyHit)
            return;

        // Verificar Conductor
        if (Conductor.instance == null)
        {
            Debug.LogError("Conductor.instance es NULL!");
            return;
        }

        float songBeat = Conductor.instance.songPositionInBeats;

        // Debug cada segundo para ver el beat actual
        if (Time.frameCount % 60 == 0) // Cada segundo aprox (60fps)
        {
            Debug.Log($"Beat actual: {songBeat:F2}, Beat objetivo: {targetBeat:F2}, Diferencia: {songBeat - targetBeat:F2}");
        }

        // --- Move towards hit point ---
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // --- Approach circle scaling ---
        if (approachCircle != null && startDistance > 0f)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPosition);
            float t = Mathf.Clamp01(currentDistance / startDistance);
            float multiplier = Mathf.Lerp(approachMinMultiplier, approachMaxMultiplier, t);
            approachCircle.localScale = approachBaseScale * multiplier;
        }

        // --- Auto miss ---
        float beatDifference = songBeat - targetBeat;
        if (beatDifference > missWindow)
        {
            Debug.Log($"⚠️ AUTO MISS DETECTADO");
            Debug.Log($"Beat diferencia: {beatDifference:F2}, Miss Window: {missWindow}");
            Miss();
        }
        else if (beatDifference > 0)
        {
            Debug.Log($"Beat diferencia positiva: {beatDifference:F2}");
        }
    }

    // --- Hit logic ---
    public HitResult TryHit(FacingDirection playerFacing)
    {
        Debug.Log("========================================");
        Debug.Log("TRYHIT LLAMADO");
        Debug.Log($"Jugador mira: {playerFacing}");
        Debug.Log($"Nota mira: {noteDirection}");
        Debug.Log($"Ya golpeada: {alreadyHit}");

        if (alreadyHit)
        {
            Debug.Log("Nota ya golpeada, ignorando...");
            Debug.Log("========================================\n");
            return HitResult.None;
        }

        if (playerFacing != noteDirection)
        {
            Debug.Log("✗ DIRECCIÓN INCORRECTA!");
            Debug.Log($"Daño a aplicar: {wrongSideDamage}");
            // Dirección incorrecta - aplicar daño
            ApplyDamage(wrongSideDamage);
            ResolveHit(HitResult.WrongSide);
            Debug.Log("========================================\n");
            return HitResult.WrongSide;
        }

        float songBeat = Conductor.instance.songPositionInBeats;
        float error = Mathf.Abs(songBeat - targetBeat);

        Debug.Log($"Beat actual: {songBeat:F3}");
        Debug.Log($"Beat objetivo: {targetBeat:F3}");
        Debug.Log($"Error absoluto: {error:F3}");
        Debug.Log($"Perfect Window: {perfectWindow}");
        Debug.Log($"Good Window: {goodWindow}");
        Debug.Log($"Miss Window: {missWindow}");

        HitResult result = HitResult.None;

        if (error <= perfectWindow)
        {
            result = HitResult.Perfect;
            Debug.Log("✓ PERFECT!");
        }
        else if (error <= goodWindow)
        {
            result = HitResult.Good;
            Debug.Log("✓ GOOD");
        }
        else if (error <= missWindow)
        {
            result = HitResult.Miss;
            Debug.Log("✗ MISS (dentro de ventana)");
        }
        else
        {
            Debug.Log("Fuera de todas las ventanas");
        }

        if (result != HitResult.None)
        {
            // Si es un miss, aplicar daño
            if (result == HitResult.Miss)
            {
                Debug.Log($"Aplicando daño por miss: {missDamage}");
                ApplyDamage(missDamage);
            }
            else
            {
                Debug.Log($"Resultado {result} - Sin daño");
            }

            ResolveHit(result);
        }

        Debug.Log("========================================\n");
        return result;
    }

    // --- Aplicar daño al jugador ---
    private void ApplyDamage(int damageAmount)
    {
        Debug.Log("========================================");
        Debug.Log("APPLY DAMAGE");
        Debug.Log($"Cantidad: {damageAmount}");

        if (damageAmount <= 0)
        {
            Debug.Log("Daño es 0 o negativo, ignorando...");
            Debug.Log("========================================\n");
            return;
        }

        // Verificar referencia
        if (playerHealth == null)
        {
            Debug.LogWarning("playerHealth es NULL, buscando de nuevo...");
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            Debug.Log($"✓ PlayerHealth válido: {playerHealth.gameObject.name}");
            Debug.Log("Llamando a TakeDamage...");
            playerHealth.TakeDamage(damageAmount);
        }
        else
        {
            Debug.LogError("✗ PlayerHealth sigue siendo NULL!");
            Debug.Log("¿Está el objeto con PlayerHealth en la escena?");
            Debug.Log("¿Está activo el GameObject?");
        }

        Debug.Log("========================================\n");
    }

    // --- Results ---
    private void ResolveHit(HitResult result)
    {
        Debug.Log($"ResolveHit: {result}");
        alreadyHit = true;

        // Verificar si el evento tiene suscriptores
        if (OnHitResult != null)
        {
            Debug.Log($"Evento OnHitResult tiene {OnHitResult.GetInvocationList().Length} suscriptores");
            OnHitResult?.Invoke(result, noteDirection);
        }
        else
        {
            Debug.LogWarning("OnHitResult no tiene suscriptores");
        }

        Debug.Log($"Destruyendo nota: {gameObject.name}");
        Destroy(gameObject);
    }

    private void Miss()
    {
        Debug.Log("========================================");
        Debug.Log("MISS AUTOMÁTICO");
        Debug.Log($"Nota beat: {targetBeat}");
        Debug.Log($"Daño: {missDamage}");
        Debug.Log($"Posición actual: {transform.position}");
        Debug.Log($"Posición objetivo: {targetPosition}");

        alreadyHit = true;

        // Aplicar daño por miss automático (no presionar nada)
        ApplyDamage(missDamage);

        // Verificar evento
        if (OnHitResult != null)
        {
            OnHitResult?.Invoke(HitResult.Miss, noteDirection);
        }

        Debug.Log($"Destruyendo nota por miss: {gameObject.name}");
        Destroy(gameObject);
        Debug.Log("========================================\n");
    }

    void OnDestroy()
    {
        Debug.Log($"Nota DESTRUIDA: {gameObject.name} - Ya golpeada: {alreadyHit}");
    }
}

