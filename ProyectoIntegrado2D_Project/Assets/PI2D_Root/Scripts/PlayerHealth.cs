using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    [SerializeField] private int currentHealth;

    private HealthBar healthBar;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float hitFlashDuration = 0.15f;

    private Coroutine flashRoutine;

    void Awake()
    {
        Debug.Log("🔵 PLAYERHEALTH - Awake llamado");
    }

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<HealthBar>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        Debug.Log("========================================");
        Debug.Log("🟢 PLAYERHEALTH - Start");
        Debug.Log($"   Vida inicializada: {currentHealth}/{maxHealth}");
        Debug.Log("========================================\n");
    }

    public void TakeDamage(int damageAmount)
    {
        Debug.Log("========================================");
        Debug.Log("🔴 PLAYERHEALTH - TakeDamage");

        if (damageAmount <= 0)
        {
            Debug.LogError("⚠️ damageAmount inválido");
            Debug.Log("========================================\n");
            return;
        }

        int vidaAnterior = currentHealth;
        currentHealth -= damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        Debug.Log($"   Vida: {vidaAnterior} → {currentHealth}");

        // 🔴 FLASH ROJO
        PlayHitFlash();

        if (currentHealth <= 0)
            Die();

        Debug.Log("========================================\n");
    }

    private void PlayHitFlash()
    {
        if (spriteRenderer == null)
            return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        Color original = spriteRenderer.color;
        spriteRenderer.color = hitColor;

        yield return new WaitForSeconds(hitFlashDuration);

        spriteRenderer.color = original;
    }

    private void Die()
    {
        Debug.Log("💀 PLAYERHEALTH - GAME OVER");
        // lógica de game over acá
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
