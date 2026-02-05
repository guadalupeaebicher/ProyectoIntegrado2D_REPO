using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image _image;
    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        _image.fillAmount = GameManager.Instance.VidaActual / GameManager.Instance.VidaMaxima;
    }
}
