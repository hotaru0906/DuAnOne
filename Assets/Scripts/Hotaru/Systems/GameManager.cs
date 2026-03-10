using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Skill Unlock Flags")]
    public bool unlockedSkill1 = false;
    public bool unlockedSkill2 = false;
    public bool unlockedBullet = false;

    [Header("Player Data")]
    public int playerGold;

    [Header("Player Leveling")]
    public int playerLevel = 1;
    public int playerStatPoints = 0;

    [Header("Player Stats")]
    public int playerStr = 5;
    public int playerVit = 5;
    public int playerSpd = 5;
    public int playerInt = 5;
    public int playerCrt = 5;

    [Header("Player Potions")]
    public int healthPotion = 0;
    public int manaPotion = 0;
    public int recallPotion = 0;

    [Header("Player EXP")]
    public int currentExp = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadPlayerGold();
            LoadPlayerStats();
            LoadPlayerLevel();
            LoadPlayerPotions();
            LoadPlayerExp();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
{
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ResetPlayerData();
            Debug.Log("Player data reset to default!");
        }
    
        if (Input.GetKeyDown(KeyCode.E))
        {
            var player = FindObjectOfType<Player>();
            if (player != null)
            {
                playerGold = player.gold;
                SavePlayerGold(playerGold);
                SavePlayerStats(player.str, player.vit, player.spd, player.intStat, player.crt);
                SavePlayerLevel(player.level, player.statPoints);
                SavePlayerExp(player.currentExp);
                SavePlayerPotions(player.healthPotionCount, player.manaPotionCount, player.teleportPotionCount);
            }
        }
    }

    // Hàm reset player data về mặc định
    public void ResetPlayerData()
    {
        playerGold = 0;
        playerLevel = 1;
        playerStatPoints = 0;
        playerStr = 5;
        playerVit = 5;
        playerSpd = 5;
        playerInt = 5;
        playerCrt = 5;
        healthPotion = 0;
        manaPotion = 0;
        recallPotion = 0;
        currentExp = 0;
        unlockedSkill1 = false;
        unlockedSkill2 = false;
        unlockedBullet = false;
        SavePlayerData();
    }
    public void SavePlayerData()
    {
        SavePlayerGold(playerGold);
        SavePlayerStats(playerStr, playerVit, playerSpd, playerInt, playerCrt);
        SavePlayerLevel(playerLevel, playerStatPoints);
        SavePlayerExp(currentExp);
        SavePlayerPotions(healthPotion, manaPotion, recallPotion);
        SaveSkillUnlocks(unlockedSkill1, unlockedSkill2, unlockedBullet);
    }

    // Gold
    public void SavePlayerGold(int gold)
    {
        playerGold = gold;
        PlayerPrefs.SetInt("PlayerGold", gold);
        PlayerPrefs.Save();
        Debug.Log($"Player gold saved: {gold}");
    }

    public int LoadPlayerGold()
    {
        if (PlayerPrefs.HasKey("PlayerGold"))
        {
            playerGold = PlayerPrefs.GetInt("PlayerGold");
            Debug.Log($"Player gold loaded: {playerGold}");
        }
        else
        {
            Debug.Log("No saved gold found. Using Inspector value.");
        }
        return playerGold;
    }

    // Stats
    public void SavePlayerStats(int str, int vit, int spd, int intStat, int crt)
    {
        playerStr = str;
        playerVit = vit;
        playerSpd = spd;
        playerInt = intStat;
        playerCrt = crt;

        PlayerPrefs.SetInt("PlayerStr", str);
        PlayerPrefs.SetInt("PlayerVit", vit);
        PlayerPrefs.SetInt("PlayerSpd", spd);
        PlayerPrefs.SetInt("PlayerInt", intStat);
        PlayerPrefs.SetInt("PlayerCrt", crt);
        PlayerPrefs.Save();

        Debug.Log($"Player stats saved: STR={str}, VIT={vit}, SPD={spd}, INT={intStat}, CRT={crt}");
    }

    public void LoadPlayerStats()
    {
        if (PlayerPrefs.HasKey("PlayerStr"))
        {
            playerStr = PlayerPrefs.GetInt("PlayerStr");
            playerVit = PlayerPrefs.GetInt("PlayerVit");
            playerSpd = PlayerPrefs.GetInt("PlayerSpd");
            playerInt = PlayerPrefs.GetInt("PlayerInt");
            playerCrt = PlayerPrefs.GetInt("PlayerCrt");

            Debug.Log($"Player stats loaded: STR={playerStr}, VIT={playerVit}, SPD={playerSpd}, INT={playerInt}, CRT={playerCrt}");
        }
        else
        {
            Debug.Log("No saved stats found. Using Inspector values.");
        }
    }

    // Level & Stat Points
    public void SavePlayerLevel(int level, int points)
    {
        playerLevel = level;
        playerStatPoints = points;

        PlayerPrefs.SetInt("PlayerLevel", level);
        PlayerPrefs.SetInt("PlayerStatPoints", points);
        PlayerPrefs.Save();

        Debug.Log($"Saved level = {level}, stat points = {points}");
    }

    public void LoadPlayerLevel()
    {
        if (PlayerPrefs.HasKey("PlayerLevel"))
        {
            playerLevel = PlayerPrefs.GetInt("PlayerLevel");
            playerStatPoints = PlayerPrefs.GetInt("PlayerStatPoints", 0); // Default to 0 if not found
            Debug.Log($"Player level loaded: {playerLevel}, Stat Points: {playerStatPoints}");
        }
        else
        {
            Debug.Log("No saved level found. Using default values.");
        }
    }

    // EXP
    public void SavePlayerExp(int currentExp)
    {
        this.currentExp = currentExp;
        PlayerPrefs.SetInt("PlayerCurrentExp", currentExp);
        PlayerPrefs.Save();
        Debug.Log($"Saved EXP: current = {currentExp}");
    }

    public void LoadPlayerExp()
    {
        currentExp = PlayerPrefs.GetInt("PlayerCurrentExp", 0);
        Debug.Log($"Loaded EXP: current = {currentExp}");
    }

    // Potions
    public void SavePlayerPotions(int hp, int mp, int rp)
    {
        healthPotion = hp;
        manaPotion = mp;
        recallPotion = rp;

        PlayerPrefs.SetInt("HealthPotion", hp);
        PlayerPrefs.SetInt("ManaPotion", mp);
        PlayerPrefs.SetInt("RecallPotion", rp);
        PlayerPrefs.Save();

        Debug.Log($"Saved potions: HP={hp}, MP={mp}, Recall={rp}");
    }

    public void LoadPlayerPotions()
    {
        healthPotion = PlayerPrefs.GetInt("HealthPotion", 0);
        manaPotion = PlayerPrefs.GetInt("ManaPotion", 0);
        recallPotion = PlayerPrefs.GetInt("RecallPotion", 0);

        Debug.Log($"Loaded potions: HP={healthPotion}, MP={manaPotion}, Recall={recallPotion}");
    }

    // Getters (tuỳ bạn có dùng hay không)
    public int GetPlayerGold() => playerGold;

    public (int str, int vit, int spd, int intStat, int crt) GetPlayerStats()
    {
        return (playerStr, playerVit, playerSpd, playerInt, playerCrt);
    }

    public (int level, int points) GetPlayerLevel()
    {
        return (playerLevel, playerStatPoints);
    }

    public int GetCurrentExp() => currentExp;

    public void SaveSkillUnlocks(bool skill1, bool skill2, bool bullet)
    {
        unlockedSkill1 = skill1;
        unlockedSkill2 = skill2;
        unlockedBullet = bullet;

        PlayerPrefs.SetInt("UnlockedSkill1", skill1 ? 1 : 0);
        PlayerPrefs.SetInt("UnlockedSkill2", skill2 ? 1 : 0);
        PlayerPrefs.SetInt("UnlockedBullet", bullet ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSkillUnlocks()
    {
        unlockedSkill1 = PlayerPrefs.GetInt("UnlockedSkill1", 0) == 1;
        unlockedSkill2 = PlayerPrefs.GetInt("UnlockedSkill2", 0) == 1;
        unlockedBullet = PlayerPrefs.GetInt("UnlockedBullet", 0) == 1;
    }
}
