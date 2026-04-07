using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PointEditMode
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
    public void Point_Update_When_Level_Up()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.level = 1;
        player.statPoints = 0;

        player.LevelUp();

        Assert.AreEqual(3, player.statPoints);
    }

    [Test]
    public void Point_Update_When_Level_Up_Multiple_Times()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.level = 1;
        player.statPoints = 0;

        for (int i = 0; i < 5; i++)
        {
            player.LevelUp();
        }

        Assert.AreEqual(15, player.statPoints);
    }

    [Test]
    public void Point_Update_When_Using_Points()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("str");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Point_Can_Not_Spend_When_No_Points_Left()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 0;

        player.UseStatPoint("str");

        Assert.AreEqual(0, player.statPoints);
    }

    [Test]
    public void Using_Point_To_Increase_Strength()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("str");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Using_Multiple_Points_To_Increase_Strength()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        for (int i = 0; i < 3; i++)
        {
            player.UseStatPoint("str");
        }

        Assert.AreEqual(0, player.statPoints);
    }

    [Test]
    public void Using_Point_To_Increase_Vitality()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("vit");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Using_Multiple_Points_To_Increase_Vitality()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        for (int i = 0; i < 3; i++)
        {
            player.UseStatPoint("vit");
        }

        Assert.AreEqual(0, player.statPoints);
    }

    [Test]
    public void Using_Point_To_Increase_Speed()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("spd");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Using_Multiple_Points_To_Increase_Speed()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        for (int i = 0; i < 3; i++)
        {
            player.UseStatPoint("spd");
        }

        Assert.AreEqual(0, player.statPoints);
    }

    [Test]
    public void Using_Point_To_Increase_Intelligence()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("int");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Using_Multiple_Points_To_Increase_Intelligence()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        for (int i = 0; i < 3; i++)
        {
            player.UseStatPoint("int");
        }

        Assert.AreEqual(0, player.statPoints);
    }

    [Test]
    public void Using_Point_To_Increase_Critical()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        player.UseStatPoint("crt");

        Assert.AreEqual(2, player.statPoints);
    }

    [Test]
    public void Using_Multiple_Points_To_Increase_Critical()
    {
        Player player = new GameObject().AddComponent<Player>();
        player.statPoints = 3;

        for (int i = 0; i < 3; i++)
        {
            player.UseStatPoint("crt");
        }

        Assert.AreEqual(0, player.statPoints);
    }
}
