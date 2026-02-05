using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton simple
    public static GameManager Instance { get; private set; }

    // Referencias a tus sistemas
    private PlayerHealth playerHealth;
    private ScoreManager scoreManager;

    // Propiedades públicas - VISIBLES EN INSPECTOR
    [Header("Datos del Juego")]
    [SerializeField] private int vidaActual;
    [SerializeField] private int vidaMaxima;
    [SerializeField] private int puntos;

    // Propiedades públicas para acceder desde otros scripts
    public float VidaActual => vidaActual;
    public float VidaMaxima => vidaMaxima;
    public int Puntos => puntos;

    void Awake()
    {
        // Singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Buscar sistemas
        Invoke("BuscarSistemas", 0.1f);
    }

    void BuscarSistemas()
    {
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        scoreManager = FindAnyObjectByType<ScoreManager>();

        if (playerHealth != null)
            Debug.Log($"✅ PlayerHealth encontrado: {playerHealth.gameObject.name}");
        if (scoreManager != null)
            Debug.Log($"✅ ScoreManager encontrado: {scoreManager.gameObject.name}");
    }

    void Update()
    {
        // Actualizar datos cada frame
        ActualizarDatos();
    }

    void ActualizarDatos()
    {
        // Vida actual
        if (playerHealth != null)
        {
            vidaMaxima = playerHealth.maxHealth;

            // Intentar obtener vida actual
            var getHealthMethod = playerHealth.GetType().GetMethod("GetCurrentHealth");
            if (getHealthMethod != null)
            {
                vidaActual = (int)getHealthMethod.Invoke(playerHealth, null);
            }
            else
            {
                var healthField = playerHealth.GetType().GetField("currentHealth");
                if (healthField != null)
                    vidaActual = (int)healthField.GetValue(playerHealth);
            }
        }

        // Puntos
        if (scoreManager != null)
        {
            puntos = scoreManager.score;
        }
    }
}