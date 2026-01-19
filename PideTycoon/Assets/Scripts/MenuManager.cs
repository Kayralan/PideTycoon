using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject shopPanel; // Senin Scroll View'ın içinde olduğu ana kutu
    public GameObject openShopButton; // Dükkanı açan buton (Dükkan açılınca gizlensin mi?)

    // Oyun başlarken dükkan kapalı olsun
    private void Start()
    {
        CloseShop();
    }

    public void OpenShop()
    {
        shopPanel.SetActive(true);  // Paneli aç
        openShopButton.SetActive(false); // Açma butonunu gizle (İsteğe bağlı)
    }

    public void CloseShop()
    {
        shopPanel.SetActive(false); // Paneli kapat
        openShopButton.SetActive(true); // Açma butonunu geri getir
    }
}
