using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager instance;

    public bool snapTurn = true;
    public bool continuousTurn = false;

    public float masterVolume = 1f;
    public float sfxVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ApplyAllSettings();
    }

    public void ApplyAllSettings()
    {
        ApplyTurnSettings();
        ApplyAudioSettings();
    }

    void ApplyTurnSettings()
    {
        var snap = FindFirstObjectByType<ActionBasedSnapTurnProvider>();
        var continuous = FindFirstObjectByType<ActionBasedContinuousTurnProvider>();

        if (snap != null)
            snap.enabled = snapTurn;

        if (continuous != null)
            continuous.enabled = continuousTurn;
    }

    void ApplyAudioSettings()
    {
        AudioListener.volume = masterVolume;

        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(sfxVolume);
    }

    public void SetSnapTurn(bool value)
    {
        snapTurn = value;
        continuousTurn = !value;

        ApplyTurnSettings();
        SaveSettings();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        AudioListener.volume = value;
        SaveSettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;

        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(value);

        SaveSettings();
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("SnapTurn", snapTurn ? 1 : 0);
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        snapTurn = PlayerPrefs.GetInt("SnapTurn", 1) == 1;
        continuousTurn = !snapTurn;

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyAllSettings();

        // Update UI in new scene
        var ui = FindFirstObjectByType<SettingsUI>();
        if (ui != null)
            ui.RefreshUI();
    }
}