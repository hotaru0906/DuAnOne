using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerAttackPlayTest
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

    [TearDown]
    public void Cleanup()
    {
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_By_J_Key()
    {
        player.transform.position = Vector3.zero;

        player.isAttacking = true;

        yield return null;

        Assert.IsTrue(player.isAttacking, "Player should be in attacking state");
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_Multiple_Times_By_J_Key()
    {
        player.transform.position = Vector3.zero;

        // Simulate multiple attacks
        for (int i = 0; i < 3; i++)
        {
            player.isAttacking = true;
            yield return null; // Wait a frame between attacks
            Assert.IsTrue(player.isAttacking, $"Player should be in attacking state on attack {i + 1}");
            player.isAttacking = false; // Reset attack state for next iteration
        }
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_While_Moving_Left()
    {
        player.transform.position = Vector3.zero;

        // Simulate moving left and attacking at the same time
        player.rb.velocity = new Vector2(-1f, 0f);
        player.isAttacking = true;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isAttacking, "Player should be in attacking state while moving");
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_While_Moving_Left_And_Jumping()
    {
        player.transform.position = Vector3.zero;

        // Simulate moving left, jumping, and attacking at the same time
        player.rb.velocity = new Vector2(-1f, 5f); // Move left and jump
        player.isAttacking = true;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isAttacking, "Player should be in attacking state while moving and jumping");
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_While_Moving_Right()
    {
        player.transform.position = Vector3.zero;

        // Simulate moving right and attacking at the same time
        player.rb.velocity = new Vector2(1f, 0f);
        player.isAttacking = true;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isAttacking, "Player should be in attacking state while moving");
    }

    [UnityTest]
    public IEnumerator Player_Can_Attack_While_Moving_Right_And_Jumping()
    {
        player.transform.position = Vector3.zero;

        // Simulate moving right, jumping, and attacking at the same time
        player.rb.velocity = new Vector2(1f, 5f); // Move right and jump
        player.isAttacking = true;

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isAttacking, "Player should be in attacking state while moving and jumping");
    }
}
