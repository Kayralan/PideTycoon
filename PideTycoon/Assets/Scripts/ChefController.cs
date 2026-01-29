using UnityEngine;

public class ChefController : MonoBehaviour
{
    // Şimdilik animasyon yok, sadece kodun hata vermesini engellemek için
    // boş bıraktık. İleride buraya animasyon kodları gelecek.

    private void Update()
    {
        // Debug amaçlı konsola yazdırabilirsin istersen:
         if (GameManager.Instance.isCooking) Debug.Log("Usta Pişiriyor...");
    }
}