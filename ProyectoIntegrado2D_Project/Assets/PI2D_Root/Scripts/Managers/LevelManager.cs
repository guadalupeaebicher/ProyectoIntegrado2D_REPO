using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioLevelManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string nextSceneName = "MenuPrincipal";

    void Start()
    {
        if (audioSource == null)
            audioSource = FindObjectOfType<AudioSource>();

        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
            Invoke(nameof(FinishLevel), audioSource.clip.length);
        }
    }

    void FinishLevel()
    {
        LevelResult result = ScoreManager.instance.GetFinalResult();

        Debug.Log("=== RESULTADO FINAL ===");
        Debug.Log($"Score: {result.score}");
        Debug.Log($"Max Combo: {result.maxCombo}");
        Debug.Log($"Perfects: {result.perfects}");
        Debug.Log($"Goods: {result.goods}");
        Debug.Log($"Misses: {result.misses}");
        Debug.Log($"Wrongs: {result.wrongs}");
        Debug.Log($"Accuracy: {(result.accuracy * 100f):F1}%");
        Debug.Log($"Rank: {result.rank}");

        SceneManager.LoadScene(nextSceneName);
    }
}
