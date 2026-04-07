using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CojnPlayTest
{
    GameManager gameManager;
    Player player;
    Coin coin;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();

        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player"; // Required for collision detection
        player = playerObject.AddComponent<Player>();

        var playerCollider = playerObject.AddComponent<BoxCollider2D>();
        var playerRigidbody = playerObject.AddComponent<Rigidbody2D>();
        var playeranimator = playerObject.AddComponent<Animator>();
        playeranimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Player");
        playerRigidbody.gravityScale = 0;

        GameObject coinObject = new GameObject("Coin");
        coin = coinObject.AddComponent<Coin>();
        var coinCollider = coinObject.AddComponent<BoxCollider2D>();
        coinCollider.isTrigger = true;

        yield return null;
    }


    [TearDown]
    public void Cleanup()
    {
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            GameObject.DestroyImmediate(obj);
        }
    }
    [UnityTest]
    public IEnumerator Coin_Collected_When_Player_Collides()
    {
        player.transform.position = Vector3.zero;
        coin.transform.position = new Vector3(0.1f, 0, 0);

        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(coin == null, "Coin should be destroyed after collection");
    }

    [UnityTest]
    public IEnumerator Coin_Will_Not_Be_Collected_When_Player_Does_Not_Collide()
    {
        player.transform.position = Vector3.zero;
        coin.transform.position = new Vector3(1, 0, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsNotNull(coin, "Coin should not be destroyed when not collected");

    }

    [UnityTest]
    public IEnumerator Coin_Should_Be_Destroyed_After_Collected_And_Not_Reappear()
    {
        player.transform.position = Vector3.zero;
        coin.transform.position = new Vector3(0.5f, 0, 0);

        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(coin == null, "Coin should be destroyed after first collection");

        player.transform.position = Vector3.zero;
        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(coin == null, "Coin should NOT exist anymore");
    }

    [UnityTest]
    public IEnumerator Coin_Drops_When_Enemy_Is_Destroyed()
    {
        GameObject enemyObject = new GameObject("Enemy");
        Enemy enemy = enemyObject.AddComponent<Enemy>();

        GameObject coinPrefab = new GameObject("CoinPrefab");
        coinPrefab.AddComponent<Coin>();

        enemy.coinPrefab = coinPrefab;
        enemy.amount = 5;

        Vector3 enemyPosition = enemy.transform.position;

        enemy.DestroyEnemy();

        yield return null;

        Coin spawnedCoin = GameObject.FindObjectOfType<Coin>();

        Assert.IsNotNull(spawnedCoin, "Coin should be spawned when enemy is destroyed");

        Assert.AreEqual(enemyPosition, spawnedCoin.transform.position, "Coin should spawn at enemy position");

        Assert.AreEqual(5, spawnedCoin.value, "Coin value should match enemy amount");

        Assert.IsTrue(enemy == null || enemy.Equals(null), "Enemy should be destroyed");
    }
}
