using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    public GameObject menuPausa;
    public bool juegoPausado = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (juegoPausado)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Reanudar()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1;
        juegoPausado = false;
    }

    public void Pausar()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0;
        juegoPausado = true;
    }

    public void Menu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Salir()
    {
        Application.Quit();
    }
}
