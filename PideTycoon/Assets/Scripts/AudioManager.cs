using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Singleton yapısı: Her yerden kolayca ulaşabilmemizi sağlar
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Arka plan müziği için
    public AudioSource sfxSource;   // Fırın, para, tıklama gibi efektler için

    private void Awake()
    {
        // Sahnede sadece bir tane AudioManager olduğundan emin oluyoruz
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Sahne değişse bile sesler kesilmesin
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Müziği susturma / açma
    public void SetMusicMute(bool isMuted)
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }
    }

    // Efektleri susturma / açma
    public void SetSFXMute(bool isMuted)
    {
        if (sfxSource != null)
        {
            sfxSource.mute = isMuted;
        }
    }

    // Oyun içinde herhangi bir yerden efekt çalmak için kullanacağımız pratik fonksiyon
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && !sfxSource.mute)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}