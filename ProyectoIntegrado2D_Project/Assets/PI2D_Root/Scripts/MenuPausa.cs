using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    public InputActionReference pauseAction;

    private bool juegoPausado;
    private Coroutine pausaCoroutine;

    void OnEnable()
    {
        pauseAction.action.performed += OnPause;
        pauseAction.action.Enable();
    }

    void OnDisable()
    {
        pauseAction.action.performed -= OnPause;
        pauseAction.action.Disable();
    }

    void Start()
    {
        menuPausa.SetActive(false);
        juegoPausado = false;
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }

    public void Pausar()
    {
        if (juegoPausado) return;

        menuPausa.SetActive(true);
        juegoPausado = true;

        // 1. Congelar tiempo
        Time.timeScale = 0;

        // 2. Pausar TODA la música y sonidos del juego
        // Esto afecta a TODOS los AudioSource sin excepción
        AudioListener.pause = true;

        // 3. Forzar pausa de cualquier sistema de ritmo
        // Detener todas las corrutinas en la escena
        if (pausaCoroutine != null)
            StopCoroutine(pausaCoroutine);

        pausaCoroutine = StartCoroutine(PausaCompleta());

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator PausaCompleta()
    {
        // Esto se ejecuta en tiempo real (ignora Time.timeScale)
        while (juegoPausado)
        {
            // Buscar y detener objetos que se mueven
            GameObject[] notas = GameObject.FindGameObjectsWithTag("Note");
            foreach (GameObject nota in notas)
            {
                // Congelar posición
                if (nota.GetComponent<Rigidbody>() != null)
                {
                    nota.GetComponent<Rigidbody>().isKinematic = true;
                }
                if (nota.GetComponent<Rigidbody2D>() != null)
                {
                    nota.GetComponent<Rigidbody2D>().simulated = false;
                }
            }

            yield return new WaitForSecondsRealtime(0.1f); // Revisar cada 0.1 segundos
        }
    }

    public void Reanudar()
    {
        if (!juegoPausado) return;

        menuPausa.SetActive(false);
        juegoPausado = false;

        // 1. Reanudar tiempo
        Time.timeScale = 1;

        // 2. Reanudar TODOS los sonidos
        AudioListener.pause = false;

        // 3. Detener la corrutina de pausa
        if (pausaCoroutine != null)
        {
            StopCoroutine(pausaCoroutine);
            pausaCoroutine = null;
        }

        // 4. Reactivar física de notas
        GameObject[] notas = GameObject.FindGameObjectsWithTag("Note");
        foreach (GameObject nota in notas)
        {
            if (nota.GetComponent<Rigidbody>() != null)
            {
                nota.GetComponent<Rigidbody>().isKinematic = false;
            }
            if (nota.GetComponent<Rigidbody2D>() != null)
            {
                nota.GetComponent<Rigidbody2D>().simulated = true;
            }
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1;
        AudioListener.pause = false; // Asegurar que audio esté activo
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Salir()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Application.Quit();
    }
}