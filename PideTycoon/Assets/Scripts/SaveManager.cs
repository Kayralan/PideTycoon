using UnityEngine;
using System;
using System.IO;

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

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
        saveFilePath = Application.persistentDataPath + "/savefile.json";
    }

    private void Start()
    {
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
        // Debug.Log("Oyun Kaydedildi.");
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
        double secondsPassed = (DateTime.Now - oldTime).TotalSeconds;

        if (secondsPassed < 60) return; // En az 60 saniye geçmeli

        float moneyPerSecond = GameManager.Instance.GetMoneyPerSecond();
        float offlineEarnings = moneyPerSecond * (float)secondsPassed;

        if (offlineEarnings > 0 && UIManager.Instance != null)
        {
            UIManager.Instance.ShowOfflineRewardPanel(offlineEarnings, secondsPassed);
        }
    }
}