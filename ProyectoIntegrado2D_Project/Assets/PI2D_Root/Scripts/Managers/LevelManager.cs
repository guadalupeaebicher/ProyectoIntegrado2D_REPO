using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioLevelManager : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;

    [Header("Siguiente Nivel")]
    [SerializeField] private string nextSceneName = "MenuPrincipal"; // Cambia esto por tu escena

    void Start()
    {
        // Si no asignaste el AudioSource en el Inspector, lo buscamos automáticamente
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Si sigue siendo null, buscamos en toda la escena
        if (audioSource == null)
        {
            audioSource = FindObjectOfType<AudioSource>();
            Debug.Log("AudioSource encontrado automáticamente en escena");
        }

        // Reproducir la canción si existe
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();

            // Calcular duración exacta del clip de audio
            float songDuration = audioSource.clip.length;
            Debug.Log($"Duración de la canción: {songDuration} segundos");

            // Programar cambio de escena cuando termine la canción
            Invoke("GoToNextScene", songDuration);
        }
        else
        {
            Debug.LogError("No se encontró AudioSource o clip de audio!");
            // Fallback: usar tiempo fijo de 199 segundos (3:19)
            Invoke("GoToNextScene", 199f);
        }
    }

    void GoToNextScene()
    {
        Debug.Log("Canción terminada - Cambiando de escena");

        // Detener la música si está reproduciéndose
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Cambiar a la siguiente escena
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Nombre de siguiente escena no asignado!");
            // Cargar la primera escena en Build Settings como fallback
            SceneManager.LoadScene(0);
        }
    }

    // Método público para terminar el nivel manualmente (si el jugador gana antes)
    public void FinishLevelEarly()
    {
        Debug.Log("Nivel terminado manualmente");
        CancelInvoke("GoToNextScene"); // Cancelar el temporizador automático
        GoToNextScene();
    }

    // Método para pausar/reanudar si necesitas
    public void PauseLevel()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
        Time.timeScale = 0f;
    }

    public void ResumeLevel()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
        Time.timeScale = 1f;
    }
}