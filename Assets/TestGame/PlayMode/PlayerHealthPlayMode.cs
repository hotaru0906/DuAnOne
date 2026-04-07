using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthPlayMode
{
    GameManager gameManager;
    Player player;
    Enemy enemy;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();

        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player";
        player = playerObject.AddComponent<Player>();

        var playerCollider = playerObject.AddComponent<BoxCollider2D>();
        var playerRigidbody = playerObject.AddComponent<Rigidbody2D>();
        var playeranimator = playerObject.AddComponent<Animator>();
        playeranimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Player");
        playerRigidbody.gravityScale = 0;

        GameObject enemyObject = new GameObject("Enemy");
        enemyObject.tag = "Enemy";

        var enemyCollider = enemyObject.AddComponent<BoxCollider2D>();
        var enemyRigidbody = enemyObject.AddComponent<Rigidbody2D>();
        enemyRigidbody.gravityScale = 0;

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
    public IEnumerator Player_Can_Not_Change_Health_When_Player_Move()
    {
        player.MaxHealth = 100;
        player.Health = 100;

        player.transform.position = Vector3.zero;
        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.AreEqual(100, player.Health);
    }
    [UnityTest]
    public IEnumerator Player_Can_Not_Change_Health_When_Player_Stays_Still()
    {
        player.MaxHealth = 100;
        player.Health = 100;

        player.transform.position = Vector3.zero;
        player.rb.velocity = Vector2.zero;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.AreEqual(100, player.Health);
    }
    [UnityTest]
    public IEnumerator Player_Health_Equals_0_When_Player_Dies()
    {
        player.MaxHealth = 100;
        player.Health = 100;

        player.Die();

        yield return null;

        Assert.AreEqual(0, player.Health);
    }
    [UnityTest]
    public IEnumerator TakeDamage_SetsHitState_And_ReducesHealth()
    {
        player.MaxHealth = 100;
        player.Health = 100;

        player.TakeDamage(20);

        yield return null;

        Assert.AreEqual(80, player.Health);
        Assert.IsTrue(player.isHit, "Player should enter hit state");
        Assert.AreEqual(Vector2.zero, player.rb.velocity, "Player should stop moving");
    }

    [UnityTest]
    public IEnumerator TakeDamage_Decreases_Health_1_Time_When_A_Lot_Enemies_Attack()
    {
        player.MaxHealth = 100;
        player.Health = 100;

        for (int i = 0; i < 10; i++)
        {
            player.TakeDamage(10);
            player.isInvincible = false; // Reset invincibility to simulate multiple hits
        }

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.AreEqual(90, player.Health);
    }

    [UnityTest]
    public IEnumerator Player_Can_Not_Move_When_Health_Is_Zero()
    {
        player.MaxHealth = 100;
        player.Health = 0;

        player.transform.position = Vector3.zero;
        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.AreEqual(Vector2.zero, player.rb.velocity, "Player should not move when health is zero");
    }
}