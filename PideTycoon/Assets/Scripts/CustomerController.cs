using UnityEngine;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    private enum CustomerState { WalkingIn, WaitingForFood, Eating, WalkingOut }
    private CustomerState currentState;

    [Header("Ayarlar")]
    public float moveSpeed = 3f;
    public float eatingDuration = 2.0f;

    private Seat assignedSeat;
    private Transform targetPoint;
    private Transform exitPoint;
    private bool hasOrdered = false;
    
    // Görsel geri bildirim için renk değişimi
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        currentState = CustomerState.WalkingIn;
        ChangeColor(Color.white); // Başlangıç rengi
    }

    public void Setup(Seat seat, Transform exit)
    {
        assignedSeat = seat;
        targetPoint = seat.seatPoint; 
        exitPoint = exit;
    }

    private void Update()
    {
        switch (currentState)
        {
            case CustomerState.WalkingIn:
                MoveTo(targetPoint.position, CustomerState.WaitingForFood);
                break;

            case CustomerState.WaitingForFood:
                // Koltuğa vardık, rengi sarı yap (Bekliyor)
                ChangeColor(Color.yellow);

                // 1. Sipariş verilmediyse ver
                if (!hasOrdered)
                {
                    GameManager.Instance.StartCookingProcess();
                    hasOrdered = true;
                }

                // 2. Pide hazır mı?
                if (GameManager.Instance.isPideReady)
                {
                    GameManager.Instance.CustomerTookPide();
                    StartCoroutine(EatRoutine());
                }
                break;

            case CustomerState.Eating:
                // Coroutine çalışıyor, burada işlem yok
                break;

            case CustomerState.WalkingOut:
                MoveTo(exitPoint.position, CustomerState.WalkingOut);
                break;
        }
    }

    private void MoveTo(Vector3 destination, CustomerState nextStateIfArrived)
    {
        // Hedefe doğru hareket et
        transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);

        // Hedefe vardık mı?
        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            if (currentState == CustomerState.WalkingOut)
            {
                Destroy(gameObject); // Çıkışa vardı, yok et
            }
            else
            {
                currentState = nextStateIfArrived;
            }
        }
    }

    IEnumerator EatRoutine()
    {
        currentState = CustomerState.Eating;
        ChangeColor(Color.red); // Yerken kırmızı olsun
        
        // Yeme süresi kadar bekle
        yield return new WaitForSeconds(eatingDuration);
        
        // Ödeme Yap
        GameManager.Instance.SellPide();
        
        // Kalk ve git (Rengi tekrar beyaz yap)
        ChangeColor(Color.white);
        assignedSeat.isOccupied = false;
        targetPoint = exitPoint;
        currentState = CustomerState.WalkingOut;
    }

    // Yardımcı fonksiyon: Renk değiştirmek için
    void ChangeColor(Color color)
    {
        if (rend != null) rend.material.color = color;
    }
}