using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public float moveRange;
    public float attackRange;
    public float health;
    public float attackDamage;
    public float defense;
    // public float intellect;
    // public float spirit;
    // public float mastery;

    public const float STAT_VARIANCE = 5f;

    public UnitStats(float newMovRange, float newAttRange, float newHealth, float newAttDam, float newDef)
    {
        moveRange = newMovRange;
        attackRange = newAttRange;
        health = newHealth;
        attackDamage = newAttDam;
        defense = newDef;
        // intellect = newIntellect;
        // spirit = newSpirit;
        // mastery = newMastery;
    }

    public void LevelUpStats(UnitStats classGrowths, UnitStats unitGrowths)
    {
        float rand = Random.value;

        ApplyGrowth(ref moveRange, classGrowths.moveRange + unitGrowths.moveRange, rand);
        ApplyGrowth(ref attackRange, classGrowths.attackRange + unitGrowths.attackRange, rand);
        ApplyGrowth(ref health, classGrowths.health + unitGrowths.health, rand);
        ApplyGrowth(ref attackDamage, classGrowths.attackDamage + unitGrowths.attackDamage, rand);
        ApplyGrowth(ref defense, classGrowths.defense + unitGrowths.defense, rand);
        // ApplyGrowth(ref intellect, classGrowths.intellect + unitGrowths.intellect, rand);
        // ApplyGrowth(ref spirit, classGrowths.spirit + unitGrowths.spirit, rand);
        // ApplyGrowth(ref mastery, classGrowths.mastery + unitGrowths.mastery, rand);
    }

    public static float InitializeStatValue(float baseValue, float growth)
    {
        float min = Mathf.Clamp(baseValue - (growth * STAT_VARIANCE), 0, float.MaxValue);
        float max = baseValue + (growth * STAT_VARIANCE);

        return Mathf.Round(Random.Range(min, max));
    }

    private void ApplyGrowth(ref float stat, float growth, float rand)
    {
        if(growth > 1f)
        {
            int guaranteedIncrease = Mathf.FloorToInt(growth);
            stat += guaranteedIncrease;
            growth -= growth;
        }

        if (growth > rand)
        {
            stat++;
        }
    }
}
