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

    // Offline Panel değişkenlerini sildik çünkü kullanmıyoruz.
    // Artık Inspector'da boş kalsalar bile hata vermezler.

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
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameStateChanged -= UpdateUI;
    }

    // ShowOfflineRewardPanel ve CollectOfflineMoney fonksiyonlarını sildik.

    private void UpdateUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Para yazısını güncelle
        moneyText.text = NumberFormatter.FormatNumber(gm.money);
        
        // Pide yazısını güncelle
        var currentPide = gm.pideler[gm.currentPideIndex];
        currentPideText.text = "Üretilen: " + currentPide.isim;
        
        // Rebirth yazısını güncelle
        rebirthInfoText.text = "Rebirth: " + gm.globalRebirthMultiplier.ToString("F1") + "x";

        // Butonları güncelle
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