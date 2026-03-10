using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawn : MonoBehaviour
    // Hàm spawn object tại vị trí chỉ định (dùng cho Boss4 summon)
    
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public Transform spawnPoint;

    public void SpawnObjectAtPosition(Vector3 position)
    {
        if (objectToSpawn != null)
        {
            Instantiate(objectToSpawn, position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("PortalSpawn: objectToSpawn is not assigned!");
        }
    }

    // Hàm destroy portal
    public void DestroyPortal()
    {
        Destroy(gameObject);
    }
}
