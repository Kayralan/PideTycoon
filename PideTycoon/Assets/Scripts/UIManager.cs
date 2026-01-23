using UnityEngine;
using TMPro; // TextMeshPro kütüphanesi şart!

public class UIManager : MonoBehaviour
{
    [Header("Ekran Bilgileri (Text Objelerini Sürükle)")]
    public TextMeshProUGUI moneyText;       // Sol üstteki para yazısı
    public TextMeshProUGUI currentPideText; // "Üretilen: Lahmacun" yazısı
    public TextMeshProUGUI rebirthInfoText; // "Rebirth: 1.5x" yazısı

    [Header("Buton İçindeki Yazılar (Text Objelerini Sürükle)")]
    public TextMeshProUGUI incomeButtonText;   // Gelir Butonunun içindeki Text
    public TextMeshProUGUI speedButtonText;    // Hız Butonunun içindeki Text
    public TextMeshProUGUI nextPideButtonText; // Yeni Pide Butonunun içindeki Text

    private void Start()
    {
        // GameManager hazır olduğunda bu fonksiyon çalışır
        if (GameManager.Instance != null)
        {
            // GameManager'daki değişimleri dinlemeye başla
            GameManager.Instance.OnGameStateChanged += UpdateUI;

            // Oyun açılır açılmaz ekranı bir kere güncelle
            UpdateUI();
        }
    }

    private void OnDestroy()
    {
        // Script yok edilirse (sahne değişimi vb.) dinlemeyi bırak
        // Bu yapılmazsa oyun hata verebilir.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
        }
    }

    // Ekranı Güncelleyen Ana Fonksiyon
    private void UpdateUI()
    {
        // Kod tekrarını önlemek için kısa referans
        var gm = GameManager.Instance;
        if (gm == null) return; // Hata koruması

        // --- 1. EKRAN BİLGİLERİ ---

        // Parayı "1.50aa" formatında yaz (Helper scriptini kullanır)
        moneyText.text = NumberFormatter.FormatNumber(gm.money);

        // Şu anki pide ismini yaz
        var currentPide = gm.pideler[gm.currentPideIndex];
        currentPideText.text = "Üretilen: " + currentPide.isim;

        // Rebirth Çarpanı
        rebirthInfoText.text = "Rebirth: " + gm.globalRebirthMultiplier.ToString("F1") + "x";


        // --- 2. BUTON YAZILARI ---

        // Gelir Butonu (Sadece Fiyat)
        float incomeCost = gm.incomeUpgrade.GetCost();
        incomeButtonText.text = NumberFormatter.FormatNumber(incomeCost);

        // Hız Butonu (Sadece Fiyat)
        float speedCost = gm.speedUpgrade.GetCost();
        speedButtonText.text = NumberFormatter.FormatNumber(speedCost);

        // Yeni Pide Butonu (İsim + Fiyat)
        int nextIndex = gm.currentPideIndex + 1;
        if (nextIndex < gm.pideler.Count)
        {
            var nextPide = gm.pideler[nextIndex];
            string fiyatStr = NumberFormatter.FormatNumber(nextPide.acilmaUcreti);
            
            // Alt alta İsim ve Fiyat yazar
            nextPideButtonText.text = $"{fiyatStr}";
        }
        else
        {
            // Pide listesi bittiyse
            nextPideButtonText.text = "MAX";
        }
    }
}