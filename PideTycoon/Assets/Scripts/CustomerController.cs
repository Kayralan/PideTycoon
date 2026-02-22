using UnityEngine;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    private enum CustomerState { WalkingToCorner, WalkingToSeat, WaitingForFood, Eating, WalkingToExitCorner, WalkingOut }
    private CustomerState currentState;

    [Header("Ayarlar")]
    public float moveSpeed = 3f;
    public float eatingDuration = 2.0f;
    
    [Header("Model Düzeltmesi")]
    [Tooltip("Eğer karakterler ters yürüyorsa burayı işaretle/kaldır")]
    public bool modelTersMi = false;

    // --- YENİ EKLENEN PİDE DEĞİŞKENLERİ ---
    [Header("Pide Görselleri (PNG'ler)")]
    public Sprite[] pideSprites; // 5 pidenin resmini buraya atacağız
    private GameObject masadakiPideObjesi; // Çıkan pideyi hafızada tutmak için

    private Seat assignedSeat;
    private Transform targetPoint;
    private Transform exitPoint;
    private Transform cornerPoint; 
    private Transform lookAtTarget; 
    private bool hasOrdered = false;
    
    public Animator anim;

    private void Start()
    {
        if (anim == null) anim = GetComponentInChildren<Animator>();
        currentState = CustomerState.WalkingToCorner; 
        SetAnimations(true, false);
    }

    public void Setup(Seat seat, Transform exit)
    {
        assignedSeat = seat;
        exitPoint = exit;
        cornerPoint = seat.cornerWaypoint; 
        lookAtTarget = seat.lookTarget; 
        
        if (cornerPoint != null) targetPoint = cornerPoint;
        else targetPoint = seat.seatPoint; 
    }

    private void Update()
    {
        switch (currentState)
        {
            case CustomerState.WalkingToCorner:
                MoveTo(targetPoint.position, CustomerState.WalkingToSeat);
                break;
            case CustomerState.WalkingToSeat:
                targetPoint = assignedSeat.seatPoint; 
                MoveTo(targetPoint.position, CustomerState.WaitingForFood);
                break;
            case CustomerState.WaitingForFood:
                SetAnimations(false, false);
                LookAtAssignedTarget(); 

                if (!hasOrdered)
                {
                    GameManager.Instance.StartCookingProcess();
                    hasOrdered = true;
                }
                
                if (GameManager.Instance.isPideReady)
                {
                    GameManager.Instance.CustomerTookPide();
                    StartCoroutine(EatRoutine());
                }
                break;
            case CustomerState.Eating:
                LookAtAssignedTarget(); 
                break;
            case CustomerState.WalkingToExitCorner:
                if (cornerPoint != null) targetPoint = cornerPoint;
                else targetPoint = exitPoint;
                MoveTo(targetPoint.position, CustomerState.WalkingOut);
                break;
            case CustomerState.WalkingOut:
                targetPoint = exitPoint; 
                MoveTo(targetPoint.position, CustomerState.WalkingOut);
                break;
        }
    }

    private void MoveTo(Vector3 destination, CustomerState nextStateIfArrived)
    {
        Vector3 fixedDestination = new Vector3(destination.x, destination.y, -1f);
        transform.position = Vector3.MoveTowards(transform.position, fixedDestination, moveSpeed * Time.deltaTime);

        Vector3 direction = (fixedDestination - transform.position).normalized;
        if (direction.sqrMagnitude > 0.001f)
        {
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.y);
            if (modelTersMi) lookDirection *= -1;
            
            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 15f);
            }
        }

        if (Vector3.Distance(transform.position, fixedDestination) < 0.05f)
        {
            if (currentState == CustomerState.WalkingOut) Destroy(gameObject); 
            else
            {
                currentState = nextStateIfArrived;
                if (currentState == CustomerState.WalkingToExitCorner || currentState == CustomerState.WalkingOut)
                    SetAnimations(true, false);
            }
        }
    }

    private void LookAtAssignedTarget()
    {
        if (lookAtTarget != null)
        {
            Vector3 direction = (lookAtTarget.position - transform.position).normalized;
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.y);
            if (modelTersMi) lookDirection *= -1;

            if (lookDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
        }
    }

    // --- PİDENİN ÇIKTIĞI VE YENDİĞİ YER ---
    IEnumerator EatRoutine()
    {
        currentState = CustomerState.Eating;
        SetAnimations(false, false);
        yield return new WaitForSeconds(0.1f);
        
        // Pide yeme animasyonunu başlat
        SetAnimations(false, true); 

        // --- RASTGELE PİDE SPAWN ETME ---
        if (pideSprites != null && pideSprites.Length > 0 && assignedSeat.tablePoint != null)
        {
            // Rastgele bir resim seç
            Sprite secilenPide = pideSprites[Random.Range(0, pideSprites.Length)];
            
            // Masanın üzerinde yeni bir obje yarat
            masadakiPideObjesi = new GameObject("PideMasa");
            
            // Pideyi masaya ve kameraya biraz daha yakın (Z = -1.5f) koyalım ki sandalyeye veya karaktere girmesin
            masadakiPideObjesi.transform.position = new Vector3(assignedSeat.tablePoint.position.x, assignedSeat.tablePoint.position.y, -1.5f);
            
            // Objeye resmi ekle
            SpriteRenderer sr = masadakiPideObjesi.AddComponent<SpriteRenderer>();
            sr.sprite = secilenPide;

            // PNG'ler büyükse diye bir küçültme ayarı (Eğer küçük görünürse buradaki 0.5f'leri 1f yapabilirsin)
            masadakiPideObjesi.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            // Masanın açısına göre pideyi yatırıp estetik durmasını istersen rotasyon da ekleyebiliriz ama 2D olduğu için genelde düz durması iyidir.
        }

        // Yeme süresi kadar bekle
        yield return new WaitForSeconds(eatingDuration);
        
        // --- YEMEK BİTTİ, PİDEYİ YOK ET ---
        if (masadakiPideObjesi != null)
        {
            Destroy(masadakiPideObjesi);
        }

        GameManager.Instance.SellPide();
        assignedSeat.isOccupied = false;
        
        currentState = CustomerState.WalkingToExitCorner;
        SetAnimations(true, false); 
    }

    private void SetAnimations(bool isWalking, bool isEating)
    {
        if (anim != null)
        {
            anim.SetBool("isWalking", isWalking);
            anim.SetBool("isEating", isEating);
        }
    }
}