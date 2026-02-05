using UnityEngine;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Texts")]
    public TMP_Text scoreText;
    public TMP_Text rankText;
    public TMP_Text accuracyText;
    public TMP_Text maxComboText;

    public TMP_Text perfectsText;
    public TMP_Text goodsText;
    public TMP_Text missesText;

    void Start()
    {
        panel.SetActive(false);

        if (GameResults.lastResult == null)
        {
            Debug.LogError("No hay resultados para mostrar");
            return;
        }

        ShowResults(GameResults.lastResult);
    }

    public void ShowResults(LevelResult result)
    {
        panel.SetActive(true);

        scoreText.text = result.score.ToString();
        rankText.text = result.rank.ToString();
        accuracyText.text = $"{result.accuracy * 100f:0.00}%";
        maxComboText.text = result.maxCombo.ToString();

        perfectsText.text = result.perfects.ToString();
        goodsText.text = result.goods.ToString();
        missesText.text = result.misses.ToString();
    }
}
