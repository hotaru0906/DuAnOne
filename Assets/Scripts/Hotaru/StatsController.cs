using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro elements

public class StatsController : MonoBehaviour
{
    public Player player; // Reference to the Player script
    public int points = 3; // Total points available to allocate

    public int str = 0; // Strength
    public int vit = 0; // Vitality
    public int spd = 0; // Speed
    public int intStat = 0; // Intelligence
    public int crt = 0; // Critical Rate

    public TMP_Text pointsText; // UI Text to display remaining points
    public TMP_Text strText; // UI Text to display Strength value
    public TMP_Text vitText; // UI Text to display Vitality value
    public TMP_Text spdText; // UI Text to display Speed value
    public TMP_Text intText; // UI Text to display Intelligence value
    public TMP_Text crtText; // UI Text to display Critical Rate value
    public TMP_Text healthText; // UI Text to display Health value
    public TMP_Text manaText; // UI Text to display Mana value
    public TMP_Text expText; // UI Text to display Experience value
    public TMP_Text goldText; // UI Text to display Gold value

    void Start()
    {
        if (player != null)
        {
            // Initialize stats from Player
            str = player.str;
            vit = player.vit;
            spd = player.spd;
            intStat = player.intStat;
            crt = player.crt;
        }
        UpdateUI(); // Initialize UI with current values
    }

    void Update()
    {
        if (player != null)
        {
            points = player.statPoints; // Synchronize points with Player's statPoints

            // Update health, mana, exp, and gold values
            if (healthText != null)
                healthText.text = $"{player.Health}/{player.MaxHealth}";

            if (manaText != null)
                manaText.text = $"{player.Mana}/{player.MaxMana}";

            if (expText != null)
                expText.text = $"{player.currentExp}/{player.expToNextLevel}";

            if (goldText != null)
                goldText.text = $"{player.gold}";

            UpdateUI(); // Update the UI to reflect the current points
        }
    }

    public void AddPointToStat(string stat)
    {
        if (points <= 0)
        {
            Debug.Log("No points left to allocate.");
            return;
        }

        switch (stat)
        {
            case "str":
                str++;
                if (player != null) player.str = str;
                break;
            case "vit":
                vit++;
                if (player != null) player.vit = vit;
                break;
            case "spd":
                spd++;
                if (player != null) player.spd = spd;
                break;
            case "int":
                intStat++;
                if (player != null) player.intStat = intStat;
                break;
            case "crt":
                crt++;
                if (player != null) player.crt = crt;
                break;
            default:
                Debug.LogError("Invalid stat name: " + stat);
                return;
        }

        points--; // trừ local
        if (player != null) player.statPoints = points; // ← CẬP NHẬT về Player

        UpdateUI(); // Update the UI to reflect changes
    }


    void UpdateUI()
    {
        if (pointsText != null)
            pointsText.text = "Points: " + points;

        if (strText != null)
            strText.text = "STR: " + str;

        if (vitText != null)
            vitText.text = "VIT: " + vit;

        if (spdText != null)
            spdText.text = "SPD: " + spd;

        if (intText != null)
            intText.text = "INT: " + intStat;

        if (crtText != null)
            crtText.text = "CRT: " + crt;
    }

    public void SyncStatsFromPlayer()
    {
        if (player != null)
        {
            str = player.str;
            vit = player.vit;
            spd = player.spd;
            intStat = player.intStat;
            crt = player.crt;
            points = player.statPoints;
            UpdateUI(); // Update the UI to reflect the synced stats
        }
    }

    void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            // Sync stats from GameManager
            var stats = GameManager.Instance.GetPlayerStats();
            str = stats.str;
            vit = stats.vit;
            spd = stats.spd;
            intStat = stats.intStat;
            crt = stats.crt;

            if (player != null)
            {
                player.str = str;
                player.vit = vit;
                player.spd = spd;
                player.intStat = intStat;
                player.crt = crt;

                // Sync gold from GameManager to Player
                player.gold = GameManager.Instance.GetPlayerGold();
            }

            // Sync gold to UI
            if (goldText != null)
            {
                goldText.text = GameManager.Instance.GetPlayerGold().ToString();
            }

            UpdateUI(); // Update the UI with the synced stats
        }
    }
}
