using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ChestPlayTest
{
    GameManager gameManager;
    Player player;
    Chest chest;

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

        GameObject chestObject = new GameObject("Chest");
        chest = chestObject.AddComponent<Chest>();
        var chestCollider = chestObject.AddComponent<BoxCollider2D>();
        chestCollider.isTrigger = true;

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
    public IEnumerator Chest_Collected_When_Player_Collides()
    {
        player.transform.position = Vector3.zero;
        chest.transform.position = new Vector3(0.1f, 0, 0);

        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(chest == null, "Chest should be destroyed after collection");
    }

    [UnityTest]
    public IEnumerator Chest_Will_Not_Be_Collected_When_Player_Does_Not_Collide()
    {
        player.transform.position = Vector3.zero;
        chest.transform.position = new Vector3(1, 0, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsNotNull(chest, "Chest should not be destroyed when not collected");

    }

    [UnityTest]
    public IEnumerator Chest_Will_Not_Be_Collected_When_Player_Has_Collected_Before()
    {
        player.transform.position = Vector3.zero;
        chest.transform.position = new Vector3(0.5f, 0, 0);

        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(chest == null, "Chest should be destroyed after first collection");

        player.transform.position = Vector3.zero;
        player.rb.velocity = new Vector2(1, 0);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(chest == null, "Chest should NOT exist anymore");
    }

}
