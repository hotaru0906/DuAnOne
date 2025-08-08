[System.Serializable]
public class SaveData
{
    public string currentSceneName;
    public float playerPosX;
    public float playerPosY;

    public int str, vit, spd, intStat, crt;
    public int level, currentExp, statPoints;
    public int gold;
    public int health, maxHealth, mana, maxMana;

    public int healthPotion, manaPotion, recallPotion;

    public bool unlockedSkill1, unlockedSkill2, unlockedBullet;

    // public List<string> defeatedBosses = new List<string>();
}
