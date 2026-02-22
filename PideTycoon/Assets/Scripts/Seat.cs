using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool isOccupied = false; // Dolu mu?
    public Transform seatPoint; // Müşterinin tam oturma noktası
    public Transform cornerWaypoint;
    public Transform lookTarget; // Müşterinin masadayken yüzünü döneceği obje
    public Transform tablePoint;
}