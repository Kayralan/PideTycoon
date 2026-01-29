using UnityEngine;
using System.Collections.Generic;
using System;

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

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Oyun Verileri")]
    public List<PideInfo> pideler;
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

    // --- PİŞİRME DURUMLARI ---
    [Header("Üretim Takibi")]
    public bool isCooking = false;
    public bool isPideReady = false;
    
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
        // Usta pişiriyorsa ve pide henüz hazır değilse süreyi işlet
        if (isCooking && !isPideReady)
        {
            float targetTime = baseCookTime / speedUpgrade.GetPower();
            currentCookTimer += Time.deltaTime;

            if (currentCookTimer >= targetTime)
            {
                // Pide Pişti!
                currentCookTimer = 0f;
                isCooking = false;
                isPideReady = true;
                Debug.Log("Pide Tezgaha Kondu!");
            }
        }
    }

    // --- MÜŞTERİ ETKİLEŞİMLERİ ---

    // 1. Müşteri masaya oturunca ustayı tetikler
    public void StartCookingProcess()
    {
        // Eğer usta boşsa ve tezgahta pide yoksa pişirmeye başla
        if (!isCooking && !isPideReady)
        {
            isCooking = true;
            currentCookTimer = 0f;
        }
    }

    // 2. Müşteri pideyi tezgahtan alınca
    public void CustomerTookPide()
    {
        if (isPideReady)
        {
            isPideReady = false;
            // Tezgah boşaldı.
        }
    }

    // 3. Müşteri yemeği bitirip ödeme yapınca
    public void SellPide()
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

    // --- BUTON İŞLEMLERİ ---
    public void BuyIncomeUpgrade()
    {
        if (TrySpend(incomeUpgrade.GetCost())) { incomeUpgrade.LevelUp(); UpdateUI(); }
    }

    public void BuySpeedUpgrade()
    {
        if (TrySpend(speedUpgrade.GetCost())) { speedUpgrade.LevelUp(); UpdateUI(); }
    }

    public void BuyNextPide()
    {
        if (currentPideIndex + 1 >= pideler.Count) return;
        PideInfo nextPide = pideler[currentPideIndex + 1];
        if (TrySpend(nextPide.acilmaUcreti)) { currentPideIndex++; UpdateUI(); }
    }

    public void DoRebirth()
    {
        if (money >= rebirthCost)
        {
            rebirthCount++;
            globalRebirthMultiplier += rebirthMultiplierAdd;
            money = 0;
            currentPideIndex = 0;
            incomeUpgrade.Reset();
            speedUpgrade.Reset();
            isCooking = false;
            isPideReady = false;
            UpdateUI();
        }
    }

    // --- YARDIMCI VE SAVE FONKSİYONLARI ---
    private bool TrySpend(float amount)
    {
        if (money >= amount) { money -= amount; UpdateUI(); return true; }
        return false;
    }

    private void UpdateUI() => OnGameStateChanged?.Invoke();

    // Offline kazanç için teorik hız (Müşterisiz)
    public float GetMoneyPerSecond()
    {
        PideInfo currentPide = pideler[currentPideIndex];
        float onePideIncome = currentPide.satisFiyati * incomeUpgrade.GetPower() * globalRebirthMultiplier;
        float cookTime = baseCookTime / speedUpgrade.GetPower();
        return onePideIncome / cookTime; // Saniyede kazanılan teorik para
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