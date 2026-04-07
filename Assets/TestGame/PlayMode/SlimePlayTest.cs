using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SlimePlayTest
{
    GameManager gameManager;
    Player player;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        GameObject gameManagerObject = new GameObject("GameManager");
        gameManager = gameManagerObject.AddComponent<GameManager>();
        GameManager.Instance = gameManager;
        GameObject playerObject = new GameObject("Player");
        playerObject.tag = "Player"; // Required for collision detection
        player = playerObject.AddComponent<Player>();

        var playerCollider = playerObject.AddComponent<BoxCollider2D>();
        var playerRigidbody = playerObject.AddComponent<Rigidbody2D>();
        var playeranimator = playerObject.AddComponent<Animator>();
        playeranimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Player");
        playerRigidbody.gravityScale = 0;

        yield return null;
    }
}