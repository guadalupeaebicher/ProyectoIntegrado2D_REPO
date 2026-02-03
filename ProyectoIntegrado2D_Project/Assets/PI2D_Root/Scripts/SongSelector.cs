using UnityEngine;
using UnityEngine.SceneManagement;

public class SongSelector : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
