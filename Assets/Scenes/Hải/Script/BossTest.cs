using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTest : MonoBehaviour
{
    public GameObject miniEnemyPrefab;
    private float summonCooldown = 10f;
    private float summonTimer = 0f;
    public int numberToSummon = 3;
    public float summonRadius = 2f;
    public void SummonMinions()
    {
        for (int i = 0; i < numberToSummon; i++)
        {
            // Tạo vị trí ngẫu nhiên quanh boss
            Vector2 randomOffset = Random.insideUnitCircle * summonRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0);

            Instantiate(miniEnemyPrefab, spawnPosition, Quaternion.identity);
        }
    }
    void Update()
    {
        summonTimer += Time.deltaTime;
        if (summonTimer >= summonCooldown)
        {
            SummonMinions();
            summonTimer = 0f;
        }
    }
}
