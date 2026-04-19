using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Toggle snapToggle;
    public Toggle continuousToggle;

    public Slider masterVolumeSlider;
    public Slider sfxVolumeSlider;

    void Start()
    {
        RefreshUI();
    }

    void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (SettingsManager.instance == null) return;

        var settings = SettingsManager.instance;

        // Turn
        snapToggle.isOn = settings.snapTurn;
        continuousToggle.isOn = settings.continuousTurn;

        // Volume
        masterVolumeSlider.value = settings.masterVolume;
        sfxVolumeSlider.value = settings.sfxVolume;
    }
}