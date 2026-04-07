using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PotionEditMode
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
    public void RecoverHealth_Increases_Health_By_25Percent()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxHealth = 100;
        player.Health = 50;

        player.RecoverHealth();

        Assert.AreEqual(75, player.Health);
    }

    [Test]
    public void RecoverHealth_Decreases_Potion_Count()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.healthPotionCount = 3;


        player.RecoverHealth();

        Assert.Less(player.healthPotionCount, 3);
    }
    [Test]
    public void RecoverMana_Increases_Mana_By_25Percent()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.MaxMana = 100;
        player.Mana = 50;

        player.RecoverMana();

        Assert.AreEqual(75, player.Mana);
    }

    [Test]
    public void RecoverMana_Decreases_Potion_Count()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.manaPotionCount = 3;


        player.RecoverMana();

        Assert.Less(player.manaPotionCount, 3);
    }

    [Test]
    public void Recall_Decreases_Potion_Count()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.teleportPotionCount = 3;


        player.Recall();

        Assert.AreEqual(2, player.teleportPotionCount);
    }
}
