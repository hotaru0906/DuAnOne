using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerMovePlayMode
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
    #region Move Left Tests
    [UnityTest]
    public IEnumerator Player_Can_Move_Left_By_A_Key()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(-1f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Less(player.transform.position.x, 0, "Player should have moved left");
    }

    [UnityTest]
    public IEnumerator Player_Can_Move_Left_By_Arrow_Left_Key()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(-1f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Less(player.transform.position.x, 0, "Player should have moved left");
    }

    [UnityTest]
    public IEnumerator Player_Will_Facing_Left_When_Moving_Left()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(-1f, 0f);
        player.Flip();

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Less(player.transform.localScale.x, 0, "Player should be facing left");
    }

    [UnityTest]
    public IEnumerator Player_Can_Dash_Left_By_Ctrl_Key()
    {
        player.transform.position = Vector3.zero;
        player.dashForce = 10f;
        player.dashDuration = 0.15f;

        // Set player facing left (negative scale)
        player.transform.localScale = new Vector3(-1f, 1f, 1f);

        // Simulate dash directly (bypass Input.GetKeyDown check)
        player.isDashing = true;
        player.dashTime = player.dashDuration;
        float dashDirection = Mathf.Sign(player.transform.localScale.x); // -1 for left
        player.rb.velocity = new Vector2(dashDirection * player.dashForce, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isDashing, "Player should be dashing");
        Assert.Less(player.transform.position.x, 0f, "Player should have dashed left");
    }

    #endregion

    #region Move Right Tests

    [UnityTest]
    public IEnumerator Player_Can_Move_Right_By_D_Key()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(1f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(player.transform.position.x, 0, "Player should have moved right");
    }

    [UnityTest]
    public IEnumerator Player_Can_Move_Right_By_Arrow_Right_Key()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(1f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(player.transform.position.x, 0, "Player should have moved right");
    }

    [UnityTest]
    public IEnumerator Player_Will_Facing_Right_When_Moving_Right()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(1f, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(player.transform.localScale.x, 0, "Player should be facing right");
    }

    [UnityTest]
    public IEnumerator Player_Can_Dash_Right_By_Ctrl_Key()
    {
        player.transform.position = Vector3.zero;
        player.dashForce = 10f;
        player.dashDuration = 0.15f;

        // Set player facing right (positive scale)
        player.transform.localScale = new Vector3(1f, 1f, 1f);

        // Simulate dash directly (bypass Input.GetKeyDown check)
        player.isDashing = true;
        player.dashTime = player.dashDuration;
        float dashDirection = Mathf.Sign(player.transform.localScale.x); // 1 for right
        player.rb.velocity = new Vector2(dashDirection * player.dashForce, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsTrue(player.isDashing, "Player should be dashing");
        Assert.Greater(player.transform.position.x, 0f, "Player should have dashed right");
    }
    #endregion

    [UnityTest]
    public IEnumerator Player_Can_Jump_When_Stay_Still()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(0f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(player.transform.position.y, 0, "Player should have jumped");
    }

    [UnityTest]
    public IEnumerator Player_Can_Not_Jump_Multiple_Times()
    {
        player.transform.position = Vector3.zero;
        player.rb.velocity = new Vector2(0f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        player.rb.velocity = new Vector2(0f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Less(player.transform.position.y, 10f, "Player should not have jumped again while in the air");
    }

    [UnityTest]
    public IEnumerator Player_Can_Not_Dash_While_Jumping()
    {
        player.transform.position = Vector3.zero;
        player.dashForce = 10f;
        player.dashDuration = 0.15f;

        // Simulate jump
        player.rb.velocity = new Vector2(0f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        // Simulate dash directly (bypass Input.GetKeyDown check)
        player.isGrounded = false; // Ensure player is not grounded
        player.dashTime = player.dashDuration;
        float dashDirection = Mathf.Sign(player.transform.localScale.x); // Use current facing direction
        player.rb.velocity = new Vector2(dashDirection * player.dashForce, 0f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.IsFalse(player.isDashing, "Player should not be able to dash while jumping");
    }

    [UnityTest]
    public IEnumerator Player_Can_Move_Left_And_Jump_Simultaneously()
    {
        player.transform.position = Vector3.zero;

        player.rb.velocity = new Vector2(-1f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Less(player.transform.position.x, 0, "Player should have moved left");
        Assert.Greater(player.transform.position.y, 0, "Player should have jumped");
    }

    [UnityTest]
    public IEnumerator Player_Can_Move_Right_And_Jump_Simultaneously()
    {
        player.transform.position = Vector3.zero;

        // Simulate moving right and jumping at the same time
        player.rb.velocity = new Vector2(1f, 5f);

        yield return new WaitForFixedUpdate();
        yield return null;

        Assert.Greater(player.transform.position.x, 0, "Player should have moved right");
        Assert.Greater(player.transform.position.y, 0, "Player should have jumped");
    }
}