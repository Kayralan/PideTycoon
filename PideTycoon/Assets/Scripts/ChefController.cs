using UnityEngine;

public class ChefController : MonoBehaviour
{
    // Her yerden kolayca ulaşmak için Singleton yapıyoruz
    public static ChefController Instance;

    [Header("Usta Görselleri")]
    public SpriteRenderer spriteRenderer;
    public Sprite idleSprite;    // Bize bakan, bekleyen usta
    public Sprite cookingSprite; // Arkası dönük, fırına pide atan usta

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Oyun başladığında usta boşta dursun
        SetIdle(); 
    }

    // Usta boşa geçtiğinde bu çalışacak
    public void SetIdle()
    {
        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    // Pide pişmeye başladığında bu çalışacak
    public void SetCooking()
    {
        if (spriteRenderer != null && cookingSprite != null)
        {
            spriteRenderer.sprite = cookingSprite;
        }
    }
}