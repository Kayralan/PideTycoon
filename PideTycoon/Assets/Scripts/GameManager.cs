using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi (Gelişmiş yazı sistemi için)

public class GameManager : MonoBehaviour
{
    // Singleton Yapısı: Bu scriptten sahnede sadece 1 tane olmasını ve 
    // diğer scriptlerin (PideStation gibi) buna kolayca ulaşmasını sağlar.
    public static GameManager Instance;

    [Header("Ekonomi Ayarları")]
    public float totalMoney = 0f; // Oyuncunun cebindeki toplam para
    
    [Header("UI Referansları")]
    public TMP_Text totalMoneyText; // Ekranda parayı gösteren yazı (TMP)

    // Oyun başlarken (Start'tan bile önce) çalışan özel Unity fonksiyonu
    private void Awake()
    {
        // Eğer sahnede henüz bir Instance yoksa, bu scripti Instance yap.
        if (Instance == null) 
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Oyun başladığında ekrandaki parayı bir kez güncelle
        UpdateUI();
    }

    // Parayı artıran fonksiyon. Pide piştiğinde bu çağrılır.
    // amount: Ne kadar ekleneceği
    public void AddMoney(float amount)
    {
        totalMoney += amount; // Mevcut paranın üzerine ekle
        UpdateUI(); // Ekrandaki yazıyı güncelle
    }

    // Harcama yapma fonksiyonu. Usta veya Müdür alırken çağrılır.
    // amount: Harcanacak miktar
    // Geriye 'bool' (doğru/yanlış) döndürür. Para yettiyse true, yetmediyse false döner.
    public bool SpendMoney(float amount)
    {
        // Eğer cepteki para, harcanacak miktardan büyük veya eşitse
        if (totalMoney >= amount)
        {
            totalMoney -= amount; // Parayı düş
            UpdateUI(); // Ekranı güncelle
            return true; // İşlem başarılı (Satın alabilirsin)
        }
        
        // Buraya geldiyse para yetmemiş demektir
        return false; // İşlem başarısız
    }

    // Ekrandaki para yazısını güncelleyen yardımcı fonksiyon
    void UpdateUI()
    {
        // Eğer text kutusu boş değilse (bağlantı kopmamışsa)
        if (totalMoneyText != null)
        {
            // "F0" kodu, ondalık kısım olmasın demektir (Örn: 100 TL, 100.5 değil)
            totalMoneyText.text = totalMoney.ToString("F0") + " TL";
        }
    }
}