using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveSystem : MonoBehaviour
{
    public static void SaveGame(string saveFileName)
    {
        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Cannot find Player to save.");
            return;
        }

        SaveData data = new SaveData
        {
            currentSceneName = SceneManager.GetActiveScene().name,
            playerPosX = player.transform.position.x,
            playerPosY = player.transform.position.y,

            str = player.str,
            vit = player.vit,
            spd = player.spd,
            intStat = player.intStat,
            crt = player.crt,
            level = player.level,
            currentExp = player.currentExp,
            statPoints = player.statPoints,
            gold = player.gold,

            health = player.Health,
            maxHealth = player.MaxHealth,
            mana = player.Mana,
            maxMana = player.MaxMana,

            healthPotion = player.healthPotionCount,
            manaPotion = player.manaPotionCount,
            recallPotion = player.teleportPotionCount,

            unlockedSkill1 = player.canUseSkill1,
            unlockedSkill2 = player.canUseSkill2,
            unlockedBullet = player.canShootBullet,
        };

        string path = Application.persistentDataPath + $"/{saveFileName}.json";
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
        Debug.Log("Game Saved to " + path);
    }

    public static void DeleteSave(string saveFileName)
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted: " + path);
        }
    }

    // Load cần gọi từ MonoBehaviour vì có coroutine
    public static IEnumerator LoadGame(string saveFileName)
    {
        string path = Application.persistentDataPath + $"/{saveFileName}.json";
        if (!File.Exists(path))
        {
            Debug.LogError("Save file not found: " + path);
            yield break;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(data.currentSceneName);
        yield return new WaitUntil(() => loadOp.isDone);

        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            player.transform.position = new Vector2(data.playerPosX, data.playerPosY);
            player.str = data.str;
            player.vit = data.vit;
            player.spd = data.spd;
            player.intStat = data.intStat;
            player.crt = data.crt;
            player.level = data.level;
            player.currentExp = data.currentExp;
            player.statPoints = data.statPoints;
            player.gold = data.gold;

            player.Health = data.health;
            player.MaxHealth = data.maxHealth;
            player.Mana = data.mana;
            player.MaxMana = data.maxMana;

            player.healthPotionCount = data.healthPotion;
            player.manaPotionCount = data.manaPotion;
            player.teleportPotionCount = data.recallPotion;

            player.canUseSkill1 = data.unlockedSkill1;
            player.canUseSkill2 = data.unlockedSkill2;
            player.canShootBullet = data.unlockedBullet;

            player.ApplyStats();
        }
    }
}
