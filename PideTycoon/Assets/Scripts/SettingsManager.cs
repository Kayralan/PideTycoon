using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel; 

    [Header("Buttons")]
    public Button openSettingsButton;
    public Button closeSettingsButton;

    [Header("UI References (Toggles)")]
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Toggle vibrationToggle;

    private void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Buton dinleyicileri
        if (openSettingsButton != null) openSettingsButton.onClick.AddListener(OpenSettingsPanel);
        if (closeSettingsButton != null) closeSettingsButton.onClick.AddListener(CloseSettingsPanel);

        LoadSettings();

        // Toggle dinleyicileri ve hata kontrolleri
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(ToggleMusic);
        else
            Debug.LogError("HATA: Music Toggle arayüzde (Inspector) atanmamış!");
            
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(ToggleSFX);
            
        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(ToggleVibration);
    }

    public void OpenSettingsPanel() { if (settingsPanel != null) settingsPanel.SetActive(true); }
    public void CloseSettingsPanel() { if (settingsPanel != null) settingsPanel.SetActive(false); }

    public void ToggleMusic(bool isEnabled)
    {
        Debug.Log("Müzik Toggle tetiklendi! Yeni durum: " + (isEnabled ? "Açık" : "Kapalı"));
        
        PlayerPrefs.SetInt("Music", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
        
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicMute(!isEnabled);
            Debug.Log("Sinyal AudioManager'a başarıyla iletildi.");
        }
        else
        {
            Debug.LogError("HATA: Sahnede AudioManager bulunamadı! Ses bu yüzden kapanmıyor.");
        }
    }

    public void ToggleSFX(bool isEnabled)
    {
        PlayerPrefs.SetInt("SFX", isEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXMute(!isEnabled);
        }
    }

    public void ToggleVibration(bool isEnabled)
    {
        PlayerPrefs.SetInt("Vibration", isEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        bool isMusicOn = PlayerPrefs.GetInt("Music", 1) == 1;
        bool isSfxOn = PlayerPrefs.GetInt("SFX", 1) == 1;
        bool isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;

        if (musicToggle != null) musicToggle.isOn = isMusicOn;
        if (sfxToggle != null) sfxToggle.isOn = isSfxOn;
        if (vibrationToggle != null) vibrationToggle.isOn = isVibrationOn;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicMute(!isMusicOn);
            AudioManager.Instance.SetSFXMute(!isSfxOn);
        }
    }
}