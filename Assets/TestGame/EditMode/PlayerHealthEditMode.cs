using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHealthEditMode
{
    [TearDown]
    public void Cleanup()
    {
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    [Test]
    public void Player_Health_Should_Be_Initialized_Correctly()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxHealth = 100;
        player.Health = 100;

        Assert.AreEqual(100, player.Health);
    }

    [Test]
    public void Player_Heal_Correctly_Using_HealthPotion()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxHealth = 100;
        player.Health = 50;
        player.healthPotionCount = 1;

        player.RecoverHealth();

        Assert.AreEqual(75, player.Health);
    }

    [Test]
    public void Player_Heal_Can_Not_Exceed_MaxHealth()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxHealth = 100;
        player.Health = 90;
        player.healthPotionCount = 1;

        player.RecoverHealth();

        Assert.AreEqual(100, player.Health);
    }

    [Test]
    public void Player_Cannot_Heal_When_Health_Is_Zero()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxHealth = 100;
        player.healthPotionCount = 1;
        player.Health = 0;

        player.RecoverHealth();

        Assert.AreEqual(0, player.Health, "Health should remain 0 when player is dead");
        Assert.AreEqual(1, player.healthPotionCount, "Potion should NOT be consumed when dead");
    }
}