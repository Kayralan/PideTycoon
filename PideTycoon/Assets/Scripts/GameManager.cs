using UnityEngine;
using System.Collections.Generic;
using System;

// --- YARDIMCI SINIFLAR ---
[System.Serializable]
public class UpgradeStat
{
    public string name;
    public int level = 1;
    public float baseCost = 100f;
    public float costMultiplier = 1.5f;
    public float powerMultiplier = 1.1f;

    public float GetCost() => baseCost * Mathf.Pow(costMultiplier, level - 1);
    public float GetPower() => Mathf.Pow(powerMultiplier, level - 1);
    public void LevelUp() => level++;
    public void Reset() => level = 1;
}

[System.Serializable]
public struct PideInfo
{
    public string isim;
    public float satisFiyati;
    public float acilmaUcreti;
}

// --- ANA CLASS ---
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyun Verileri")]
    public List<PideInfo> pideler;

    [Header("Yükseltmeler")]
    public UpgradeStat incomeUpgrade;
    public UpgradeStat speedUpgrade;

    [Header("Rebirth Ayarları")]
    public float rebirthCost = 1000000f;
    public float rebirthMultiplierAdd = 0.5f;

    [Header("Durum (Read Only)")]
    public float money = 0f;
    public int currentPideIndex = 0;
    public int rebirthCount = 0;
    public float globalRebirthMultiplier = 1f;

    // Otomatik Üretim
    private float currentCookTimer = 0f;
    private float baseCookTime = 3.0f;

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

    private void Update()
    {
        // Otomatik Pişirme Döngüsü
        float targetTime = baseCookTime / speedUpgrade.GetPower();
        currentCookTimer += Time.deltaTime;

        if (currentCookTimer >= targetTime)
        {
            currentCookTimer = 0f;
            SellPide();
        }
    }

    private void SellPide()
    {
        PideInfo currentPide = pideler[currentPideIndex];
        float income = currentPide.satisFiyati * incomeUpgrade.GetPower() * globalRebirthMultiplier;
        AddMoney(income);
    }

    public void AddMoney(float amount)
    {
        money += amount;
        UpdateUI();
    }

    // --- BUTON FONKSİYONLARI ---
    public void BuyIncomeUpgrade()
    {
        if (TrySpend(incomeUpgrade.GetCost()))
        {
            incomeUpgrade.LevelUp();
            UpdateUI();
        }
    }

    public void BuySpeedUpgrade()
    {
        if (TrySpend(speedUpgrade.GetCost()))
        {
            speedUpgrade.LevelUp();
            UpdateUI();
        }
    }

    public void BuyNextPide()
    {
        if (currentPideIndex + 1 >= pideler.Count) return;
        PideInfo nextPide = pideler[currentPideIndex + 1];

        if (TrySpend(nextPide.acilmaUcreti))
        {
            currentPideIndex++;
            UpdateUI();
        }
    }

    public void DoRebirth()
    {
        if (money >= rebirthCost)
        {
            rebirthCount++;
            globalRebirthMultiplier += rebirthMultiplierAdd;
            
            // Sıfırlama
            money = 0;
            currentPideIndex = 0;
            incomeUpgrade.Reset();
            speedUpgrade.Reset();
            currentCookTimer = 0f;
            
            UpdateUI();
        }
    }

    // --- YARDIMCI VE SAVE SİSTEMİ İÇİN METODLAR ---
    private bool TrySpend(float amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    private void UpdateUI() => OnGameStateChanged?.Invoke();

    public float GetMoneyPerSecond()
    {
        PideInfo currentPide = pideler[currentPideIndex];
        float onePideIncome = currentPide.satisFiyati * incomeUpgrade.GetPower() * globalRebirthMultiplier;
        float cookTime = baseCookTime / speedUpgrade.GetPower();
        return onePideIncome / cookTime;
    }

    public void LoadDataFromSave(float loadedMoney, int pideIndex, int rCount, float rMult)
    {
        money = loadedMoney;
        currentPideIndex = pideIndex;
        rebirthCount = rCount;
        globalRebirthMultiplier = rMult;
        UpdateUI();
    }

    public void SetUpgradeLevels(int incomeLvl, int speedLvl)
    {
        incomeUpgrade.level = incomeLvl;
        speedUpgrade.level = speedLvl;
        UpdateUI();
    }
}