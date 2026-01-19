using UnityEngine;
using UnityEngine.UI; // Buton ve Slider'lar için gerekli standart kütüphane
using TMPro;          // Yazılar için gerekli TextMeshPro kütüphanesi

public class PideStation : MonoBehaviour
{
    // [Header] kodu, Unity editöründe değişkenleri başlıklar altında düzenli gösterir.
    
    [Header("Station Settings (İstasyon Ayarları)")]
    public string pideName = "Kıymalı Pide"; // Pidenin adı
    public float salePrice = 50f;            // Pide başı kazanılacak para
    public float cookingTime = 3.0f;         // Pidenin pişme süresi (saniye)
    public float unlockCost = 100f;          // Bu ustayı işe alma maliyeti
    public float managerCost = 500f;         // Otomatik müdür alma maliyeti

    [Header("Status (Anlık Durum)")]
    public bool isUnlocked = false;          // Usta alındı mı? (Evet/Hayır)
    public bool hasManager = false;          // Müdür alındı mı? (Evet/Hayır)
    
    private float currentTimer = 0f;         // O anki pişirme süresini tutan sayaç
    private bool isCooking = false;          // Şu an pişiriyor mu?

    [Header("UI References (Arayüz Bağlantıları)")]
    // Oyun objelerini buraya sürükleyeceğiz
    public GameObject lockedPanel;           // Kilitliyken görünen gri panel
    public GameObject operationPanel;        // Açılınca görünen işlem paneli
    
    // TextMeshPro Yazıları
    public TMP_Text stationNameText;         // "Kıymalı Pide Ustası" yazan yer
    public TMP_Text unlockCostText;          // "Hire: 100 TL" yazan yer
    public TMP_Text managerCostText;         // "Manager: 500 TL" yazan yer

    // Butonlar ve Slider
    public Button unlockButton;              // Kilit açma butonu
    public Slider progressBar;               // İlerleme çubuğu (Loading bar)
    public Button buyManagerButton;          // Müdür alma butonu

    // Start: Oyun başladığında 1 kere çalışır
    private void Start()
    {
        // Panellerin durumunu (açık/kapalı) ayarla
        UpdateUI();

        // Yazıları scriptteki değerlere göre otomatik doldur
        // Bu sayede Unity editöründen tek tek elle yazı yazmak zorunda kalmayız
        if(stationNameText != null) stationNameText.text = pideName + " Chef"; // Örn: Kıymalı Pide Chef
        if(unlockCostText != null) unlockCostText.text = "Hire: " + unlockCost.ToString("F0");
        if(managerCostText != null) managerCostText.text = "Manager: " + managerCost.ToString("F0");
    }

    // Update: Her karede (frame) çalışır. (Saniyede 60+ kere)
    private void Update()
    {
        // Kural 1: Eğer istasyon kilitliyse (Usta yoksa) alt satırlara hiç geçme, fonksiyondan çık.
        if (!isUnlocked) return;

        // Durum A: Eğer şu an pişirme işlemi yapılıyorsa
        if (isCooking)
        {
            // Sayacı geçen süre kadar artır
            currentTimer += Time.deltaTime;
            
            // Slider'ı (barı) güncelle: (Geçen Süre / Toplam Süre) oranı 0 ile 1 arasında değer verir
            if (progressBar != null)
                progressBar.value = currentTimer / cookingTime;

            // Süre doldu mu kontrol et
            if (currentTimer >= cookingTime)
            {
                OnCookingComplete(); // Pişirme bitti fonksiyonunu çağır
            }
        }
        // Durum B: Pişirme yapılmıyor AMA Müdür var
        else if (hasManager) 
        {
            // Müdür varsa bekleme yapma, hemen yeni pişirme başlat
            StartCooking();
        }
    }

    // Pişirme işlemini başlatan tetikleyici
    // Bu fonksiyonu "Pide Pişir" butonuna bağlayacağız
    public void StartCooking()
    {
        // Eğer zaten pişirmiyorsa VE istasyon açıksa
        if (!isCooking && isUnlocked)
        {
            isCooking = true; // Pişirme modunu aç
            currentTimer = 0f; // Sayacı sıfırla
        }
    }

    // Pişirme süresi dolunca çalışan fonksiyon
    private void OnCookingComplete()
    {
        // GameManager'a git ve parayı kasaya ekle
        GameManager.Instance.AddMoney(salePrice);
        
        // Değerleri sıfırla
        currentTimer = 0f;
        if (progressBar != null) progressBar.value = 0;

        isCooking = false; // Pişirme modunu kapat (Müdür varsa Update fonksiyonu hemen geri açacak)
    }

    // --- SATIN ALMA İŞLEMLERİ ---

    // UI'daki "Hire Chef" (İşe Al) butonuna bağlanacak
    public void UnlockStation()
    {
        // Zaten açıksa işlem yapma
        if (isUnlocked) return;

        // GameManager'a sor: Param yetiyor mu? Ve yetiyorsa harca.
        if (GameManager.Instance.SpendMoney(unlockCost))
        {
            // Para yettiyse buraya girer
            isUnlocked = true; // Kilidi aç
            UpdateUI(); // Görünümü güncelle
        }
        else
        {
            // Para yetmediyse konsola hata bas (Oyuncu görmez, geliştirici görür)
            Debug.Log("Not enough money! (Para yetersiz)");
        }
    }

    // UI'daki "Buy Manager" butonuna bağlanacak
    public void BuyManager()
    {
        // Zaten müdür varsa işlem yapma
        if (hasManager) return;

        // Parayı harca
        if (GameManager.Instance.SpendMoney(managerCost))
        {
            hasManager = true; // Müdürü aktif et
            
            // Müdürü aldık, butonu artık tıklanamaz yap (Gri olsun)
            if(buyManagerButton != null) buyManagerButton.interactable = false; 
        }
    }

    // Görünümü (Kilitli mi / Açık mı) ayarlayan fonksiyon
    private void UpdateUI()
    {
        if (isUnlocked)
        {
            // İstasyon açıksa: Kilit panelini gizle, Çalışma panelini göster
            lockedPanel.SetActive(false);
            operationPanel.SetActive(true);
        }
        else
        {
            // İstasyon kilitliyse: Kilit panelini göster, Çalışma panelini gizle
            lockedPanel.SetActive(true);
            operationPanel.SetActive(false);
        }
    }
}