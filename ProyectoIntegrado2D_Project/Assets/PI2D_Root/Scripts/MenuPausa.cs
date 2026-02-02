using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;

    private bool juegoPausado = false;
    private bool initialized = false;

    // Unity 6 activa la UI antes de cualquier método normal.
    // Esto garantiza que la desactivamos UNA VEZ después del rebuild.
    void LateUpdate()
    {
        if (!initialized)
        {
            ForceHideMenu();
            initialized = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (juegoPausado)
                Reanudar();
            else
                Pausar();
        }
    }

    private void ForceHideMenu()
    {
        if (menuPausa == null) return;

        // Desactivar hijos
        foreach (Transform t in menuPausa.transform)
            t.gameObject.SetActive(false);

        // Desactivar menú
        menuPausa.SetActive(false);

        // Asegurar que el tiempo va normal
        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        if (menuPausa == null) return;

        foreach (Transform t in menuPausa.transform)
            t.gameObject.SetActive(true);

        menuPausa.SetActive(true);

        Time.timeScale = 0;
        juegoPausado = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reanudar()
    {
        ForceHideMenu(); // Reusar lógica

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Salir()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}