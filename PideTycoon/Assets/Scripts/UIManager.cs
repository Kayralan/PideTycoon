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
    public GameObject offlinePanel;         // Panelin kendisi
    public TextMeshProUGUI offlineTimeText; // "3 saat 15 dk yoktun" yazısı
    public TextMeshProUGUI offlineEarningsText; // Kazanılan para yazısı
    
    // Paneldeki parayı butona basana kadar burada tutacağız
    private float tempOfflineMoney = 0f;

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
        
        // Başlangıçta paneli kapalı tut
        if(offlinePanel != null) offlinePanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
    }

    // SaveManager burayı çağırır
    public void ShowOfflineRewardPanel(float moneyEarned, double secondsPassed)
    {
        if (offlinePanel == null) return;

        tempOfflineMoney = moneyEarned;
        
        System.TimeSpan ts = System.TimeSpan.FromSeconds(secondsPassed);
        string timeStr = string.Format("{0:D2}s {1:D2}dk", ts.Hours, ts.Minutes);

        // Eğer 4 saati geçtiyse kullanıcıya bilgi verelim
        if (secondsPassed > 14400) // 14400 sn = 4 saat
        {
            offlineTimeText.text = $"Süre: {timeStr} (Max 4 Saat)";
        }
        else
        {
            offlineTimeText.text = $"Süre: {timeStr}";
        }

        offlineEarningsText.text = NumberFormatter.FormatNumber(moneyEarned);
        offlinePanel.SetActive(true);
    }

    // Panelin üzerindeki "Topla" butonuna bu fonksiyonu bağlayacaksın
    public void CollectOfflineMoney()
    {
        if (tempOfflineMoney > 0)
        {
            GameManager.Instance.AddMoney(tempOfflineMoney);
            tempOfflineMoney = 0;
            offlinePanel.SetActive(false); // Paneli kapat
        }
    }

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
            nextPideButtonText.text = $"{nextPide.isim}\n{NumberFormatter.FormatNumber((float)nextPide.acilmaUcreti)}";
        }
        else
        {
            nextPideButtonText.text = "MAX";
        }
    }
}