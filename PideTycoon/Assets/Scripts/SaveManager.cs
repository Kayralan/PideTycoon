using UnityEngine;
using System;
using System.IO;
using System.Collections;

[System.Serializable]
public class SaveData
{
    public float money;
    public int currentPideIndex;
    public int incomeLevel;
    public int speedLevel;
    public int rebirthCount;
    public float rebirthMultiplier;
    public string lastQuitTime;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string saveFilePath;

    // AYAR: Maksimum kaç saniye kazanç biriksin? (4 Saat = 14400 sn)
    private const double MAX_OFFLINE_SECONDS = 14400; 

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        saveFilePath = Application.persistentDataPath + "/savefile.json";
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        LoadGame();
    }

    private void OnApplicationQuit() => SaveGame();
    private void OnApplicationPause(bool pause) { if (pause) SaveGame(); }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        var gm = GameManager.Instance;
        
        data.money = gm.money;
        data.currentPideIndex = gm.currentPideIndex;
        data.incomeLevel = gm.incomeUpgrade.level;
        data.speedLevel = gm.speedUpgrade.level;
        data.rebirthCount = gm.rebirthCount;
        data.rebirthMultiplier = gm.globalRebirthMultiplier;
        data.lastQuitTime = DateTime.Now.ToBinary().ToString();

        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            var gm = GameManager.Instance;
            
            gm.LoadDataFromSave(data.money, data.currentPideIndex, data.rebirthCount, data.rebirthMultiplier);
            gm.SetUpgradeLevels(data.incomeLevel, data.speedLevel);
            
            CalculateOfflineEarnings(data.lastQuitTime);
        }
    }

    private void CalculateOfflineEarnings(string lastTimeStr)
    {
        if (string.IsNullOrEmpty(lastTimeStr)) return;

        long temp = Convert.ToInt64(lastTimeStr);
        DateTime oldTime = DateTime.FromBinary(temp);
        
        // Gerçekte geçen süre
        double realSecondsPassed = (DateTime.Now - oldTime).TotalSeconds;

        if (realSecondsPassed < 60) return; // 1 dakikadan azsa hiç gösterme

        // Hesaplamada kullanılacak süre (Max 4 saat sınırı)
        double cappedSeconds = Math.Min(realSecondsPassed, MAX_OFFLINE_SECONDS);

        float moneyPerSecond = GameManager.Instance.GetMoneyPerSecond();
        float offlineEarnings = moneyPerSecond * (float)cappedSeconds;

        if (offlineEarnings > 0 && UIManager.Instance != null)
        {
            // UI'a hem kazanılan parayı, hem de gerçekte geçen süreyi gönderiyoruz
            UIManager.Instance.ShowOfflineRewardPanel(offlineEarnings, realSecondsPassed);
        }
    }
}