using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    public enum ShopkeeperType
    {
        Smiter,
        Wizard
    }

    public ShopkeeperType shopkeeperType;

    [Header("Smiter Settings")]
    public List<string> smiterItems = new List<string> { "STR", "VIT", "SPD", "INT", "CRT" };

    [Header("Wizard Settings")]
    public List<string> wizardItems = new List<string> { "Health Potion", "Mana Potion", "Recall Potion" };

    [Header("Upgrade Costs")]
    public int baseUpgradeCost = 5; // Initial cost for upgrading
    private int currentUpgradeCost;

    [Header("TMP References for Stats")]
    public TMPro.TMP_Text strText;
    public TMPro.TMP_Text vitText;
    public TMPro.TMP_Text spdText;
    public TMPro.TMP_Text intText;
    public TMPro.TMP_Text crtText;

    [Header("Potion Costs")]
    public int healthPotionCost = 8;
    public int manaPotionCost = 15;
    public int recallPotionCost = 50;

    [Header("TMP References for Potions")]
    public TMPro.TMP_Text healthPotionText;
    public TMPro.TMP_Text manaPotionText;
    public TMPro.TMP_Text recallPotionText;

    public StatsController statsController; // Reference to StatsController

    // Start is called before the first frame update
    void Start()
    {
        if (shopkeeperType == ShopkeeperType.Smiter)
        {
            currentUpgradeCost = baseUpgradeCost; // Initialize the upgrade cost

            // Initialize TMP text for all stats
            if (strText != null) DisplayUpgradeMessage("STR", currentUpgradeCost, strText);
            if (vitText != null) DisplayUpgradeMessage("VIT", currentUpgradeCost, vitText);
            if (spdText != null) DisplayUpgradeMessage("SPD", currentUpgradeCost, spdText);
            if (intText != null) DisplayUpgradeMessage("INT", currentUpgradeCost, intText);
            if (crtText != null) DisplayUpgradeMessage("CRT", currentUpgradeCost, crtText);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Interact()
    {
        if (shopkeeperType == ShopkeeperType.Smiter)
        {
            Debug.Log("Smiter: Available upgrades - " + string.Join(", ", smiterItems));

            // Update TMP text when interacting with the Smiter
            if (strText != null) DisplayUpgradeMessage("STR", currentUpgradeCost, strText);
            if (vitText != null) DisplayUpgradeMessage("VIT", currentUpgradeCost, vitText);
            if (spdText != null) DisplayUpgradeMessage("SPD", currentUpgradeCost, spdText);
            if (intText != null) DisplayUpgradeMessage("INT", currentUpgradeCost, intText);
            if (crtText != null) DisplayUpgradeMessage("CRT", currentUpgradeCost, crtText);
        }
        else if (shopkeeperType == ShopkeeperType.Wizard)
        {
            Debug.Log("Wizard: Available potions - " + string.Join(", ", wizardItems));
            // Add logic for selling potions
        }
    }

    private void DisplayUpgradeMessage(string statName, int currentCost, TMPro.TMP_Text targetText)
    {
        if (targetText != null)
        {
            Debug.Log($"Updating TMP for {statName}: Cost = {currentCost}, TargetText = {targetText.name}");
            targetText.text = $"Nâng cấp \"{statName}\" + 1 \"{statName}\" Gold = {currentCost}";
        }
        else
        {
            Debug.LogError($"TargetText for {statName} is null. Cannot update TMP.");
        }
    }

    public void UpgradeStrength(Player player)
    {
        Debug.Log("Attempting to upgrade STR...");
        if (shopkeeperType == ShopkeeperType.Smiter && player.gold >= currentUpgradeCost)
        {
            player.gold -= currentUpgradeCost; // Deduct gold from the player
            player.str += 1; // Increase player's STR stat
            currentUpgradeCost += 5; // Increase the cost for the next upgrade

            Debug.Log("STR upgraded successfully. Calling DisplayUpgradeMessage...");
            if (strText == null)
            {
                Debug.LogError("strText TMP reference is null. Please assign it in the Inspector.");
            }
            DisplayUpgradeMessage("STR", currentUpgradeCost, strText);

            // Sync stats with StatsController
            if (statsController != null)
            {
                statsController.SyncStatsFromPlayer();
            }

            // Save updated stats to GameManager
            if (GameManager.Instance != null)
            {
                var stats = statsController;
                GameManager.Instance.SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                GameManager.Instance.SavePlayerGold(player.gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to upgrade STR.");
        }
    }

    public void UpgradeVitality(Player player)
    {
        Debug.Log("Attempting to upgrade VIT...");
        if (shopkeeperType == ShopkeeperType.Smiter && player.gold >= currentUpgradeCost)
        {
            player.gold -= currentUpgradeCost; // Deduct gold from the player
            player.vit += 1; // Increase player's VIT stat
            currentUpgradeCost += 5; // Increase the cost for the next upgrade

            Debug.Log("VIT upgraded successfully. Calling DisplayUpgradeMessage...");
            if (vitText == null)
            {
                Debug.LogError("vitText TMP reference is null. Please assign it in the Inspector.");
            }
            DisplayUpgradeMessage("VIT", currentUpgradeCost, vitText);

            // Sync stats with StatsController
            if (statsController != null)
            {
                statsController.SyncStatsFromPlayer();
            }

            // Save updated stats to GameManager
            if (GameManager.Instance != null)
            {
                var stats = statsController;
                GameManager.Instance.SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                GameManager.Instance.SavePlayerGold(player.gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to upgrade VIT.");
        }
    }

    public void UpgradeSpeed(Player player)
    {
        Debug.Log("Attempting to upgrade SPD...");
        if (shopkeeperType == ShopkeeperType.Smiter && player.gold >= currentUpgradeCost)
        {
            player.gold -= currentUpgradeCost; // Deduct gold from the player
            player.spd += 1; // Increase player's SPD stat
            currentUpgradeCost += 5; // Increase the cost for the next upgrade

            Debug.Log("SPD upgraded successfully. Calling DisplayUpgradeMessage...");
            if (spdText == null)
            {
                Debug.LogError("spdText TMP reference is null. Please assign it in the Inspector.");
            }
            DisplayUpgradeMessage("SPD", currentUpgradeCost, spdText);

            // Sync stats with StatsController
            if (statsController != null)
            {
                statsController.SyncStatsFromPlayer();
            }

            // Save updated stats to GameManager
            if (GameManager.Instance != null)
            {
                var stats = statsController;
                GameManager.Instance.SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                GameManager.Instance.SavePlayerGold(player.gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to upgrade SPD.");
        }
    }

    public void UpgradeIntelligence(Player player)
    {
        Debug.Log("Attempting to upgrade INT...");
        if (shopkeeperType == ShopkeeperType.Smiter && player.gold >= currentUpgradeCost)
        {
            player.gold -= currentUpgradeCost; // Deduct gold from the player
            player.intStat += 1; // Increase player's INT stat
            currentUpgradeCost += 5; // Increase the cost for the next upgrade

            Debug.Log("INT upgraded successfully. Calling DisplayUpgradeMessage...");
            if (intText == null)
            {
                Debug.LogError("intText TMP reference is null. Please assign it in the Inspector.");
            }
            DisplayUpgradeMessage("INT", currentUpgradeCost, intText);

            // Sync stats with StatsController
            if (statsController != null)
            {
                statsController.SyncStatsFromPlayer();
            }

            // Save updated stats to GameManager
            if (GameManager.Instance != null)
            {
                var stats = statsController;
                GameManager.Instance.SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                GameManager.Instance.SavePlayerGold(player.gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to upgrade INT.");
        }
    }

    public void UpgradeCriticalRate(Player player)
    {
        Debug.Log("Attempting to upgrade CRT...");
        if (shopkeeperType == ShopkeeperType.Smiter && player.gold >= currentUpgradeCost)
        {
            player.gold -= currentUpgradeCost; // Deduct gold from the player
            player.crt += 1; // Increase player's CRT stat
            currentUpgradeCost += 5; // Increase the cost for the next upgrade

            Debug.Log("CRT upgraded successfully. Calling DisplayUpgradeMessage...");
            if (crtText == null)
            {
                Debug.LogError("crtText TMP reference is null. Please assign it in the Inspector.");
            }
            DisplayUpgradeMessage("CRT", currentUpgradeCost, crtText);

            // Sync stats with StatsController
            if (statsController != null)
            {
                statsController.SyncStatsFromPlayer();
            }

            // Save updated stats to GameManager
            if (GameManager.Instance != null)
            {
                var stats = statsController;
                GameManager.Instance.SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                GameManager.Instance.SavePlayerGold(player.gold);
            }
        }
        else
        {
            Debug.Log("Not enough gold to upgrade CRT.");
        }
    }

    public void BuyHealthPotion(Player player)
    {
        if (player.gold >= healthPotionCost)
        {
            player.gold -= healthPotionCost;
            player.healthPotionCount++;
            Debug.Log("Bought a health potion.");
            player.UpdatePotionUI();
            GameManager.Instance.SavePlayerPotions(player.healthPotionCount, player.manaPotionCount,
            player.teleportPotionCount);
        }
        else
        {
            Debug.Log("Not enough gold to buy a health potion.");
        }
    }

    public void BuyManaPotion(Player player)
    {
        if (player.gold >= manaPotionCost)
        {
            player.gold -= manaPotionCost;
            player.manaPotionCount++;
            Debug.Log("Bought a mana potion.");
            player.UpdatePotionUI();
            GameManager.Instance.SavePlayerPotions(player.healthPotionCount, player.manaPotionCount,
            player.teleportPotionCount);
        }
        else
        {
            Debug.Log("Not enough gold to buy a mana potion.");
        }
    }

    public void BuyRecallPotion(Player player)
    {
        if (player.gold >= recallPotionCost)
        {
            player.gold -= recallPotionCost;
            player.teleportPotionCount++;
            Debug.Log("Bought a recall potion.");
            player.UpdatePotionUI();
            GameManager.Instance.SavePlayerPotions(player.healthPotionCount, player.manaPotionCount,
            player.teleportPotionCount);
        }
        else
        {
            Debug.Log("Not enough gold to buy a recall potion.");
        }
    }
}
