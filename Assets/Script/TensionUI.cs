using UnityEngine;
using UnityEngine.UI;

public class TensionUI : MonoBehaviour
{
    public Slider tensionSlider;
    public Image fillImage;

    public void UpdateTension(float tension)
    {
        tensionSlider.value = tension;

        if (tension < 0.5f)
        {
            fillImage.color = Color.Lerp(Color.white, Color.yellow, tension * 2f);
        }
        else
        {
            fillImage.color = Color.Lerp(Color.yellow, Color.red, (tension - 0.5f) * 2f);
        }
    }

    public void ResetBar()
    {
        tensionSlider.value = 0;
        fillImage.color = Color.white;
    }
}