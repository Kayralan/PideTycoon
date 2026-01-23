using UnityEngine;
using System.Collections.Generic;
using System;

// --- YARDIMCI SINIFLAR ---

[System.Serializable]
public class UpgradeStat
{
    public string name;             // Örn: "Hız", "Lezzet"
    public int level = 1;           // Başlangıç seviyesi
    public float baseCost = 100f;   // İlk ücret
    public float costMultiplier = 1.5f; // Fiyat artış katsayısı
    public float powerMultiplier = 1.1f; // Her seviyede vereceği güç (%10 vb.)

    // Şu anki seviyenin maliyetini hesapla
    public float GetCost()
    {
        return baseCost * Mathf.Pow(costMultiplier, level - 1);
    }

    // Şu anki gücü hesapla
    public float GetPower()
    {
        return Mathf.Pow(powerMultiplier, level - 1);
    }

    public void LevelUp() { level++; }
    public void Reset() { level = 1; }
}

[System.Serializable]
public struct PideInfo
{
    public string isim;         // Pide Adı
    public float satisFiyati;   // Temel kazanç
    public float acilmaUcreti;  // Tier atlama maliyeti
}

// --- ANA GAME MANAGER ---

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyun Verileri")]
    public List<PideInfo> pideler; // Inspector'dan doldurulacak

    [Header("Yükseltmeler")]
    public UpgradeStat incomeUpgrade; // 1. Buton: Gelir Artışı
    public UpgradeStat speedUpgrade;  // 3. Buton: Hız Artışı

    [Header("Rebirth Ayarları")]
    [SerializeField] private float rebirthCost = 1000000f; // 1 Milyon
    [SerializeField] private float rebirthMultiplierAdd = 0.5f; // Her rebirth +0.5x ekler

    [Header("Durum (Sadece Okuma)")]
    public float money = 0f;
    public int currentPideIndex = 0;
    public int rebirthCount = 0;
    public float globalRebirthMultiplier = 1f; // Tüm gelirleri çarpan ana değer

    // Usta (Otomatik Üretim) Değişkenleri
    private float currentCookTimer = 0f;
    private float baseCookTime = 3.0f; // Hiç upgrade yokken pişme süresi

    // UI Güncelleme Eventi
    public event Action OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    private void Start()
    {
        UpdateUI();
    }

    // --- OTOMATİK ÜRETİM DÖNGÜSÜ (USTA SİMÜLASYONU) ---
    private void Update()
    {
        // 1. Pişirme süresini hesapla (Hız upgrade'ine göre azalır)
        float targetTime = baseCookTime / speedUpgrade.GetPower();

        // 2. Zamanlayıcıyı ilerlet
        currentCookTimer += Time.deltaTime;

        // 3. Süre doldu mu?
        if (currentCookTimer >= targetTime)
        {
            currentCookTimer = 0f; // Sayacı sıfırla
            SellPide(); // Parayı kazan
        }
    }

    // --- PARA KAZANMA MANTIĞI ---
    private void SellPide()
    {
        PideInfo currentPide = pideler[currentPideIndex];

        // FORMÜL: PideFiyatı * GelirUpgrade * RebirthÇarpanı
        float income = currentPide.satisFiyati * incomeUpgrade.GetPower() * globalRebirthMultiplier;

        money += income;
        UpdateUI();
    }

    // --- BUTON FONKSİYONLARI ---

    // 1. Buton: Gelir Yükseltmesi
    public void BuyIncomeUpgrade()
    {
        float cost = incomeUpgrade.GetCost();
        if (TrySpend(cost))
        {
            incomeUpgrade.LevelUp();
            UpdateUI();
        }
    }

    // 2. Buton: Yeni Pide (Tier) Açma
    public void BuyNextPide()
    {
        // Son pidede miyiz?
        if (currentPideIndex + 1 >= pideler.Count) return;

        PideInfo nextPide = pideler[currentPideIndex + 1];
        
        if (TrySpend(nextPide.acilmaUcreti))
        {
            currentPideIndex++;
            UpdateUI();
        }
    }

    // 3. Buton: Hız Yükseltmesi
    public void BuySpeedUpgrade()
    {
        float cost = speedUpgrade.GetCost();
        if (TrySpend(cost))
        {
            speedUpgrade.LevelUp();
            UpdateUI();
        }
    }

    // Rebirth Butonu
    public void DoRebirth()
    {
        if (money >= rebirthCost)
        {
            rebirthCount++;
            
            // Rebirth Çarpanını Arttır (Örn: 1.0 -> 1.5 -> 2.0)
            globalRebirthMultiplier += rebirthMultiplierAdd;

            // OYUNU SIFIRLA (Soft Reset)
            ResetGameProgress();
            
            UpdateUI();
        }
    }

    // --- YARDIMCI METODLAR ---

    private bool TrySpend(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            return true;
        }
        return false;
    }

    private void ResetGameProgress()
    {
        money = 0;
        currentPideIndex = 0; // Lahmacuna geri dön
        
        // Upgrade seviyelerini sıfırla
        incomeUpgrade.Reset();
        speedUpgrade.Reset();

        // Sayaçları sıfırla
        currentCookTimer = 0f;
    }

    private void UpdateUI()
    {
        // UI Manager dinliyorsa tetikle
        OnGameStateChanged?.Invoke();
    }
    
    // UI scriptinden progress bar yapmak istersen bu oranı çekebilirsin (0 ile 1 arası döner)
    public float GetProductionProgress()
    {
        float targetTime = baseCookTime / speedUpgrade.GetPower();
        return Mathf.Clamp01(currentCookTimer / targetTime);
    }
}
