using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    public Transform spawnPoint;

    // Hàm spawn object tại vị trí spawnPoint (nếu có) hoặc vị trí portal
    public void SpawnObject()
    {
        if (objectToSpawn != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(objectToSpawn, pos, Quaternion.identity);
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
