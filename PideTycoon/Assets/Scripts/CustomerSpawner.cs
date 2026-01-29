using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab; 
    public List<Seat> seats; // Inspector'dan koltukları sürükle
    public Transform exitPoint;
    
    public float spawnRate = 2.0f;
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0f;
            TrySpawnCustomer();
        }
    }

    void TrySpawnCustomer()
    {
        Seat emptySeat = null;
        foreach (var seat in seats)
        {
            if (!seat.isOccupied)
            {
                emptySeat = seat;
                break;
            }
        }

        if (emptySeat != null)
        {
            emptySeat.isOccupied = true;
            GameObject newCustomer = Instantiate(customerPrefab, transform.position, Quaternion.identity);
            
            CustomerController controller = newCustomer.GetComponent<CustomerController>();
            controller.Setup(emptySeat, exitPoint);
        }
    }
}