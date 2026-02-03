using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    public InputActionReference pauseAction;

    private bool juegoPausado;
    private bool initialized;

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
        ForceHideMenu(); // Unity 6 friendly
        initialized = true;
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        if (juegoPausado)
            Reanudar();
        else
            Pausar();
    }

    private void ForceHideMenu()
    {
        if (!menuPausa) return;

        menuPausa.SetActive(false);

        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        if (!menuPausa) return;

        menuPausa.SetActive(true);

        Time.timeScale = 0;
        juegoPausado = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Reanudar()
    {
        ForceHideMenu();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuPrincipal");
    }

    public void Salir()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
}
