using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth; // Cambiado a serialized para ver en Inspector
    private HealthBar healthBar;

    void Awake()
    {
        Debug.Log("🔵 PLAYERHEALTH - Awake llamado");
    }

    void Start()
    {
        currentHealth = maxHealth;

        healthBar = GetComponentInChildren<HealthBar>();
        Debug.Log("========================================");
        Debug.Log("🟢 PLAYERHEALTH - Start");
        Debug.Log($"   Objeto: {gameObject.name}");
        Debug.Log($"   Vida inicializada: {currentHealth}/{maxHealth}");
        Debug.Log($"   Instancia: {GetInstanceID()}");
        Debug.Log("========================================\n");
    }
 
    void OnEnable()
    {
        Debug.Log("🟡 PLAYERHEALTH - OnEnable");
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("========================================");
        Debug.Log("🔴 PLAYERHEALTH - TakeDamage INICIADO");
        Debug.Log($"   Método llamado por: {GetCallerInfo()}");
        Debug.Log($"   Parámetro damageAmount: {damageAmount}");
        Debug.Log($"   Vida ANTES del daño: {currentHealth}");

        // Verificación EXTRA del parámetro
        if (damageAmount <= 0)
        {
            Debug.LogError($"   ⚠️ ERROR: damageAmount es {damageAmount} (debe ser > 0)");
            Debug.Log("========================================\n");
            return;
        }

        // Aplicar el daño
        int vidaAnterior = currentHealth;
        currentHealth -= damageAmount;

        // Asegurar que no sea negativo
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log($"   Vida DESPUÉS del daño: {currentHealth}");
        Debug.Log($"   Diferencia: {vidaAnterior} → {currentHealth} (-{damageAmount})");

        // Verificación de que realmente cambió
        if (currentHealth == vidaAnterior)
        {
            Debug.LogError($"   ⚠️ ERROR: La vida NO cambió!");
            Debug.LogError($"   currentHealth == vidaAnterior == {currentHealth}");
        }

        // También mostrar en consola con un mensaje visible
        Debug.Log($"   ❤️  VIDA: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("   ☠️  ¡VIDA CERO! - GAME OVER");
            Die();
        }

        Debug.Log("========================================\n");
    }

    private string GetCallerInfo()
    {
        // Método para obtener quién llamó a TakeDamage
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        if (stackTrace.FrameCount > 1)
        {
            var frame = stackTrace.GetFrame(2); // Frame 2 es quien llamó a TakeDamage
            var method = frame.GetMethod();
            return $"{method.DeclaringType.Name}.{method.Name}";
        }
        return "Desconocido";
    }

    private void Die()
    {
        Debug.Log("💀 PLAYERHEALTH - Game Over!");
        // Aquí puedes añadir lógica de game over
    }

    void Update()
    {
        // Presiona P para verificar la vida actual
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"📊 VIDA ACTUAL (P presionado): {currentHealth}/{maxHealth}");
            Debug.Log($"   ¿PlayerHealth activo? {gameObject.activeSelf}");
            Debug.Log($"   ¿Enabled? {enabled}");
        }

        // Presiona L para forzar daño de prueba DESDE EL PLAYERHEALTH
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("🧪 DAÑO DE PRUEBA INTERNO (L presionado)");
            TakeDamage(5);
        }
    }

    // Método para ver la vida actual (público)
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Método para debug desde consola
    public void DebugHealth()
    {
        Debug.Log($"📈 DebugHealth(): {currentHealth}/{maxHealth}");
          //update del healtbar
    }
}