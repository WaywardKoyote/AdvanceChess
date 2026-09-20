using UnityEngine;

[System.Serializable]
public struct UnitStats
{
    public float speed;
    public float perception;
    public float endurance;
    public float strength;
    public float luck;
    public float intellect;
    public float spirit;
    public float mastery;

    public UnitStats(float newSpeed, float newPerception, float newEndurance, float newStrength, float newLuck, float newIntellect, float newSpirit, float newMastery)
    {
        speed = newSpeed;
        perception = newPerception;
        endurance = newEndurance;
        strength = newStrength;
        luck = newLuck;
        intellect = newIntellect;
        spirit = newSpirit;
        mastery = newMastery;
    }

    public void LevelUpStats()
    {
        speed += Random.Range(0, 10);
        perception += Random.Range(0, 10);
        endurance += Random.Range(0, 10);
        strength += Random.Range(0, 10);
        luck += Random.Range(0, 10);
        intellect += Random.Range(0, 10);
        spirit += Random.Range(0, 10);
        mastery += Random.Range(0, 10);
    }
}
