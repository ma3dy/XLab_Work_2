using UnityEngine;

public class Unit
{
    public string Name { get; private set; }
    public int Health { get; private set; }
    public int MinDamage { get; private set; }
    public int MaxDamage { get; private set; }

    public Unit(string name, int health, int minDamage, int maxDamage)
    {
        Name = name;
        Health = health;
        MinDamage = minDamage;
        MaxDamage = maxDamage;
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);
    }

    public int GetRandomDamage()
    {
        return Random.Range(MinDamage, MaxDamage + 1);
    }
}