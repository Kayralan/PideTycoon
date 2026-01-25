using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Ana Ekran")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI currentPideText;
    public TextMeshProUGUI rebirthInfoText;

    [Header("Butonlar")]
    public TextMeshProUGUI incomeButtonText;
    public TextMeshProUGUI speedButtonText;
    public TextMeshProUGUI nextPideButtonText;

    [Header("Offline Kazanç Paneli")]
    public GameObject offlinePanel;
    public TextMeshProUGUI offlineTimeText;
    public TextMeshProUGUI offlineEarningsText;
    
    private float pendingOfflineMoney = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateUI;
            UpdateUI();
        }
        if(offlinePanel != null) offlinePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
    }

    // --- PANEL YÖNETİMİ ---
    public void ShowOfflineRewardPanel(float moneyEarned, double secondsPassed)
    {
        pendingOfflineMoney = moneyEarned;
        System.TimeSpan ts = System.TimeSpan.FromSeconds(secondsPassed);
        string timeStr = string.Format("{0:D2}s {1:D2}dk {2:D2}sn", ts.Hours, ts.Minutes, ts.Seconds);

        offlineTimeText.text = $"Dükkan {timeStr} çalıştı!";
        offlineEarningsText.text = NumberFormatter.FormatNumber(moneyEarned);
        
        offlinePanel.SetActive(true);
    }

    public void CollectOfflineMoney()
    {
        if (pendingOfflineMoney > 0)
        {
            GameManager.Instance.AddMoney(pendingOfflineMoney);
            pendingOfflineMoney = 0;
            offlinePanel.SetActive(false);
        }
    }

    // --- GÜNCELLEME ---
    private void UpdateUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        moneyText.text = NumberFormatter.FormatNumber(gm.money);
        
        var currentPide = gm.pideler[gm.currentPideIndex];
        currentPideText.text = "Üretilen: " + currentPide.isim;
        
        rebirthInfoText.text = "Rebirth: " + gm.globalRebirthMultiplier.ToString("F1") + "x";

        incomeButtonText.text = NumberFormatter.FormatNumber(gm.incomeUpgrade.GetCost());
        speedButtonText.text = NumberFormatter.FormatNumber(gm.speedUpgrade.GetCost());

        int nextIndex = gm.currentPideIndex + 1;
        if (nextIndex < gm.pideler.Count)
        {
            var nextPide = gm.pideler[nextIndex];
            nextPideButtonText.text = $"{nextPide.isim}\n{NumberFormatter.FormatNumber(nextPide.acilmaUcreti)}";
        }
        else
        {
            nextPideButtonText.text = "MAX";
        }
    }
}